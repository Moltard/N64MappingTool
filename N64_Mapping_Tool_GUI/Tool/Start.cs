using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Application.Tool.Utils;
using N64Library.Tool.Exporter;
using N64Library.Tool.Data;
using N64Library.Tool.Modifier;
using N64Library.Tool.Parser;
using N64Library.Tool.Utils;

namespace N64Application.Tool
{

    [Flags]
    enum ObjOptions
    {
        ReverseVertex = 1,
        Merge = 2,
        Sort = 4,
        UniformScale = 8,
        NonUniformScale = 16,
        Rotate = 32,
        BlackList = 64,
        SmdUseTextureName = 128
    }

    [Flags]
    enum PictureOptions
    {
        FlipHorizontally = 1,
        FlipVertically = 2,
        MirrorHorizontally = 4,
        MirrorVertically = 8
    }
    
    static class Start
    {
        /// <summary>
        /// Used to store a list of path to texture files
        /// </summary>
        internal static List<string> texturesList = new List<string>();

        /// <summary>
        /// Used to store a list of path to obj files
        /// </summary>
        internal static List<string> objFileList = new List<string>();

        public static ToolResult ToolObj(ActionN64 action, Dictionary<string, string> inputs)
        {
            return ToolObj(action, inputs, 0);
        }

        public static ToolResult ToolObj(ActionN64 action, Dictionary<string,string> inputs, ObjOptions objOptions)
        {
            string outputFile = null;
            switch (action)
            {
                case ActionN64.WrlConversion: // Convert Wrl to Obj
                    if (inputs.TryGetValue(DictConstants.WrlFile, out string wrlFile))
                    {
                        if (inputs.TryGetValue(DictConstants.ObjOutput, out outputFile))
                        {
                            return ConvertWrlToObj(wrlFile, outputFile, objOptions);
                        }
                    }
                    break;
                case ActionN64.WrlsConversion: // Convert Wrl files to Obj
                    if (inputs.TryGetValue(DictConstants.WrlDirectory, out string wrlDirectory))
                    {
                        return ConvertWrlsToObj(wrlDirectory, objOptions);
                    }
                    break;
                    
                case ActionN64.DeleteMaterials: // Delete Obj Materials
                    if (inputs.TryGetValue(DictConstants.ObjFile, out string objFile))
                    {
                        if (inputs.TryGetValue(DictConstants.ObjOutput, out outputFile))
                        {
                            return DeleteMaterialsObj(objFile, outputFile);
                        }
                    }
                    break;
                case ActionN64.DeleteUnusedMaterials: // Delete Unused Obj Materials
                    if (inputs.TryGetValue(DictConstants.ObjFile, out objFile))
                    {
                        if (inputs.TryGetValue(DictConstants.ObjOutput, out outputFile))
                        {
                            return DeleteUnusedMaterialsObj(objFile, outputFile);
                        }
                    }
                    break;

                case ActionN64.AddMaterials: // Add Obj Materials
                    if (inputs.TryGetValue(DictConstants.ObjFile, out objFile))
                    {
                        if (inputs.TryGetValue(DictConstants.ObjOutput, out outputFile))
                        {
                            return AddMaterialsObj(objFile, outputFile);
                        }
                    }
                    break;
                case ActionN64.ModifyObj: // Modify Obj
                    if (inputs.TryGetValue(DictConstants.ObjFile, out objFile))
                    {
                        if (inputs.TryGetValue(DictConstants.ObjOutput, out outputFile))
                        {
                            return ModifyObj(objFile, outputFile, objOptions, inputs);
                        }
                    }
                    break;
                case ActionN64.ObjToSmd: // Obj to Smd
                    if (inputs.TryGetValue(DictConstants.ObjFile, out objFile))
                    {
                        if (inputs.TryGetValue(DictConstants.SmdOutput, out outputFile))
                        {
                            bool useTextureName = objOptions.HasFlag(ObjOptions.SmdUseTextureName);
                            return ConvertObjToSmd(objFile, outputFile, useTextureName);
                        }
                    }
                    break;
                case ActionN64.RefModelSmd: // Reference Model
                    if (inputs.TryGetValue(DictConstants.ObjFile, out objFile))
                    {
                        if (inputs.TryGetValue(DictConstants.SmdOutput, out outputFile))
                        {
                            return ConvertRefModelSmd(objFile, outputFile, inputs);
                        }
                    }
                    break;
                case ActionN64.MergeObjFilesDirectory: // Merge Obj Files in a Directory
                    if (inputs.TryGetValue(DictConstants.ObjDirectory, out string objDirectory))
                    {
                        if (inputs.TryGetValue(DictConstants.ObjOutput, out outputFile))
                        {
                            return MergeObjFiles(objDirectory, outputFile);
                        }
                    }
                    break;
                case ActionN64.MergeObjFilesList: // Merge Obj Files in a List
                    if (inputs.TryGetValue(DictConstants.ObjOutput, out outputFile))
                    {
                        return MergeObjFiles(objFileList, outputFile);
                    }
                    break;
            }
            return new ToolResult(ToolResultEnum.DefaultError);
        }

