using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace ConsoleStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Stopwatch sw = new Stopwatch();

            sw.Start();

            using (FileStream fs = new FileStream(@"c:\files\file1.txt", FileMode.Create))
            {
                for (int i = 0; i < 200000; i++)
                {
                    fs.Position = 10;
                    fs.WriteByte(100);

                    fs.Position = 5;
                    fs.WriteByte(100);
                    fs.Position = 15;
                    fs.WriteByte(100);
                    fs.Position = 3;
                    fs.WriteByte(100);
                }
            }

            sw.Stop();
            Console.WriteLine($"Filestream: {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            using (MemoryMappedFile mmf =
                MemoryMappedFile.CreateFromFile
                (@"c:\files\file2.txt", FileMode.Create, "map1", 1000))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    for (int i = 0; i < 200000; i++)
                    {
                        accessor.Write(10, (byte)101);
                        accessor.Write(5, (byte)100);
                        accessor.Write(15, (byte)100);
                        accessor.Write(3, (byte)100);
                    }
                }
            }

            sw.Stop();
            Console.WriteLine($"Filestream: {sw.Elapsed}");
            sw.Reset();
        }
    }
}
