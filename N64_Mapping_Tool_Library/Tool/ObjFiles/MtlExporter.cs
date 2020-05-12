using N64Library.Tool.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.ObjFiles
{
    public class MtlExporter
    {
        /// <summary>
        /// Create the mtl file with the given data
        /// </summary>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        /// <param name="mtlFilename">Output path</param>
        /// <returns></returns>
        public static bool WriteMtl(MtlData mtlData, string mtlFilename)
        {
            try
            {
                using (StreamWriter mtl = new StreamWriter(mtlFilename))
                {
                    mtl.WriteLine(ObjHelper.GetCreditsFile());

                    // newmtl, Ns, Ka, Kd, Ks, Ni, d, illum, map_Kd, map_Ka, map_Ks, map_d
                    foreach (MaterialMtl material in mtlData.MaterialsList)
                    {
                        mtl.WriteLine(MtlHelper.GetNewNewMtl(material.NewMtl));
                        mtl.WriteLine(MtlHelper.GetNewNs(material.Ns));

                        if (material.Ka != null)
                            mtl.WriteLine(MtlHelper.GetNewKa(material.Ka.ToString()));
                        else
                            mtl.WriteLine(MtlHelper.GetNewKa());

                        if (material.Kd != null)
                            mtl.WriteLine(MtlHelper.GetNewKd(material.Kd.ToString()));
                        else
                            mtl.WriteLine(MtlHelper.GetNewKd());

                        if (material.Ks != null)
                            mtl.WriteLine(MtlHelper.GetNewKs(material.Ks.ToString()));
                        else
                            mtl.WriteLine(MtlHelper.GetNewKs());

                        if (material.Ke != null)
                            mtl.WriteLine(MtlHelper.GetNewKe(material.Ke.ToString()));
                        else
                            mtl.WriteLine(MtlHelper.GetNewKe());

                        mtl.WriteLine(MtlHelper.GetNewNi(material.Ni));
                        mtl.WriteLine(MtlHelper.GetNewD(material.D)); 
                        mtl.WriteLine(MtlHelper.GetNewIllum(material.Illum)); 

                        if (material.MapKd != null)
                            mtl.WriteLine(MtlHelper.GetNewMapKd(material.MapKd));
                        if (material.MapKa != null)
                            mtl.WriteLine(MtlHelper.GetNewMapKa(material.MapKa));
                        if (material.MapKs != null)
                            mtl.WriteLine(MtlHelper.GetNewMapKs(material.MapKs));
                        if (material.MapD != null)
                            mtl.WriteLine(MtlHelper.GetNewMapD(material.MapD));

                        mtl.WriteLine(); // Empty line
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create the mtl file with the given lines
        /// </summary>
        /// <param name="mtlLines"></param>
        /// <param name="mtlFilename"></param>
        /// <returns></returns>
        public static bool WriteMtl(List<string> mtlLines, string mtlFilename)
        {
            try
            {
                using (StreamWriter mtl = new StreamWriter(mtlFilename)) // Make the .mtl file
                {
                    mtl.WriteLine(ObjHelper.GetCreditsFile());
                    foreach (string line in mtlLines)
                        mtl.WriteLine(line); // Add the data to the mtl file
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

    }
}
