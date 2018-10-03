using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyDirectoryContents
{
    class Program
    {
        const string sourcePath = @"E:\Test";
        const string destPath = @"E:\TestCopy";
        static void Main(string[] args)
        {
            if (!MatchDirectoryContents(sourcePath, destPath))
            {
                Console.WriteLine("Copying the files");
                DirectoryCopy(sourcePath, destPath, true);
                Console.WriteLine("All files copied");
            }
            else
                Console.WriteLine("Directory Contents are same");
            
            Console.ReadLine();
        }

        public static bool MatchDirectoryContents(string sourcePath, string destinationPath)
        {
            FileCompare fileCompare = new FileCompare();

            DirectoryInfo srcDir = new DirectoryInfo(sourcePath);
            DirectoryInfo destDir = new DirectoryInfo(destinationPath);
            FileInfo[] lstSrcFiles = srcDir.GetFiles();
            FileInfo[] lstDestFiles = destDir.GetFiles();
            DirectoryInfo[] srcSubDirs = srcDir.GetDirectories();
            DirectoryInfo[] destSubDirs = destDir.GetDirectories();

            if (fileCompare.Equals(lstSrcFiles, lstDestFiles))
            {
                if (srcSubDirs.Count() == 0 && destSubDirs.Count() == 0)
                    return true;
                else if (srcSubDirs.Count() == destSubDirs.Count())
                {
                    bool result = false;
                    for (int i = 0; i < srcSubDirs.Count(); i++)
                    {
                        result = MatchDirectoryContents(srcSubDirs[i].FullName, destSubDirs[i].FullName);
                    }
                    return result;
                }
                else return false;
            }
            else return false;
        }

        public static void DirectoryCopy(string sourcePath, string destinationPath, bool copySubDirs)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
            FileInfo[] sourceFiles = sourceDir.GetFiles(); // fetch the info of all files in the source. this method has similar overloads as Directory.GetFiles();
            DirectoryInfo[] sourceSubDirs = sourceDir.GetDirectories(); // fetch the info of all subdirectories in the top folder only, this does not fetch sub directories of sub directories. For this, use recursion.

            // create destination folder if it does not exist
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            // iterates over all files in  source path. 
            foreach (FileInfo file in sourceFiles)
            {
                string path = Path.Combine(destinationPath, file.Name); // concatenates dest path + file name
                file.CopyTo(path, true);    // copies the contents of source file in destination file, with overwriting.
            }

            if (copySubDirs)
            {
                // iterates over sub directories in source path, uses recursion
                foreach (DirectoryInfo subDir in sourceSubDirs)
                {
                    string tempPath = Path.Combine(destinationPath, subDir.Name);   // concatenates dest path + sub dir name
                    DirectoryCopy(subDir.FullName, tempPath, true);     // recursively calls function
                }
            }
        }

        public class FileCompare : System.Collections.Generic.IEqualityComparer<FileInfo[]>
        {
            public FileCompare() { }

            public bool Equals(FileInfo[] lstSrc, FileInfo[] lstDest)
            {
                bool result = false;

                if (lstSrc.Count() == lstDest.Count())
                {
                    for (int i = 0; i < lstSrc.Count(); i++)
                        result = (lstSrc[i].Name == lstDest[i].Name && lstSrc[i].Length == lstDest[i].Length) ? true : false;
                }
                else result = false;

                return result;
            }

            public int GetHashCode(FileInfo[] f1)
            {
                return 0;
            }
        }
    }
}

// References:
// https://stackoverflow.com/questions/3146586/directory-vs-directoryinfo
// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-compare-the-contents-of-two-folders-linq

