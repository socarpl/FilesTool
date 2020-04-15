using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileTools
{
    class Program
    {
        public 
        static void Main(string[] args)
        {
            MoveAllFoldersToParentInDir(@"\\192.168.0.199\Share\Movies\MrRobot Season 3");
        }

        public enum MoveConflictStrategy { LeaveInFolder, OverwriteInParent };

        public static void MoveFolderToParent(string folderpath,
            bool removeChildFoldersOnCompletion = true,
            MoveConflictStrategy strategy = MoveConflictStrategy.LeaveInFolder,
            bool forceRemoveChildFolder = false)
        {
            if(Directory.Exists(folderpath))
            {
                var dir = new DirectoryInfo(folderpath);
                var parentDir = Path.Combine(dir.FullName, ".."); //this seems to be a little hack. To be changed in future.

                foreach (var file in dir.GetFiles())
                {
                    string newFilePath = Path.Combine(parentDir, file.Name);
                    try
                    {
                        File.Move(file.FullName, newFilePath, strategy == MoveConflictStrategy.OverwriteInParent);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }

                int filesremaining = dir.GetFiles().Length;

                if((removeChildFoldersOnCompletion == true && filesremaining==0) || forceRemoveChildFolder)
                {
                    Directory.Delete(folderpath);
                }
            }
        }

        public static void MoveAllFoldersToParentInDir(string folderWithFoldersPath, 
            bool removeChildFoldersOnCompletion = true, 
            MoveConflictStrategy strategy = MoveConflictStrategy.LeaveInFolder)
        {
            if(Directory.Exists(folderWithFoldersPath))
            {
                DirectoryInfo di = new DirectoryInfo(folderWithFoldersPath);
                var dirs = di.GetDirectories();
                foreach(var dir in dirs)
                {
                    MoveFolderToParent(dir.FullName);
                }
            }
        }

        public static void CreateFolderForEachFile(List<string> files, bool moveFilesToThatFolder=false, bool overwriteOnMove=false)
        {
            foreach(var filepath in files)
            {
                FileInfo file = new FileInfo(filepath);
                string filename = file.Name;
                filename = filename.Substring(0, filename.Length - file.Extension.Length);
                if(!string.IsNullOrEmpty(filename))
                {
                    string newDir = Path.Combine(file.DirectoryName, filename);
                    try
                    {
                        if (!Directory.Exists(newDir))
                            Directory.CreateDirectory(newDir);

                        if (moveFilesToThatFolder)
                            file.MoveTo(Path.Combine(newDir, file.Name), overwriteOnMove);
                    }
                    catch(Exception ex)
                    {

                    }
                }

                //if(Directory.Exists(file))
            }
        }
    }
}
