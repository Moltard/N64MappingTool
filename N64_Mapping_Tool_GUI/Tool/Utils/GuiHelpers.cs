using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.Utils;
using N64Mapping.Tool;

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
        Image
    }
    
    class GuiHelper
    {

        private const string FilterWrl = "VRML files (*.wrl)|*.wrl";
        private const string FilterObj = "Wavefront obj (*.obj)|*.obj";
        private const string FilterMtl = "Wavefront mtl (*.mtl)|*.mtl";
        private const string FilterSmd = "Studiomdl Data (*.smd)|*.smd";
        private const string FilterBmp = "Bitmap (*.bmp) | *.bmp";
        private const string FilterPng = "Portable Network Graphics (*.png) | *.png";
        private const string FilterJpeg = "Jpeg files (*.jpg, *.jpeg, *.jpe, *.jfif) | *.jpg; *.jpeg; *.jpe; *.jfif";
        private const string FilterImage = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.bmp, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.bmp; *.png";

        private const string WrlToObjName = "output";
        private const string DeleteMaterialName = "untextured";
        private const string DeleteUnusedMaterialName = "deleted_texture";
        private const string AddMaterialName = "textured";
        private const string ModifyObjName = "modified";
        private const string MergedObjName = "merged";
        private const string ExportedSmdName = "exported";
        private const string DefaultName = "default";

        private const string FolderSelection = "[Folder Selection]";

        //public const string ExtensionPatternObj = "*.obj";

        public const string ExtensionWrl = ".wrl";
        public const string ExtensionObj = ".obj";
        public const string ExtensionSmd = ".smd";
        public const string ExtensionPng = ".png";
        public static readonly string[] ExtensionImages = new string[6] { ".jpeg", ".jpg", ".jpe", ".jfif", ".bmp", ".png" };


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
                case FileFilters.Image:
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
                    return string.Empty;
                default:
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// Return the filename depending of the current action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string GetFileNameAction(ActionN64 action)
        {
            switch (action)
            {
                case ActionN64.WrlConversion:
                    return WrlToObjName;
                case ActionN64.DeleteMaterials:
                    return DeleteMaterialName;
                case ActionN64.AddMaterials:
                    return AddMaterialName;
                case ActionN64.ModifyObj:
                    return WrlToObjName;
                default:
                    return DefaultName;
            }
        }
        
        /// <summary>
        /// Open the File Browser Dialog and return the path of the selected file, null if none
        /// </summary>
        /// <returns></returns>
        public static string OpenFileDialog()
        {
            return OpenFileDialog(string.Empty);
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
        /// <returns></returns>
        public static string OpenFolderDialog()
        {
            Microsoft.Win32.FileDialog folderDialog = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = false,
                FileName = FolderSelection // Default name
            };
            bool? result = folderDialog.ShowDialog();
            if (result == true)
                return FileHelper.GetDirectoryName(folderDialog.FileName);

            return null;
        }

        /// <summary>
        /// Open the Save File Browser Dialog with presets settings
        /// </summary>
        /// <param name="action">Settings to use</param>
        /// <param name="directoryOutput">Intitial directory</param>
        /// <returns>Return the path of the file to save, null if none</returns>
        public static string SaveFileDialog(ActionN64 action, string directoryOutput = null)
        {
            switch (action)
            {
                case ActionN64.WrlConversion:
                    return SaveFileDialog(WrlToObjName, ExtensionObj, FilterObj, directoryOutput);
                case ActionN64.DeleteMaterials:
                    return SaveFileDialog(DeleteMaterialName, ExtensionObj, FilterObj, directoryOutput);
                case ActionN64.DeleteUnusedMaterials:
                    return SaveFileDialog(DeleteUnusedMaterialName, ExtensionObj, FilterObj, directoryOutput);
                case ActionN64.AddMaterials:
                    return SaveFileDialog(AddMaterialName, ExtensionObj, FilterObj, directoryOutput);
                case ActionN64.ModifyObj:
                    return SaveFileDialog(ModifyObjName, ExtensionObj, FilterObj, directoryOutput);
                case ActionN64.MergeSpecObjFiles:
                case ActionN64.MergeObjFiles:
                    return SaveFileDialog(MergedObjName, ExtensionObj, FilterObj, directoryOutput);
                case ActionN64.ObjToSmd:
                    return SaveFileDialog(ExportedSmdName, ExtensionSmd, FilterSmd, directoryOutput);
                default:
                    return SaveFileDialog(string.Empty, string.Empty, string.Empty, directoryOutput);
            }
        }

        /// <summary>
        /// Open the Save File Browser Dialog with the given settings
        /// </summary>
        /// <param name="fileName">Default filename</param>
        /// <param name="defaultExt">Default extension</param>
        /// <param name="filter">Files to filter</param>
        /// <param name="directoryOutput">Intitial directory</param>
        /// <returns>Return the path of the file to save, null if none</returns>
        public static string SaveFileDialog(string fileName, string defaultExt, FileFilters filter, string directoryOutput = null)
        {
            return SaveFileDialog(fileName, defaultExt, GetFileFilter(filter), directoryOutput);
        }

        /// <summary>
        /// Open the Save File Browser Dialog with the given settings
        /// </summary>
        /// <param name="fileName">Default filename</param>
        /// <param name="defaultExt">Default extension</param>
        /// <param name="filter">Files to filter</param>
        /// <param name="directoryOutput">Intitial directory</param>
        /// <returns>Return the path of the file to save, null if none</returns>
        public static string SaveFileDialog(string fileName, string defaultExt, string filter, string directoryOutput)
        {
            if (directoryOutput == null)
            {
                directoryOutput = string.Empty;
            }
            Microsoft.Win32.FileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter,
                FileName = fileName,
                DefaultExt = defaultExt,
                InitialDirectory = directoryOutput
            };

            bool? result = saveDialog.ShowDialog();
            if (result == true)
                return saveDialog.FileName; 

            return null;
        }

    }
}
