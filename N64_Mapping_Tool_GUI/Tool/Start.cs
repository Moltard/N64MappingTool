using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using N64Library.Tool.WrlFiles;
using N64Library.Tool.ObjFiles;
using N64Library.Tool.Utils;
using N64Library.Tool.SmdFiles;
using N64Library.Tool.WrlFiles;

namespace N64Mapping.Tool
{

    enum ActionN64
    {
        WrlConversion,
        WrlsConversion,
        DeleteMaterials,
        DeleteUnusedMaterials,
        AddMaterials,
        ModifyObj,
        MergeObjFiles,
        MergeSpecObjFiles,
        CopyObjTextures,
        ObjToSmd,
        RefModelSmd,
        FlipPicture,
        MirrorPicture,
        MakeTransparent,
        CopyBlacklistTextures,
        NegativePicture,
    }

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
        CopyUseTextureDir = 128,
        FlipHorizontally = 256,
        FlipVertically = 512,
        MirrorHorizontally = 1024,
        MirrorVertically = 2048,
        SmdUseTextureName = 4096,
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


        public static bool Tool(ActionN64 action, Dictionary<string,string> inputs, ObjOptions objOptions)
        {
            string objOutput = null;
            switch (action)
            {
                case ActionN64.WrlConversion: // Convert Wrl to Obj
                    if (inputs.TryGetValue("WrlFile", out string wrlFile))
                    {
                        if (inputs.TryGetValue("ObjOutput", out objOutput))
                        {
                            return ConvertWrlToObj(wrlFile, objOutput, objOptions);
                        }
                    }
                    break;
                case ActionN64.WrlsConversion: // Convert Wrl files to Obj
                    if (inputs.TryGetValue("WrlDirectory", out string wrlDirectory))
                    {
                        return ConvertWrlsToObj(wrlDirectory, objOptions);
                    }
                    break;


                case ActionN64.DeleteMaterials: // Delete Obj Materials
                    if (inputs.TryGetValue("ObjFile", out string objFile))
                    {
                        if (inputs.TryGetValue("ObjOutput", out objOutput))
                        {
                            return DeleteMaterialsObj(objFile, objOutput);
                        }
                    }
                    break;
                case ActionN64.DeleteUnusedMaterials: // Delete Unused Obj Materials
                    if (inputs.TryGetValue("ObjFile", out objFile))
                    {
                        if (inputs.TryGetValue("ObjOutput", out objOutput))
                        {
                            return DeleteUnusedMaterialsObj(objFile, objOutput);
                        }
                    }
                    break;

                case ActionN64.AddMaterials: // Add Obj Materials
                    if (inputs.TryGetValue("ObjFile", out objFile))
                    {
                        if (inputs.TryGetValue("ObjOutput", out objOutput))
                        {
                            return AddMaterialsObj(objFile, objOutput);
                        }
                    }
                    break;
                case ActionN64.ModifyObj: // Modify Obj
                    if (inputs.TryGetValue("ObjFile", out objFile))
                    {
                        if (inputs.TryGetValue("ObjOutput", out objOutput))
                        {
                            return ModifyObj(objFile, objOutput, objOptions, inputs);
                        }
                    }
                    break;
                
                case ActionN64.ObjToSmd: //  Obj to Smd
                    if (inputs.TryGetValue("ObjFile", out objFile))
                    {
                        if (inputs.TryGetValue("SmdOutput", out string smdOutput))
                        {
                            if (objOptions.HasFlag(ObjOptions.SmdUseTextureName))
                                return ConvertObjToSmd(objFile, smdOutput, true);
                            else
                                return ConvertObjToSmd(objFile, smdOutput, false);
                        }
                    }
                    break;
                case ActionN64.RefModelSmd: // Obj to Smd
                    if (inputs.TryGetValue("ObjFile", out objFile))
                    {
                        if (inputs.TryGetValue("SmdOutput", out string smdOutput))
                        {
                            return ConvertRefModelSmd(objFile, smdOutput, inputs);
                        }
                    }
                    break;
                case ActionN64.CopyObjTextures: // Copy Obj Textures
                    if (inputs.TryGetValue("ObjFile", out objFile))
                    {
                        if (inputs.TryGetValue("DestDir", out string destDir))
                        {
                            if (objOptions.HasFlag(ObjOptions.CopyUseTextureDir))
                            {
                                if (inputs.TryGetValue("TextureDir", out string textureDir))
                                {
                                    return CopyUsedTexturesObj(objFile, destDir, textureDir);
                                }
                            }
                            else
                            {
                                return CopyUsedTexturesObj(objFile, destDir, null);
                            }
                        }
                    }
                    break;
                case ActionN64.MergeObjFiles: // Merge Obj Files in a folder
                    if (inputs.TryGetValue("ObjDir", out string objFolder))
                    {
                        if (inputs.TryGetValue("ObjOutput", out objOutput))
                        {
                            return MergeObjFiles(objFolder, objOutput);
                        }
                    }
                    break;

                case ActionN64.MergeSpecObjFiles: // Merge specific Obj Files
                    if (inputs.TryGetValue("ObjOutput", out objOutput))
                    {
                        return MergeObjFiles(objFileList, objOutput);
                    }
                    break;

                case ActionN64.FlipPicture: // Flip a picture
                    if (inputs.TryGetValue("PictureFile", out string pictureFile))
                    {
                        if (inputs.TryGetValue("PictureOutput", out string outputFile))
                        {
                            return FlipTexture(pictureFile, outputFile, objOptions);
                        }
                    }
                    break;

                case ActionN64.MirrorPicture: // Mirror a picture
                    if (inputs.TryGetValue("PictureFile", out pictureFile))
                    {
                        if (inputs.TryGetValue("PictureOutput", out string outputFile))
                        {
                            return MirrorTexture(pictureFile, outputFile, objOptions);
                        }
                    }
                    break;

                case ActionN64.MakeTransparent: // Create a transparent picture
                    if (inputs.TryGetValue("AlphaFile", out string alphaFile))
                    {
                        if (inputs.TryGetValue("TextureFile", out string textureFile))
                        {
                            if (inputs.TryGetValue("PictureOutput", out string outputFile))
                            {
                                return MakeTransparent(alphaFile, textureFile, outputFile);
                            }
                        }
                    }
                    break;
                case ActionN64.CopyBlacklistTextures: // Copy BlackListed Textures
                    if (inputs.TryGetValue("OutputFolder", out string outputFolder))
                    {
                        return CopyTexturesBlacklist(outputFolder);
                    }
                    break;
                case ActionN64.NegativePicture: // Copy BlackListed Textures
                    if (inputs.TryGetValue("PictureFile", out pictureFile))
                    {
                        if (inputs.TryGetValue("PictureOutput", out string outputFile))
                        {
                            return NegativePicture(pictureFile, outputFile);
                        }
                    }
                    break;


            }
            return false;
        }