        public static ToolResult ToolPictures(ActionN64 action, Dictionary<string, string> inputs)
        {
            return ToolPictures(action, inputs, 0);
        }

        public static ToolResult ToolPictures(ActionN64 action, Dictionary<string, string> inputs, PictureOptions pictureOptions)
        {
            switch (action)
            {
                case ActionN64.CopyObjTextures: // Copy Obj Textures
                    if (inputs.TryGetValue(DictConstants.ObjFile, out string objFile))
                    {
                        if (inputs.TryGetValue(DictConstants.OutputDirectory, out string destDir))
                        {
                            if (inputs.TryGetValue(DictConstants.TextureDir, out string textureDir))
                            {
                                return CopyUsedTexturesObj(objFile, destDir, textureDir);
                            }
                        }
                    }
                    break;

                case ActionN64.FlipTexture: // Flip a picture
                    if (inputs.TryGetValue(DictConstants.PictureFile, out string pictureFile))
                    {
                        if (inputs.TryGetValue(DictConstants.PictureOutput, out string outputFile))
                        {
                            bool horizontal = pictureOptions.HasFlag(PictureOptions.FlipHorizontally);
                            bool vertical = pictureOptions.HasFlag(PictureOptions.FlipVertically);
                            return FlipTexture(pictureFile, outputFile, horizontal, vertical);
                        }
                    }
                    break;

                case ActionN64.MirrorTexture: // Mirror a picture
                    if (inputs.TryGetValue(DictConstants.PictureFile, out pictureFile))
                    {
                        if (inputs.TryGetValue(DictConstants.PictureOutput, out string outputFile))
                        {
                            bool horizontal = pictureOptions.HasFlag(PictureOptions.MirrorHorizontally);
                            bool vertical = pictureOptions.HasFlag(PictureOptions.MirrorVertically);
                            return MirrorTexture(pictureFile, outputFile, horizontal, vertical);
                        }
                    }
                    break;

                case ActionN64.MakeTransparent: // Create a transparent picture
                    if (inputs.TryGetValue(DictConstants.AlphaFile, out string alphaFile))
                    {
                        if (inputs.TryGetValue(DictConstants.TextureFile, out string textureFile))
                        {
                            if (inputs.TryGetValue(DictConstants.PictureOutput, out string outputFile))
                            {
                                return MakeTransparent(alphaFile, textureFile, outputFile);
                            }
                        }
                    }
                    break;

                case ActionN64.NegativePicture: // Create a negative picture
                    if (inputs.TryGetValue(DictConstants.PictureFile, out pictureFile))
                    {
                        if (inputs.TryGetValue(DictConstants.PictureOutput, out string outputFile))
                        {
                            return NegativePicture(pictureFile, outputFile);
                        }
                    }
                    break;

                case ActionN64.CopyBlacklistTextures: // Copy BlackListed Textures
                    if (inputs.TryGetValue(DictConstants.OutputDirectory, out string outputDirectory))
                    {
                        return CopyTexturesBlacklist(outputDirectory);
                    }
                    break;
            }
            return new ToolResult(ToolResultEnum.DefaultError);
        }

