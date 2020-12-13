using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Application.Tool.Utils
{

    enum FileFilters
    {
        None,
        Wrl,
        Obj,
        Mtl,
        Smd,
        Bmp,
        Png,
        Jpeg,
        Images
    }

    static class FileUtils
    {
        #region Constants

        private const string FilterAny = "All files (*.*)|*.*";
        private const string FilterWrl = "VRML files (*.wrl)|*.wrl";
        private const string FilterObj = "Wavefront obj (*.obj)|*.obj";
        private const string FilterMtl = "Wavefront mtl (*.mtl)|*.mtl";
        private const string FilterSmd = "Studiomdl Data (*.smd)|*.smd";
        private const string FilterBmp = "Bitmap (*.bmp) | *.bmp";
        private const string FilterPng = "Portable Network Graphics (*.png) | *.png";
        private const string FilterJpeg = "Jpeg files (*.jpg, *.jpeg, *.jpe, *.jfif) | *.jpg; *.jpeg; *.jpe; *.jfif";
        private const string FilterImage = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.bmp, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.bmp; *.png";

        private const string FolderSelection = "[Folder Selection]";

        #endregion

        #region Methods - Files / Folders




        /// <summary>
        /// Return true if the file has the given extension
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsFileExtension(string filename, string extension)
        {
            return extension == Path.GetExtension(filename);
        }

        /// <summary>
        /// Return true if the file has one of the given extensions
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static bool IsFileExtension(string filename, string[] extensions)
        {
            return extensions.Contains(Path.GetExtension(filename));
        }


        /// <summary>
        /// Return the list of files located in the given directory. null if error
        /// </summary>
        /// <param name="dir">Directory</param>
        /// <returns>List of files or null</returns>
        public static List<string> GetFiles(string dir)
        {
            return GetFiles(dir, FileConstants.PatternAnyFile);
        }

        /// <summary>
        /// Return the list of files located in the given directory, following a specific pattern. null if error
        /// </summary>
        /// <param name="dir">Directory</param>
        /// <returns>List of files or null</returns>
        public static List<string> GetFiles(string dir, string pattern)
        {
            return GetFiles(dir, pattern, false);
        }
        
        /// <summary>
        /// Return the list of files located in the given directory and possibly subdirectories, following a specific pattern.
        /// </summary>
        /// <param name="directory">Directory</param>
        /// <param name="pattern">Search pattern</param>
        /// <param name="allDirectories">Include the subdirectories</param>
        /// <returns>List of files or null</returns>
        public static List<string> GetFiles(string directory, string pattern, bool allDirectories)
        {
            if (Directory.Exists(directory))
            {
                SearchOption searchOption = allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                try
                {
                    return new List<string>(Directory.GetFiles(directory, pattern, searchOption));
                }
                catch { }
            }
            return null;
        }


        /// <summary>
        /// Copy a list of files into another directory
        /// </summary>
        /// <param name="outputFolder"></param>
        /// <param name="filesList"></param>
        /// <returns></returns>
        public static bool CopyFiles(string outputFolder, List<string> filesList)
        {
            if (!Directory.Exists(outputFolder))
                if (!N64Library.Tool.Utils.FileUtilsShared.TryCreateDirectory(outputFolder)) // Impossible to create the destination directory
                    return false;

            foreach (string srcFile in filesList)
            {
                string destFile = Path.Combine(outputFolder, System.IO.Path.GetFileName(srcFile));

                if (!destFile.Equals(srcFile)) // Destination file is not the Source file
                {
                    if (File.Exists(srcFile))
                    {
                        if (File.Exists(destFile)) // The destination file already exists, we need to delete it
                        {
                            if (!N64Library.Tool.Utils.FileUtilsShared.TryDeleteFile(destFile))
                                continue; // Impossible to delete the file, we go to the next file
                        }
                        N64Library.Tool.Utils.FileUtilsShared.TryCopyFile(srcFile, destFile);
                    }
                }
            }
            return true;
        }

        #endregion

        #region Methods - Dialog File / Folder 

        /// <summary>
        /// Return the file filter depending of the given value
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string GetFileFilter(FileFilters filter)
        {
            switch (filter)
            {
                case FileFilters.Bmp:
                    return FilterBmp;
                case FileFilters.Images:
                    return FilterImage;
                case FileFilters.Jpeg:
                    return FilterJpeg;
                case FileFilters.Mtl:
                    return FilterMtl;
                case FileFilters.Obj:
                    return FilterObj;
                case FileFilters.Png:
                    return FilterPng;
                case FileFilters.Smd:
                    return FilterSmd;
                case FileFilters.Wrl:
                    return FilterWrl;
                case FileFilters.None:
                default:
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// Open the File Browser Dialog and return the path of the selected file, null if none
        /// </summary>
        /// <param name="filter">Filter for the file selection</param>
        /// <returns></returns>
        public static string OpenFileDialog(FileFilters filter)
        {
            return OpenFileDialog(GetFileFilter(filter));
        }

        private static string OpenFileDialog(string filter)
        {
            Microsoft.Win32.FileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter
            };
            bool? result = fileDialog.ShowDialog();
            if (result == true)
                return fileDialog.FileName;
            return null;
        }

        /// <summary>
        /// Open the File Browser Dialog and return the list of selected files, null if none
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string[] OpenFilesDialog(FileFilters filter)
        {
            return OpenFilesDialog(GetFileFilter(filter));
        }

        private static string[] OpenFilesDialog(string filter)
        {
            Microsoft.Win32.FileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter,
                Multiselect = true
            };
            bool? result = fileDialog.ShowDialog();
            if (result == true)
                return fileDialog.FileNames;
            return null;
        }

        /// <summary>
        /// Open the File Browser Dialog and return the path of the selected folder, null if none
        /// </summary>
        /// <returns>Returns the selected folder</returns>
        public static string OpenDirectoryDialog()
        {
            Microsoft.Win32.FileDialog folderDialog = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = false,
                FileName = FolderSelection // Default name
            };
            bool? result = folderDialog.ShowDialog();
            if (result == true)
                return Path.GetDirectoryName(folderDialog.FileName);

            return null;
        }

        /// <summary>
        /// Open the Save File Browser Dialog with presets settings
        /// </summary>
        /// <param name="action">Settings to use</param>
        /// <param name="initialDirectory">Intitial directory</param>
        /// <returns>Return the path of the file to save, null if none</returns>
        public static string SaveFileDialogPreset(ActionN64 action, string initialDirectory = null)
        {
            switch (action)
            {
                case ActionN64.WrlConversion:
                    return SaveFileDialog(FileConstants.FileNameWrlToObj, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.DeleteMaterials:
                    return SaveFileDialog(FileConstants.FileNameDeleteMaterials, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.DeleteUnusedMaterials:
                    return SaveFileDialog(FileConstants.FileNameDeleteUnusedMaterials, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.AddMaterials:
                    return SaveFileDialog(FileConstants.FileNameAddMaterial, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.ModifyObj:
                    return SaveFileDialog(FileConstants.FileNameModifyObj, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.MergeObjFilesList:
                case ActionN64.MergeObjFilesDirectory:
                    return SaveFileDialog(FileConstants.FileNameMergedObj, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.ObjToSmd:
                    return SaveFileDialog(FileConstants.FileNameExportedSmd, FileConstants.ExtensionSmd, FilterSmd, initialDirectory);
                case ActionN64.RefModelSmd:
                    return SaveFileDialog(FileConstants.FileNameReferenceSmd, FileConstants.ExtensionSmd, FilterSmd, initialDirectory);
                case ActionN64.NegativePicture:
                case ActionN64.FlipTexture:
                case ActionN64.MirrorTexture:
                case ActionN64.MakeTransparent:
                    return SaveFileDialog(FileConstants.FileNameTexture, FileConstants.ExtensionPng, FilterPng, initialDirectory);
                default:
                    return SaveFileDialog(string.Empty, string.Empty, FilterAny, initialDirectory);
            }
        }

        /// <summary>
        /// Open the Save File Browser Dialog with presets settings and a specific filename
        /// </summary>
        /// <param name="action">Settings to use</param>
        /// <param name="fileName">Default filename</param>
        /// <param name="initialDirectory">Intitial directory</param>
        /// <returns>Return the path of the file to save, null if none</returns>
        public static string SaveFileDialogPresetFileName(ActionN64 action, string fileName, string initialDirectory = null)
        {
            switch (action)
            {
                case ActionN64.WrlConversion:
                    return SaveFileDialog(fileName, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.DeleteMaterials:
                    return SaveFileDialog(fileName, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.DeleteUnusedMaterials:
                    return SaveFileDialog(fileName, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.AddMaterials:
                    return SaveFileDialog(fileName, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.ModifyObj:
                    return SaveFileDialog(fileName, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.MergeObjFilesList:
                case ActionN64.MergeObjFilesDirectory:
                    return SaveFileDialog(fileName, FileConstants.ExtensionObj, FilterObj, initialDirectory);
                case ActionN64.ObjToSmd:
                    return SaveFileDialog(fileName, FileConstants.ExtensionSmd, FilterSmd, initialDirectory);
                case ActionN64.RefModelSmd:
                    return SaveFileDialog(fileName, FileConstants.ExtensionSmd, FilterSmd, initialDirectory);
                case ActionN64.NegativePicture:
                case ActionN64.FlipTexture:
                case ActionN64.MirrorTexture:
                case ActionN64.MakeTransparent:
                    return SaveFileDialog(fileName, FileConstants.ExtensionPng, FilterPng, initialDirectory);
                default:
                    return SaveFileDialog(string.Empty, string.Empty, FilterAny, initialDirectory);
            }
        }
        
        /// <summary>
        /// Open the Save File Browser Dialog with the given settings
        /// </summary>
        /// <param name="fileName">Default filename</param>
        /// <param name="defaultExt">Default extension</param>
        /// <param name="filter">Files to filter</param>
        /// <param name="initialDirectory">Intitial directory</param>
        /// <returns>Return the path of the file to save, null if none</returns>
        public static string SaveFileDialog(string fileName, string defaultExt, string filter, string initialDirectory)
        {
            if (initialDirectory == null)
            {
                initialDirectory = string.Empty;
            }
            Microsoft.Win32.FileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = initialDirectory,
                Filter = filter,
                FileName = fileName,
                DefaultExt = defaultExt,
            };

            bool? result = saveDialog.ShowDialog();
            if (result == true)
                return saveDialog.FileName;

            return null;
        }
        
        #endregion

    }
}
