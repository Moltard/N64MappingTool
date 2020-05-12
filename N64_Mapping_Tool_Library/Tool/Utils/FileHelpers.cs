using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Utils
{
    public class FileHelper
    {

        /// <summary>
        /// Return true if the file has the given extension
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsFileExtension(string filename, string extension) {
            return extension == GetExtension(filename);
        }

        /// <summary>
        /// Return true if the file has one of the given extensions
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static bool IsFileExtension(string filename, string[] extensions) {
            return extensions.Contains(GetExtension(filename));
        }

        /// <summary>
        /// Return true is absolute path, false otherwise
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsPathRooted(string path) { return Path.IsPathRooted(path); }

        /// <summary>
        /// Return the name of the directory of a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string path) { return Path.GetDirectoryName(path); }

        /// <summary>
        /// Return the extension of a given file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetExtension(string path) { return Path.GetExtension(path); }

        /// <summary>
        /// Return the name of a given file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path) { return Path.GetFileName(path); }

        /// <summary>
        /// Return the name of a given file without its extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string path) { return Path.GetFileNameWithoutExtension(path); }

        /// <summary>
        /// Combine 2 path into one
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string Combine(string path1, string path2) { return Path.Combine(path1, path2); }


        /// <summary>
        /// Delete a given file. Return true if successful, false otherwise
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool DeleteFile(string fileName) {
            try
            {
                File.Delete(fileName);
            }
            catch
            {
                return false; // If impossible to delete
            }
            return true;
        }

        /// <summary>
        /// Read a given file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Return every line in an array if successful, null otherwise</returns>
        public static string[] ReadAllLines(string filename)
        {
            try
            {
                return File.ReadAllLines(filename); // The \n are removed directly 
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create a directory
        /// </summary>
        /// <param name="dir">The directory</param>
        /// <returns>Return true if successful, false otherwise</returns>
        public static bool CreateDirectory(string dir) {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Copy a list of files into another directory
        /// </summary>
        /// <param name="outputFolder"></param>
        /// <param name="texturesList"></param>
        /// <returns></returns>
        public static bool CopyFiles(string outputFolder, List<string> texturesList)
        {
            if (!System.IO.Directory.Exists(outputFolder))
                if (!FileHelper.CreateDirectory(outputFolder)) // Impossible to create the destination directory
                    return false;
            
            foreach (string srcFile in texturesList)
            {
                string destFile = FileHelper.Combine(outputFolder, FileHelper.GetFileName(srcFile));
   
                if (!destFile.Equals(srcFile)) // Destination file is not the Source file
                {
                    if (System.IO.File.Exists(srcFile))
                    {
                        if (System.IO.File.Exists(destFile)) // The destination file already exists, we need to delete it
                        {
                            if (!FileHelper.DeleteFile(destFile))
                                continue; // Impossible to delete the file, we go to the next file
                        }
                        FileHelper.CopyFile(srcFile, destFile);
                    }
                }
               
            }
            return true;
        }

        /// <summary>
        /// Copy a given file into another directory. 
        /// </summary>
        /// <param name="srcFileName"></param>
        /// <param name="destFileName"></param>
        /// <returns>Return true if successful, false otherwise</returns>
        public static bool CopyFile(string srcFile, string destFile)
        {
            try
            {
                File.Copy(srcFile, destFile);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Return the list of files located in the given directory. null if error
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string dir)
        {
            return GetFiles(dir, null);
        }

        /// <summary>
        /// Return the list of files located in the given directory, following a specific pattern. null if error
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string dir, string ext)
        {
            if (System.IO.Directory.Exists(dir))
            {
                try
                {
                    string[] files;
                    if (ext == null)
                        files = Directory.GetFiles(dir);
                    else
                        files = Directory.GetFiles(dir, ext);
                    return new List<string>(files);
                }
                catch { } 
            }
            return null;
        }




    }
}