        private static ToolResult ConvertWrlToObj(string wrlFile, string objOutput, ObjOptions objOptions)
        {
            bool success = false;
            string message = "";
            WrlData wrlData = WrlParser.TryParseWrl(wrlFile);
            if (wrlData != null)
            {
                if (objOptions.HasFlag(ObjOptions.ReverseVertex))
                {
                    WrlModifier.ReverseVertexOrder(wrlData);
                }
                success = WrlExporter.WriteObj(wrlData, objOutput);
                message = MessageBoxConstants.GetMessageExecutionCreation(success, objOutput);
            }
            else
            {
                message = MessageBoxConstants.GetMessageExecutionErrorParse(wrlFile);
            }
            return new ToolResult(message, success);
        }

        private static ToolResult ConvertWrlsToObj(string directory, ObjOptions objOptions)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            int error = 0;
            List<string> wrlList = FileUtils.GetFiles(directory, FileConstants.PatternExtensionWrl, true);
            foreach (string wrlFile in wrlList) {
                string wrlDirectory = System.IO.Path.GetDirectoryName(wrlFile);
                string wrlFilenameNoExt = System.IO.Path.GetFileNameWithoutExtension(wrlFile);
                string objOutput = System.IO.Path.Combine(wrlDirectory, wrlFilenameNoExt + ".obj");
                ToolResult result = ConvertWrlToObj(wrlFile, objOutput, objOptions);
                if (result.Success)
                    count++;
                else
                    error++;
            }
            string aa = $"error{ ((error>0) ? "" :" " )}";
            bool success = false;
            bool warn = false;
            if (error > 0)
            {
                warn = true;
                // Add a 's' to 'error' if there are more than one error
                sb.AppendLine($"{error} error{( error > 1 ? "s" : "") } converting wrl files");
            }
            if (count > 0)
            {
                success = true;
            }
            sb.Append($"Converted {count}/{wrlList.Count} wrl files located in '{directory}' and its subdirectories");
            return new ToolResult(sb.ToString(), success, warn);
        }

        private static ToolResult DeleteMaterialsObj(string objFile, string objOutput)
        {
            bool success = false;
            string message = "";
            ObjData objData = ObjParser.TryParseObj(objFile);
            if (objData != null)
            {
                ObjModifier.DeleteMaterials(objData);
                success = ObjExporter.WriteObj(objData, null, objOutput, makeMtl: false, useExistingMtl: false);
                message = MessageBoxConstants.GetMessageExecutionCreation(success, objOutput);
            }
            else
            {
                message = MessageBoxConstants.GetMessageExecutionErrorParse(objFile);
            }
            return new ToolResult(message, success);
        }

        private static ToolResult DeleteUnusedMaterialsObj(string objFile, string objOutput)
        {
            bool success = false;
            string message = "";
            ObjData objData = ObjParser.TryParseObj(objFile);
            if (objData != null)
            {
                objData.UpdateMtlData();
                MtlData mtlData = objData.Mtl;
                if (mtlData != null)
                {
                    ObjModifier.DeleteUnusedMaterials(objData, mtlData);
                    success = ObjExporter.WriteObj(objData, mtlData, objOutput, makeMtl: true, useExistingMtl: true);
                    message = MessageBoxConstants.GetMessageExecutionCreation(success, objOutput);
                }
                else
                {
                    message = MessageBoxConstants.MessageErrorDeleteUnusedMaterials + 
                        MessageBoxConstants.MessageMtlNotFound;
                }
            }
            else
            {
                message = MessageBoxConstants.GetMessageExecutionErrorParse(objFile);
            }
            return new ToolResult(message, success);
        }

        private static ToolResult AddMaterialsObj(string objFile, string objOutput)
        {
            bool success = false;
            string message = "";
            ObjData objData = ObjParser.TryParseObj(objFile);
            if (objData != null)
            {
                ObjModifier.ParseTextures(objData);
                ObjModifier.CalculateNormal(objData);
                success = ObjExporter.WriteObj(objData, null, objOutput, makeMtl: true, useExistingMtl: false);
                message = MessageBoxConstants.GetMessageExecutionCreation(success, objOutput);
            }
            else
            {
                message = MessageBoxConstants.GetMessageExecutionErrorParse(objFile);
            }
            return new ToolResult(message, success);
        }
        