        private static bool ConvertWrlToObj(string wrlFile, string objOutput, ObjOptions objOptions)
        {
            WrlData wrlData = WrlParser.ParseWrl(wrlFile);
            if (wrlData != null)
            {
                if (objOptions.HasFlag(ObjOptions.ReverseVertex))
                {
                    WrlModifier.ReverseVertexOrder(wrlData);
                }

                if (WrlExporter.WriteObj(wrlData, objOutput))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ConvertWrlsToObj(string directory, ObjOptions objOptions)
        {
            List<string> wrlList = FileHelper.GetFiles(directory, "*.wrl", true);
            foreach (string wrlFile in wrlList) {
                string wrlDirectory = FileHelper.GetDirectoryName(wrlFile);
                string wrlFilenameNoExt = FileHelper.GetFileNameWithoutExtension(wrlFile);
                string objOutput = FileHelper.Combine(wrlDirectory, wrlFilenameNoExt + ".obj");

                if (!ConvertWrlToObj(wrlFile, objOutput, objOptions))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool DeleteMaterialsObj(string objFile, string objOutput)
        {
            ObjData objData = ObjParser.ParseObj(objFile);
            if (objData != null)
            {
                ObjModifier.DeleteMaterials(objData);
                if (ObjExporter.WriteObj(objData, null, objOutput, makeMtl: false, useExistingMtl: false))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool DeleteUnusedMaterialsObj(string objFile, string objOutput)
        {
            ObjData objData = ObjParser.ParseObj(objFile);
            if (objData != null)
            {
                objData.UpdateMtlData();
                MtlData mtlData = objData.Mtl;

                if (mtlData != null)
                {
                    ObjModifier.DeleteUnusedMaterials(objData, mtlData);
                    if (ObjExporter.WriteObj(objData, mtlData, objOutput, makeMtl: true, useExistingMtl: true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool AddMaterialsObj(string objFile, string objOutput)
        {
            ObjData objData = ObjParser.ParseObj(objFile);
            if (objData != null)
            {
                ObjModifier.ParseTextures(objData);
                ObjModifier.CalculateNormal(objData);
                if (ObjExporter.WriteObj(objData, null, objOutput, makeMtl: true, useExistingMtl: false))
                {
                    return true;
                }
            }
            return false;
        }


        private static bool ModifyObj(string objFile, string objOutput, ObjOptions objOptions, Dictionary<string, string> inputs)
        {
            ObjData objData = ObjParser.ParseObj(objFile);
            if (objData != null)
            {
                objData.UpdateMtlData();
                MtlData mtlData = objData.Mtl;

                // Delete Objects first
                if (mtlData != null)
                {
                    if (objOptions.HasFlag(ObjOptions.BlackList))
                    {
                        ObjModifier.DeleteMatchingGroups(objData, mtlData, texturesList);
                    }
                }

                ObjModifier.CalculateNormal(objData);

                if (objOptions.HasFlag(ObjOptions.UniformScale))
                {
                    double? scaleValue = null;
                    if (inputs.TryGetValue("ScaleValue", out string scaleStr))
                    {
                        scaleValue = Helper.StringToDouble(scaleStr);
                    }
                    if (scaleValue != null)
                    {
                        ObjModifier.UniformScale(objData, (double)scaleValue); // Scale Model
                    }
                }

                if (objOptions.HasFlag(ObjOptions.NonUniformScale))
                {
                    double? scaleValueX = null;
                    double? scaleValueY = null;
                    double? scaleValueZ = null;
                    if (inputs.TryGetValue("ScaleValueX", out string scaleStr))
                    {
                        scaleValueX = Helper.StringToDouble(scaleStr);
                    }
                    if (inputs.TryGetValue("ScaleValueY", out scaleStr))
                    {
                        scaleValueY = Helper.StringToDouble(scaleStr);
                    }
                    if (inputs.TryGetValue("ScaleValueZ", out scaleStr))
                    {
                        scaleValueZ = Helper.StringToDouble(scaleStr);
                    }
                    if (scaleValueX != null && scaleValueY != null && scaleValueZ != null)
                    {
                        // Scale Model
                        ObjModifier.NonUniformScale(objData, (double)scaleValueX, (double)scaleValueY, (double)scaleValueZ); 
                    }
                }

                if (objOptions.HasFlag(ObjOptions.Rotate))
                {
                    double? angleX = null;
                    double? angleY = null;
                    double? angleZ = null;
                    if (inputs.TryGetValue("RotateValueX", out string rotationStr))
                    {
                        angleX = Helper.StringToDouble(rotationStr);
                    }
                    if (inputs.TryGetValue("RotateValueY", out rotationStr))
                    {
                        angleY = Helper.StringToDouble(rotationStr);
                    }
                    if (inputs.TryGetValue("RotateValueZ", out rotationStr))
                    {
                        angleZ = Helper.StringToDouble(rotationStr);
                    }
                    if (angleX == null)
                        angleX = 0.0;
                    if (angleY == null)
                        angleY = 0.0;
                    if (angleZ == null)
                        angleZ = 0.0;

                    // Rotate Model
                    ObjModifier.RotateModel(objData, (double)angleX, (double)angleY, (double)angleZ);
                }
                
                if (objOptions.HasFlag(ObjOptions.ReverseVertex))
                    ObjModifier.ReverseVertexOrder(objData); // Reverse Vertex Order
                
                
                if (mtlData != null)
                {
                    if (objOptions.HasFlag(ObjOptions.Merge))
                        ObjModifier.MergeGroups(objData, mtlData); // Merge groups and materials

                    if (objOptions.HasFlag(ObjOptions.Sort))
                        ObjModifier.SortMaterials(mtlData); // Sort materials
                }

                if (objOptions.HasFlag(ObjOptions.Sort))
                    ObjModifier.SortGroups(objData); // Sort groups


                if (ObjExporter.WriteObj(objData, mtlData, objOutput, makeMtl: true, useExistingMtl: true))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ConvertObjToSmd(string objFile, string smdOutput, bool useTextureName)
        {
            ObjData objData = ObjParser.ParseObj(objFile);
            if (objData != null)
            {
                objData.UpdateMtlData();
                MtlData mtlData = objData.Mtl;

                if (mtlData != null)
                {
                    if (SmdExporter.WriteSmd(objData, mtlData, smdOutput, useTextureName))
                    {
                        return true;
                    }
                }
                else
                {
                    // No MTL means the SMD will use a default material name for all objects
                    if (SmdExporter.WriteSmd(objData, null, smdOutput, false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool ConvertRefModelSmd(string objFile, string smdOutput, Dictionary<string, string> inputs)
        {
            ObjData objData = ObjParser.ParseObj(objFile);
            if (objData != null)
            {
                // Delete all materials
                ObjModifier.DeleteMaterials(objData);

                // Scale the model by the given value
                double? scaleValue = null;
                if (inputs.TryGetValue("ScaleValue", out string scaleStr))
                {
                    scaleValue = Helper.StringToDouble(scaleStr);
                }
                if (scaleValue != null)
                {
                    ObjModifier.UniformScale(objData, (double)scaleValue); // Scale Model
                }

                // The SMD will use a default material name for all objects
                if (SmdExporter.WriteSmd(objData, null, smdOutput, false))
                {
                    return true;
                }
                
            }
            return false;
        }
        // ConvertRefModelSmd(objFile, smdOutput, inputs);

        private static bool CopyUsedTexturesObj(string objFile, string outputFolder, string textureFolder)
        {
            ObjData objData = ObjParser.ParseObj(objFile);
            if (objData != null)
            {
                objData.UpdateMtlData();
                MtlData mtlData = objData.Mtl;

                if (mtlData != null)
                {
                    if (ObjModifier.CopyUsedTextures(mtlData, outputFolder, textureFolder))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool MergeObjFiles(string objFolder, string objOutput)
        {
            return MergeObjFiles(FileHelper.GetFiles(objFolder, "*.obj"), objOutput);
        }
        
        private static bool MergeObjFiles(List<string> fileList, string objOutput)
        {
            if (fileList != null)
            {
                List<ObjData> objDatas = ObjParser.ParseObjs(fileList);
                ObjData objData = ObjModifier.MergeObjFiles(objDatas);
                if (objData != null)
                {
                    if (ObjExporter.WriteObj(objData, objData.Mtl, objOutput, makeMtl: true, useExistingMtl: true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private static bool FlipTexture(string fileName, string outputFile, ObjOptions objOptions)
        {
            bool horizontal = objOptions.HasFlag(ObjOptions.FlipHorizontally) ? true: false;
            bool vertical = objOptions.HasFlag(ObjOptions.FlipVertically) ? true : false;
            
            if (ImageHelper.FlipTexture(fileName, horizontal, vertical, outputFile))
            {
                return true;
            }
            return false;
        }

        private static bool MirrorTexture(string fileName, string outputFile, ObjOptions objOptions)
        {
            bool horizontal = objOptions.HasFlag(ObjOptions.MirrorHorizontally) ? true: false;
            bool vertical = objOptions.HasFlag(ObjOptions.MirrorVertically) ? true : false;
            
            if (ImageHelper.MirrorTexture(fileName, horizontal, vertical, outputFile))
            {
                return true;
            }
            return false;
        }

        private static bool MakeTransparent(string alphaFile, string textureFile, string outputFile)
        {
            if (ImageHelper.MakeTransparent(alphaFile, textureFile, outputFile))
            {
                return true;
            }
            return false;
        }

        private static bool NegativePicture(string fileName, string outputFile)
        {
            if (ImageHelper.NegativePicture(fileName, outputFile))
            {
                return true;
            }
            return false;
        }



        private static bool CopyTexturesBlacklist(string outputFolder)
        {
            return FileHelper.CopyFiles(outputFolder, texturesList);
        }

    }
}
