using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVM_Compare
{
    class Program
    {
        static bool Equal<T>(T[] array1, T[] array2) where T : struct
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
                if (!array1[i].Equals(array2[i]))
                    return false;

            return true;
        }

        static void Main(string[] args)
        {
            Console.Title = "DeviceVM Version File Comparer";
            if (args.Length == 0 || args[0].Contains("help") || args[0].Contains("?"))
            {
                Console.WriteLine("DeviceVM SplashTop Version File Comparer");
                Console.WriteLine("Usage:");
                Console.WriteLine("   DVM-Compare.exe  FILE1 FILE2");
            }
            else
            {
                string file1 = args[0];
                string file2 = args[1];

                if (!File.Exists(file1))
                {
                    Console.WriteLine("Specified FILE1 does not exist");
                    return;
                }

                if (!File.Exists(file2))
                {
                    Console.WriteLine("Specified FILE2 does not exist");
                    return;
                }

                Console.WriteLine("DeviceVM SplashTop Version File Comparer");
                Console.WriteLine("File1: " + args[0]);
                Console.WriteLine("File2: " + args[1]);
                Console.WriteLine();

                SortedList<string, byte[]> file1Items = new SortedList<string, byte[]>();

                FileStream fs1 = new FileStream(args[0], FileMode.Open, FileAccess.Read);
                byte[] f1 = new byte[(int)fs1.Length];
                fs1.Read(f1, 0, f1.Length);
                fs1.Close();

                int pos = 32;
                while (pos < f1.Length)
                {
                    string name;
                    byte[] data;

                    pos += SierraLib.ByteManipulation.ParsingFunctions.ExtractData(f1, pos,32,out data);
                    name = Encoding.UTF8.GetString(data);
                    data = null;
                    pos += SierraLib.ByteManipulation.ParsingFunctions.ExtractData(f1, pos, 48, out data);


                    file1Items.Add(name, data);
                }


                fs1 = new FileStream(args[1], FileMode.Open, FileAccess.Read);
                f1 = new byte[(int)fs1.Length];
                fs1.Read(f1, 0, f1.Length);
                fs1.Close();

                pos = 32;
                while (pos < f1.Length)
                {
                    string name;
                    byte[] data;

                    pos += SierraLib.ByteManipulation.ParsingFunctions.ExtractData(f1, pos, 32, out data);
                    name = Encoding.UTF8.GetString(data);
                    data = null;
                    pos += SierraLib.ByteManipulation.ParsingFunctions.ExtractData(f1, pos, 48, out data);


                    if (!file1Items.ContainsKey(name))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Missing File Reference in FILE1 (in FILE2):");
                        Console.WriteLine("\t" + name);
                    }
                    else
                        
                    {
                        if (!Equal<byte>(file1Items[name],data))
                        {
                            Console.WriteLine();
                            Console.WriteLine("Different Definition in FILE1 (compared to FILE2):");
                            Console.WriteLine("\t" + name);
                            Console.WriteLine(Convert.ToBase64String(data));
                            Console.WriteLine(Convert.ToBase64String(file1Items[name]));
                            file1Items.Remove(name);
                        }
                        else
                        {
                            file1Items.Remove(name);
                        }
                    }
                }


                foreach (string itm in file1Items.Keys)
                {
                    Console.WriteLine("Missing File Reference in FILE2 (in FILE1):");
                    Console.WriteLine("\t" + itm);
                }

                if (file1Items.Keys.Count == 0)
                {
                    Console.WriteLine("Both Files are Identical");

                }

                Console.ReadKey();
            }
        }

        
    }
}