        private static ToolResult ModifyObj(string objFile, string objOutput, ObjOptions objOptions, Dictionary<string, string> inputs)
        {
            bool success = false;
            bool warn = false;
            StringBuilder sb = new StringBuilder();
            ObjData objData = ObjParser.TryParseObj(objFile);
            if (objData != null)
            {
                objData.UpdateMtlData();
                MtlData mtlData = objData.Mtl;

                if (objOptions.HasFlag(ObjOptions.BlackList))
                {
                    // Delete Blacklisted Objects first
                    if (mtlData != null)
                    {
                        if (!ObjModifier.TryDeleteMatchingGroups(objData, mtlData, texturesList))
                        {
                            warn = true;
                            sb.AppendLine(MessageBoxConstants.MessageErrorDeleteBlackList + MessageBoxConstants.MessageErrorExecution);
                        }
                    }
                    else
                    {
                        warn = true;
                        sb.AppendLine(MessageBoxConstants.MessageErrorDeleteBlackList + MessageBoxConstants.MessageMtlNotFound);
                    }
                }

                ObjModifier.CalculateNormal(objData);

                if (objOptions.HasFlag(ObjOptions.UniformScale))
                {
                    if (inputs.TryGetValue(DictConstants.ScaleValue, out string scaleStr))
                    {
                        if (Double.TryParse(scaleStr, out double scaleValue))
                        {
                            ObjModifier.UniformScale(objData, scaleValue); // Scale Model
                        }
                        else
                        {
                            warn = true;
                            sb.AppendLine(MessageBoxConstants.MessageErrorScale + MessageBoxConstants.MessageInvalidScaleValue);
                        }
                    }
                }

                if (objOptions.HasFlag(ObjOptions.NonUniformScale))
                {
                    bool isScaleValueX = false;
                    bool isScaleValueY = false;
                    bool isScaleValueZ = false;
                    double scaleValueX = 0.0;
                    double scaleValueY = 0.0;
                    double scaleValueZ = 0.0;
                    if (inputs.TryGetValue(DictConstants.ScaleValueX, out string scaleStr))
                    {
                        isScaleValueX = Double.TryParse(scaleStr, out scaleValueX);
                    }
                    if (inputs.TryGetValue(DictConstants.ScaleValueY, out scaleStr))
                    {
                        isScaleValueY = Double.TryParse(scaleStr, out scaleValueY);
                    }
                    if (inputs.TryGetValue(DictConstants.ScaleValueZ, out scaleStr))
                    {
                        isScaleValueZ = Double.TryParse(scaleStr, out scaleValueZ);
                    }
                    if (isScaleValueX && isScaleValueY && isScaleValueZ)
                    {
                        // Scale Model
                        ObjModifier.NonUniformScale(objData, scaleValueX, scaleValueY, scaleValueZ);
                    }
                    else
                    {
                        warn = true;
                        sb.AppendLine(MessageBoxConstants.MessageErrorScale + MessageBoxConstants.MessageInvalidScaleValues);
                    }
                }

                if (objOptions.HasFlag(ObjOptions.Rotate))
                {
                    bool isRotateValueX = false;
                    bool isRotateValueY = false;
                    bool isRotateValueZ = false;
                    double rotateValueX = 0.0;
                    double rotateValueY = 0.0;
                    double rotateValueZ = 0.0;
                    if (inputs.TryGetValue(DictConstants.RotateValueX, out string rotationStr))
                    {
                        isRotateValueX = Double.TryParse(rotationStr, out rotateValueX);
                    }
                    if (inputs.TryGetValue(DictConstants.RotateValueY, out rotationStr))
                    {
                        isRotateValueY = Double.TryParse(rotationStr, out rotateValueY);
                    }
                    if (inputs.TryGetValue(DictConstants.RotateValueZ, out rotationStr))
                    {
                        isRotateValueZ = Double.TryParse(rotationStr, out rotateValueZ);
                    }
                    if (isRotateValueX && isRotateValueY && isRotateValueZ)
                    {
                        // Rotate Model
                        ObjModifier.RotateModel(objData, rotateValueX, rotateValueY, rotateValueZ);
                    }
                    else
                    {
                        warn = true;
                        sb.AppendLine(MessageBoxConstants.MessageErrorRotate + MessageBoxConstants.MessageInvalidRotateValues);
                    }
                }

                if (objOptions.HasFlag(ObjOptions.ReverseVertex))
                {
                    // Reverse Vertex Order
                    ObjModifier.ReverseVertexOrder(objData); 
                }

                if (objOptions.HasFlag(ObjOptions.Merge))
                {
                    if (mtlData != null)
                    {
                        // Merge groups and materials
                        ObjModifier.MergeGroups(objData, mtlData);
                    }
                    else
                    {
                        warn = true;
                        sb.AppendLine(MessageBoxConstants.MessageErrorMergeGroups + MessageBoxConstants.MessageMtlNotFound);
                    }
                }
                if (objOptions.HasFlag(ObjOptions.Sort))
                {
                    // Sort groups
                    if (!ObjModifier.SortGroups(objData))
                    {
                        warn = true;
                        sb.AppendLine(MessageBoxConstants.MessageErrorSortGroups + MessageBoxConstants.MessageInvalidGroupName);
                    }

                    if (mtlData != null)
                    {
                        // Sort materials
                        if (!ObjModifier.SortMaterials(mtlData))
                        {
                            warn = true;
                            sb.AppendLine(MessageBoxConstants.MessageErrorSortMaterials + MessageBoxConstants.MessageInvalidMaterialName);
                        }
                    }
                    else
                    {
                        warn = true;
                        sb.AppendLine(MessageBoxConstants.MessageErrorSortMaterials + MessageBoxConstants.MessageMtlNotFound);
                    }
                }
                success = ObjExporter.WriteObj(objData, mtlData, objOutput, makeMtl: true, useExistingMtl: true);
                sb.Append(MessageBoxConstants.GetMessageExecutionCreation(success, objOutput));
            }
            else
            {
                sb.Append(MessageBoxConstants.GetMessageExecutionErrorParse(objFile));
            }
            return new ToolResult(sb.ToString(), success, warn);
        }

