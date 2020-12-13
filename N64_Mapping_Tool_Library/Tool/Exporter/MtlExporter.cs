using N64Library.Tool.Data;
using N64Library.Tool.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Exporter
{
    public static class MtlExporter
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
                    mtl.WriteLine(GenericUtils.GetCreditsFile());

                    // newmtl, Ns, Ka, Kd, Ks, Ni, d, illum, map_Kd, map_Ka, map_Ks, map_d
                    foreach (MaterialMtl material in mtlData.MaterialsList)
                    {
                        mtl.WriteLine(MtlUtils.GetNewNewMtl(material.NewMtl));
                        mtl.WriteLine(MtlUtils.GetNewNs(material.Ns));

                        if (material.Ka != null)
                            mtl.WriteLine(MtlUtils.GetNewKa(material.Ka.ToString()));
                        else
                            mtl.WriteLine(MtlUtils.GetNewKa());

                        if (material.Kd != null)
                            mtl.WriteLine(MtlUtils.GetNewKd(material.Kd.ToString()));
                        else
                            mtl.WriteLine(MtlUtils.GetNewKd());

                        if (material.Ks != null)
                            mtl.WriteLine(MtlUtils.GetNewKs(material.Ks.ToString()));
                        else
                            mtl.WriteLine(MtlUtils.GetNewKs());

                        if (material.Ke != null)
                            mtl.WriteLine(MtlUtils.GetNewKe(material.Ke.ToString()));
                        else
                            mtl.WriteLine(MtlUtils.GetNewKe());

                        mtl.WriteLine(MtlUtils.GetNewNi(material.Ni));
                        mtl.WriteLine(MtlUtils.GetNewD(material.D)); 
                        mtl.WriteLine(MtlUtils.GetNewIllum(material.Illum)); 

                        if (material.MapKd != null)
                            mtl.WriteLine(MtlUtils.GetNewMapKd(material.MapKd));
                        if (material.MapKa != null)
                            mtl.WriteLine(MtlUtils.GetNewMapKa(material.MapKa));
                        if (material.MapKs != null)
                            mtl.WriteLine(MtlUtils.GetNewMapKs(material.MapKs));
                        if (material.MapD != null)
                            mtl.WriteLine(MtlUtils.GetNewMapD(material.MapD));

                        mtl.WriteLine(); // Empty line
                    }
                }
            }
            catch
            {
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
                    mtl.WriteLine(GenericUtils.GetCreditsFile());
                    foreach (string line in mtlLines)
                        mtl.WriteLine(line); // Add the data to the mtl file
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
