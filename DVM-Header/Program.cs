using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DVM_Header
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "DeviceVM Header File Generator";
            if (args.Length == 0 || args[0] == "help" || args[0] == "?")
            {
                Console.WriteLine("DeviceVM SplashTop Version File Generator");
                Console.WriteLine("Usage:");
                Console.WriteLine("   DVM-Version.exe IMAGE OUTPUTFILE [VERSION]");
            }
            else
            {
                string img = args[0];
                string target = args[1];
                Version splashVersion = null;
                if (args.Length == 3)
                    splashVersion = new Version(args[2]);

                if (!File.Exists(img))
                {
                    Console.WriteLine("Specified Image Folder does not exist");
                    return;
                }

                Console.WriteLine("DeviceVM SplashTop Header File Generator");
                Console.WriteLine("Target File: " + args[0]);
                Console.WriteLine();

                if (splashVersion == null)
                {
                    Console.WriteLine("Please Enter the Version [x.x.x.x]:");
                    splashVersion = new Version(Console.ReadLine());
                }
                DateTime splashDate = DateTime.Now;


                Console.WriteLine("Generating Header");
                byte[] file = new byte[0];
                CreationFunctions.AddUTF8String(ref file, "_DeviceVM Inc._", 16);
                CreationFunctions.AddUTF8String(ref file, splashDate.ToString("yyyyMMdd"), 8);
                CreationFunctions.AddByte(ref file, (byte)splashVersion.Major);
                CreationFunctions.AddByte(ref file, (byte)splashVersion.Minor);
                CreationFunctions.AddByte(ref file, (byte)splashVersion.Build);
                CreationFunctions.AddByte(ref file, (byte)splashVersion.Revision);
                CreationFunctions.AddUTF8String(ref file, "r/jo",4);

                Console.WriteLine("Generating MD5SUM");
                MD5 md5sum = MD5CryptoServiceProvider.Create();
                FileStream fs = new FileStream(img, FileMode.Open, FileAccess.Read);
                CreationFunctions.AddData(ref file, md5sum.ComputeHash(fs));
                fs.Close();


                FileStream outFS = new FileStream(target, FileMode.OpenOrCreate, FileAccess.Write);
                outFS.Write(file, 0, file.Length);
                outFS.Close();


                Console.WriteLine("Complete");
            }
        }
    }
}