        private static ToolResult ConvertObjToSmd(string objFile, string smdOutput, bool useTextureName)
        {
            bool success = false;
            string message = "";
            ObjData objData = ObjParser.TryParseObj(objFile);
            if (objData != null)
            {
                objData.UpdateMtlData();
                MtlData mtlData = objData.Mtl;

                if (mtlData == null)
                {
                    // No MTL means the SMD will use a default material name for all objects
                    useTextureName = false;
                }
                success = SmdExporter.WriteSmd(objData, mtlData, smdOutput, useTextureName);
                message = MessageBoxConstants.GetMessageExecutionCreation(success, smdOutput);
            }
            else
            {
                message = MessageBoxConstants.GetMessageExecutionErrorParse(objFile);
            }
            return new ToolResult(message, success);
        }

        private static ToolResult ConvertRefModelSmd(string objFile, string smdOutput, Dictionary<string, string> inputs)
        {
            bool success = false;
            bool warn = false;
            StringBuilder sb = new StringBuilder();
            ObjData objData = ObjParser.TryParseObj(objFile);
            if (objData != null)
            {
                // Delete all materials
                ObjModifier.DeleteMaterials(objData);

                // Scale the model by the given value
                if (inputs.TryGetValue(DictConstants.ScaleValue, out string scaleStr))
                {
                    if (Double.TryParse(scaleStr, out double scaleValue))
                    {
                        ObjModifier.UniformScale(objData, scaleValue); // Scale Model
                    }
                    else
                    {
                        warn = true;
                        sb.AppendLine(MessageBoxConstants.MessageErrorScale + MessageBoxConstants.MessageInvalidScaleValue);
                    }
                }

                // The SMD will use a default material name for all objects
                success = SmdExporter.WriteSmd(objData, null, smdOutput, false);
                sb.Append(MessageBoxConstants.GetMessageExecutionCreation(success, smdOutput));
            }
            else
            {
                sb.Append(MessageBoxConstants.GetMessageExecutionErrorParse(objFile));
            }
            return new ToolResult(sb.ToString(), success, warn);
        }
        
