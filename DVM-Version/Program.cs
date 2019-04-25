using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DVM_Version
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Title = "DeviceVM Version File Generator";
            if (args.Length == 0 || args[0] == "help" || args[0] == "?")
            {
                Console.WriteLine("DeviceVM SplashTop Version File Generator");
                Console.WriteLine("Usage:");
                Console.WriteLine("   DVM-Version.exe SPLASHTOP_FOLDER OUTPUTFILE [VERSION] [DATE]");
                Console.WriteLine("   [DATE]: DD/MM/YYYY");
            }
            else
            {
                string src = args[0];
                string target = args[1];
                Version splashVersion = null;
                DateTime splashDate = DateTime.Now;

                Int32 binaryMagic = 0x00000006;
                Int32 archiveMagic = 0x00000003;
                Int32 kernelMagic = 0x00000002;
                Int32 dataMagic = 0x00000005;

                if(args.Length >= 3)
                    splashVersion = new Version(args[2]);

                if (args.Length == 4)
                {
                    string[] vals = args[3].Split('/');

                    splashDate = new DateTime(Convert.ToInt32(vals[2]), Convert.ToInt32(vals[1]), Convert.ToInt32(vals[0]));
                }

                if (!Directory.Exists(src))
                {
                    Console.WriteLine("Specified Image does not exist");
                    return;
                }

                Console.WriteLine("DeviceVM SplashTop Version File Generator");
                Console.WriteLine("Source Directory: " + args[0]);
                Console.WriteLine("Target File: " + args[1]);
                Console.WriteLine();

                if (splashVersion == null)
                {
                    Console.WriteLine("Please Enter the Version [x.x.x.x]:");
                    splashVersion = new Version(Console.ReadLine());
                }
                


                Console.WriteLine("Generating Header");
                byte[] file = new byte[0];
                CreationFunctions.AddUTF8String(ref file,"_DeviceVM Inc._",16); //[UTF8] (16) - Company
                CreationFunctions.AddUTF8String(ref file, splashDate.ToString("yyyyMMdd"), 8); //[UTF8] (8) - Date
                CreationFunctions.AddByte(ref file, (byte)splashVersion.Major); //[1] - Major
                CreationFunctions.AddByte(ref file, (byte)splashVersion.Minor); //[1] - Minor
                CreationFunctions.AddByte(ref file, (byte)splashVersion.Build); //[1] - Build
                CreationFunctions.AddByte(ref file, (byte)splashVersion.Revision); //[1] - Rev
                CreationFunctions.AddInt32(ref file, 0x00000075); //[4] - Magic


                MD5 md5sum = MD5CryptoServiceProvider.Create();

                //Files

                Console.WriteLine();
                Console.WriteLine("Generating File Entries");
                foreach (string fileInfo in Directory.GetFiles(src))
                {
                    
                    FileInfo fi = new FileInfo(fileInfo);
                    Console.WriteLine("\t" + fi.Name);
                    CreationFunctions.AddUTF8String(ref file, fi.Name, 32);  //[UTF8] - File Name
                    CreationFunctions.AddUTF8String(ref file, splashDate.ToString("yyyyMMdd"), 8); //[UTF8] - Date (yyyyMMdd)
                    if(fi.Name == "kernel.bin")
                        CreationFunctions.AddInt32(ref file, kernelMagic); //[4] - Magic
                    else if(fi.Extension == ".sqx")
                        CreationFunctions.AddInt32(ref file, archiveMagic); //[4] - Magic
                    else if (fi.Extension == ".dat" || fi.Extension == ".date" || fi.Extension == ".md5")
                        CreationFunctions.AddInt32(ref file, dataMagic); //[4] - Magic
                    else if(fi.Extension == ".bin" || fi.Extension == ".idx")
                        CreationFunctions.AddInt32(ref file, binaryMagic); //[4] - Magic
                    else
                        CreationFunctions.AddInt32(ref file, binaryMagic); //[4] - Magic
                    CreationFunctions.AddInt32(ref file, 0x0); //[4] - Padding
                    CreationFunctions.AddInt32(ref file, (int)fi.Length); //[4] - File Size
                    CreationFunctions.AddData(ref file, new byte[12]); //[12] - Padding

                    FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
                    CreationFunctions.AddData(ref file, md5sum.ComputeHash(fs)); //[16] - MD5
                    fs.Close();
                }


                FileStream outFS = new FileStream(target, FileMode.OpenOrCreate, FileAccess.Write);
                outFS.Write(file, 0, file.Length);
                outFS.Close();


                Console.WriteLine("Complete");
            }
        }
    }
}
