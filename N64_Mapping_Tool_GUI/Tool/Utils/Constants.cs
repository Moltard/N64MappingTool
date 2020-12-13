using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Application.Tool.Utils
{

    enum ActionN64
    {
        WrlConversion,
        WrlsConversion,
        DeleteMaterials,
        DeleteUnusedMaterials,
        AddMaterials,
        ModifyObj,
        MergeObjFilesDirectory,
        MergeObjFilesList,
        CopyObjTextures,
        ObjToSmd,
        RefModelSmd,
        FlipTexture,
        MirrorTexture,
        MakeTransparent,
        CopyBlacklistTextures,
        NegativePicture,
    }

    static class FileConstants
    {

        #region Constants

        public const string ExtensionWrl = ".wrl";
        public const string ExtensionObj = ".obj";
        public const string ExtensionSmd = ".smd";
        public const string ExtensionPng = ".png";
        public static readonly string[] ExtensionImages = new string[6] { ".jpeg", ".jpg", ".jpe", ".jfif", ".bmp", ".png" };

        public const string FileNameWrlToObj = "output";
        public const string FileNameDeleteMaterials = "no_material";
        public const string FileNameDeleteUnusedMaterials = "deleted_material";
        public const string FileNameAddMaterial = "added_material";
        public const string FileNameModifyObj = "modified";
        public const string FileNameMergedObj = "merged";
        public const string FileNameExportedSmd = "exported";
        public const string FileNameReferenceSmd = "reference";
        public const string FileNameDefault = "default";
        public const string FileNameTexture = "texture";

        public const string FolderSameSelection = "//";
        
        public const string PatternExtensionWrl = "*.wrl";
        public const string PatternExtensionObj = "*.obj";
        public const string PatternAnyFile = ".*";
        
        #endregion

    }

    static class DictConstants
    {

        #region Constants

        public const string WrlFile = "WrlFile";
        public const string WrlDirectory = "WrlDirectory";
        public const string ObjFile = "ObjFile";
        public const string ObjOutput = "ObjOutput";
        public const string ObjDirectory = "ObjDirectory";
        public const string SmdOutput = "SmdOutput";
        public const string TextureDir = "TextureDir";
        public const string PictureFile = "PictureFile";
        public const string PictureOutput = "PictureOutput";
        public const string AlphaFile = "AlphaFile";
        public const string TextureFile = "TextureFile";
        public const string OutputDirectory = "OutputDirectory";

        public const string ScaleValue = "ScaleValue";
        public const string ScaleValueX = "ScaleValueX";
        public const string ScaleValueY = "ScaleValueY";
        public const string ScaleValueZ = "ScaleValueZ";
        public const string RotateValueX = "RotateValueX";
        public const string RotateValueY = "RotateValueY";
        public const string RotateValueZ = "RotateValueZ";

        #endregion

    }

    static class MessageBoxConstants
    {

        #region Constants

        public const string MessageTitle = "N64 Mapping Tool";

        public const string MessageSaveSettingsOK = "Saved settings to settings.xml";
        public const string MessageSaveSettingsFailure = "Error trying to save settings";

        public const string MessageFieldEmpty = "Field is empty";
        public const string MessageFileNotFound = "File not found";
        public const string MessageDirectoryNotFound = "Directory not found";
        public const string MessageFileNotExtension = "File is not ";

        public const string MessageObjMergeNone = "No obj selected";
        public const string MessageObjMergeMinimum = "Need at least 2 obj files to merge";

        public const string MessagePictureNotSameSize = "Pictures are not the same size";
        public const string MessageBlackListConfigNone = "No config selected";
        public const string MessageBlackListConfigError = "Error getting the config data";
        public const string MessageBlackListBoxNoSelection = "No texture selected";

        public const string MessageWrlConvertRecursive = "This will convert every .wrl files in the selected directory and subdirectories.\n" +
            "<fileName>.wrl will become <fileName>.obj.\nDo you confirm ?";
        public const string MessageCopyTextures = "This will copy every textures used by the obj file into the Output Directory.\nDo you confirm ?";
        public const string MessageBlackListCopyTextures = "This will copy every textures of the selected Config into the Output Directory.\nDo you confirm ?";
     
        public const string MessageSuccessExecution = "Success";
        public const string MessageErrorExecution = "Error during the execution";

        public const string MessageErrorDeleteUnusedMaterials = "Couldn't delete unused materials: ";
        public const string MessageErrorDeleteBlackList = "Couldn't delete blacklisted textures: ";
        public const string MessageErrorMergeGroups = "Couldn't merge groups: ";
        public const string MessageErrorScale = "Couldn't scale model: ";
        public const string MessageErrorRotate = "Couldn't rotate model: ";
        public const string MessageErrorSortGroups = "Couldn't sort groups: ";
        public const string MessageErrorSortMaterials = "Couldn't sort materials: ";
        
        public const string MessageMtlNotFound = "Mtl file not found";
        public const string MessageInvalidScaleValue = "Invalid scale value";
        public const string MessageInvalidScaleValues = "Invalid scale values";
        public const string MessageInvalidRotateValues = "Invalid rotation values";
        public const string MessageInvalidGroupName = "Invalid group name found";
        public const string MessageInvalidMaterialName = "Invalid material name found";

        public const string MessageErrorMergeObj = "Couldn't merge obj files: ";
        public const string MessageNoFilesMerge = "No files to merge";

        public const string MessageErrorCopyTextures = "Couldn't copy textures: ";

        /// <summary>
        /// Return a message indicating if a file was created or not
        /// </summary>
        /// <param name="success">If the creation was successful or not</param>
        /// <param name="fileOutput">The name of the file</param>
        /// <returns></returns>
        public static string GetMessageExecutionCreation(bool success, string fileOutput)
        {
            if (success)
                return $"Created '{fileOutput}'";
            else
                return $"Error creating '{fileOutput}'";
        }

        /// <summary>
        /// Return a message indicating a failure at parsing a file
        /// </summary>
        /// <param name="fileOutput">The name of the file</param>
        /// <returns></returns>
        public static string GetMessageExecutionErrorParse(string fileOutput)
        {
            return $"Error parsing '{fileOutput}'";
        }

        /// <summary>
        /// Return a message indicating if the execution worked or not
        /// </summary>
        /// <param name="success">If the execution was successful or not</param>
        /// <returns></returns>
        public static string GetMessageExecutionGeneric(bool success)
        {
            return success ? MessageSuccessExecution : MessageErrorExecution;
        }

        #endregion

    }

}