        private static ToolResult MergeObjFiles(string objDirectory, string objOutput)
        {
            return MergeObjFiles(FileUtils.GetFiles(objDirectory, FileConstants.PatternExtensionObj), objOutput);
        }
        
        private static ToolResult MergeObjFiles(List<string> fileList, string objOutput)
        {
            bool success = false;
            StringBuilder sb = new StringBuilder();
            if (fileList != null)
            {
                List<ObjData> objDatas = ObjParser.ParseObjs(fileList);
                ObjData objData = ObjModifier.MergeObjFiles(objDatas);
                if (objData != null)
                {
                    success = ObjExporter.WriteObj(objData, objData.Mtl, objOutput, makeMtl: true, useExistingMtl: true);
                    sb.AppendLine($"Merged {objDatas.Count} obj files into one");
                    sb.Append(MessageBoxConstants.GetMessageExecutionCreation(success, objOutput));
                }
                else
                {
                    sb.Append(MessageBoxConstants.MessageErrorMergeObj + MessageBoxConstants.MessageErrorExecution);
                }
            }
            else
            {
                sb.Append(MessageBoxConstants.MessageErrorMergeObj + MessageBoxConstants.MessageNoFilesMerge);
            }
            return new ToolResult(sb.ToString(), success);
        }

        private static ToolResult CopyUsedTexturesObj(string objFile, string outputDirectory, string textureDirectory)
        {
            bool success = false;
            string message = "";
            ObjData objData = ObjParser.TryParseObj(objFile);
            if (objData != null)
            {
                objData.UpdateMtlData();
                MtlData mtlData = objData.Mtl;

                if (mtlData != null)
                {
                    success = ObjModifier.CopyUsedTextures(mtlData, outputDirectory, textureDirectory);
                    message = MessageBoxConstants.GetMessageExecutionGeneric(success);
                }
                else
                {
                    message = MessageBoxConstants.MessageErrorCopyTextures + MessageBoxConstants.MessageMtlNotFound;
                }
            }
            else
            {
                message = MessageBoxConstants.GetMessageExecutionErrorParse(objFile);
            }
            return new ToolResult(message, success);
        }

        private static ToolResult FlipTexture(string fileName, string outputFile, bool horizontal, bool vertical)
        {
            bool success = ImageUtilsShared.FlipTexture(fileName, horizontal, vertical, outputFile);
            string message = MessageBoxConstants.GetMessageExecutionCreation(success, outputFile);
            return new ToolResult(message, success);
        }

        private static ToolResult MirrorTexture(string fileName, string outputFile, bool horizontal, bool vertical)
        {
            bool success = ImageUtilsShared.MirrorTexture(fileName, horizontal, vertical, outputFile);
            string message = MessageBoxConstants.GetMessageExecutionCreation(success, outputFile);
            return new ToolResult(message, success);
        }

        private static ToolResult MakeTransparent(string alphaFile, string textureFile, string outputFile)
        {
            bool success = ImageUtilsShared.MakeTransparent(alphaFile, textureFile, outputFile);
            string message = MessageBoxConstants.GetMessageExecutionCreation(success, outputFile);
            return new ToolResult(message, success);
        }

        private static ToolResult NegativePicture(string fileName, string outputFile)
        {
            bool success = ImageUtilsShared.NegativePicture(fileName, outputFile);
            string message = MessageBoxConstants.GetMessageExecutionCreation(success, outputFile);
            return new ToolResult(message, success);
        }
        
        private static ToolResult CopyTexturesBlacklist(string outputDirectory)
        {
            bool success = FileUtils.CopyFiles(outputDirectory, texturesList);
            string message = MessageBoxConstants.GetMessageExecutionGeneric(success);
            return new ToolResult(message, success);
        }

    }
}
