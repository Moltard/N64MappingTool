using N64Library.Tool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Data
{
    public class MtlData
    {
        public List<MaterialMtl> MaterialsList { get; set; }
        public string FilePath { get; set; }

        public MtlData() : this(null) { }
        public MtlData(string filePath)
        {
            MaterialsList = new List<MaterialMtl>();
            FilePath = filePath;
        }

    }

    public class MaterialMtl
    {
        public string NewMtl { get; set; }  // material name
        public Color Kd { get; set; }
        public Color Ks { get; set; }
        public Color Ka { get; set; }
        public Color Ke { get; set; }
        public double Ns { get; set; }
        public double Ni { get; set; }
        public double D { get; set; }
        public int Illum { get; set; }
        public string MapKd { get; set; }  // texture name
        public string MapKs { get; set; }
        public string MapKa { get; set; }
        public string MapKe { get; set; }
        public string MapD { get; set; }
        
        public MaterialMtl()
        {
            NewMtl = null;
            Kd = null;
            Ks = null;
            Ka = null;
            Ke = null;
            Ns = 0.0;
            Ni = 1.0;
            D = 1.0;
            Illum = -1;
            MapKd = null;
            MapKs = null;
            MapKa = null;
            MapKe = null;
            MapD = null;
        }

        public MaterialMtl(MaterialMtl mtl)
        {
            NewMtl = mtl.NewMtl;
            if (mtl.Kd != null)
                Kd = new Color(mtl.Kd);
            else
                Kd = null;
            if (mtl.Ks != null)
                Ks = new Color(mtl.Ks);
            else
                Ks = null;
            if (mtl.Ka != null)
                Ka = new Color(mtl.Ka);
            else
                Ka = null;
            Ns = mtl.Ns;
            Ni = mtl.Ni;
            D = mtl.D;
            Illum = mtl.Illum;
            MapKd = mtl.MapKd;
            MapKs = mtl.MapKs;
            MapKa = mtl.MapKa;
            MapD = mtl.MapD;
        }

    }
}
