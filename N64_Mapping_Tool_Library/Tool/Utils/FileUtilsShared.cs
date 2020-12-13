using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Utils
{

    /// <summary>
    /// Utils Functions related to Files
    /// </summary>
    public static class FileUtilsShared
    {

        /// <summary>
        /// Try to read a file at a given path
        /// </summary>
        /// <param name="fileName">The file to delete</param>
        /// <returns>Return every line in an array if successful, null otherwise</returns>
        public static string[] TryReadAllLines(string fileName)
        {
            try
            {
                return File.ReadAllLines(fileName); // The \n are removed directly 
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Try to create a directory at a given path
        /// </summary>
        /// <param name="directory">The directory to create</param>
        /// <returns>Return true if successful, false otherwise</returns>
        public static bool TryCreateDirectory(string directory)
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Try to delete a given file
        /// </summary>
        /// <param name="fileName">The file to delete</param>
        /// <returns>Return true if successful, false otherwise</returns>
        public static bool TryDeleteFile(string fileName) {
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
        /// Read the given file and return its content into a string array
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string[] ReadFile(string filename)
        {
            if (File.Exists(filename))
            {
                return TryReadAllLines(filename);
            }
            return null;
        }
        
        /// <summary>
        /// Try to copy a given file into another directory.
        /// </summary>
        /// <param name="srcFileName">The file to copy</param>
        /// <param name="destFileName">The new location</param>
        /// <returns>Return true if successful, false otherwise</returns>
        public static bool TryCopyFile(string srcFile, string destFile)
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
        
    }

}
