using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace WPTools.NET
{
    public class localBuilder
    {
        public string DIR;
        public string DEST;

        public localBuilder(string dir, string dest)
        {
            DIR = dir;
            DEST = dest;

            if (!Directory.Exists(DEST))
            {
                Console.WriteLine("The build folder does not exist.");
                Directory.CreateDirectory(DEST);
            }
            else
            {
                Console.WriteLine("The build folder is already present.");
            }
        }

        public void XCopy(string source, string dest, int permissions = 0755)
        {
            string sourceHash = HashDirectory(source);

            if (File.Exists(source))
            {
                File.Copy(source, dest);
                return;
            }

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            DirectoryInfo dir = new DirectoryInfo(source);
            foreach (FileSystemInfo entry in dir.GetFileSystemInfos())
            {
                if (entry.Name == "." || entry.Name == "..")
                {
                    continue;
                }

                if (sourceHash != HashDirectory(Path.Combine(source, entry.Name)))
                {
                    XCopy(Path.Combine(source, entry.Name), Path.Combine(dest, entry.Name), permissions);
                }
            }
        }

        public string HashDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return string.Empty;
            }

            string[] files = Directory.GetFiles(directory);
            string[] subdirectories = Directory.GetDirectories(directory);
            string[] allFiles = new string[files.Length + subdirectories.Length];

            Array.Copy(files, allFiles, files.Length);
            Array.Copy(subdirectories, 0, allFiles, files.Length, subdirectories.Length);

            using (var md5 = MD5.Create())
            {
                byte[] combinedHash = new byte[md5.HashSize / 8];

                foreach (string file in allFiles)
                {
                    byte[] fileHash;

                    if (File.Exists(file))
                    {
                        using (var stream = File.OpenRead(file))
                        {
                            fileHash = md5.ComputeHash(stream);
                        }
                    }
                    else if (Directory.Exists(file))
                    {
                        fileHash = Encoding.ASCII.GetBytes(HashDirectory(file));
                    }
                    else
                    {
                        continue;
                    }

                    for (int i = 0; i < combinedHash.Length; i++)
                    {
                        combinedHash[i] ^= fileHash[i % fileHash.Length];
                    }
                }

                return BitConverter.ToString(combinedHash).Replace("-", string.Empty);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: dotnet run <DIR> <DEST>");
                return;
            }

            string dir = args[0];
            string dest = args[1];

            localBuilder builder = new localBuilder(dir, dest);

            builder.XCopy(Path.Combine(builder.DIR, "index.php"), Path.Combine(builder.DEST, "index.php"));
            builder.XCopy(Path.Combine(builder.DIR, "uninstall.php"), Path.Combine(builder.DEST, "uninstall.php"));
            builder.XCopy(Path.Combine(builder.DIR, "readme.txt"), Path.Combine(builder.DEST, "readme.txt"));
            builder.XCopy(Path.Combine(builder.DIR, "aruba-hispeed-cache.php"), Path.Combine(builder.DEST, "aruba-hispeed-cache.php"));

            builder.XCopy(Path.Combine(builder.DIR, "app"), Path.Combine(builder.DEST, "app"));
            builder.XCopy(Path.Combine(builder.DIR, "languages"), Path.Combine(builder.DEST, "languages"));

            // remove dev file
            string adminClassTestPath = Path.Combine(builder.DEST, "app", "src", "Admin", "AdminClassTest.php");
            if (File.Exists(adminClassTestPath))
            {
                File.Delete(adminClassTestPath);
            }
        }
    }
}
