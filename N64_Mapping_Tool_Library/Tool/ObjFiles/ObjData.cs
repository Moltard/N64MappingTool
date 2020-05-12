using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.Utils;

namespace N64Library.Tool.ObjFiles
{
   public class ObjData
    {
        public string MtlLib { get; set; }
        public MtlData Mtl { get; set; }
        public List<ObjectObj> ObjectsList { get; set; }
        public string FilePath { get; set; }

        public ObjData() : this(null) {}
        public ObjData(string filePath) : this(filePath, null) {}
        public ObjData(string filePath, string mtlLib)
        {
            MtlLib = mtlLib;
            ObjectsList = new List<ObjectObj>();
            FilePath = filePath;
            Mtl = null;
        }

        /// <summary>
        /// Store the data from the parsed .mtl file in the Mtl attribute
        /// </summary>
        public void UpdateMtlData()
        {
            Mtl = MtlParser.GetMtlData(this);
        }

        /// <summary>
        /// Merge the given obj with this one
        /// </summary>
        /// <param name="obj"></param>
        public void MergeObjData(ObjData obj)
        {
            if (obj != null)
            {
                ObjectsList.AddRange(obj.ObjectsList);
                if (Mtl != null)
                {
                    if (obj.Mtl != null) // We add the data from the other mtl
                    {
                        Mtl.MaterialsList.AddRange(obj.Mtl.MaterialsList);
                    }
                }
                else // The current mtl is null
                {
                    if (obj.Mtl != null) // We take the data from the other mtl
                    {
                        Mtl = obj.Mtl;
                    }
                }
            }
        }
    }

    public class ObjectObj
    {
        public string ObjectName { get; set; }
        public string GroupName { get; set; }
        public string MaterialName { get; set; }
        public string TextureName { get; set; }
        public int Smooth { get; set; }
        public List<Point> VerticesList { get; set; }
        public List<UVCoordinates> UVsList { get; set; }
        public List<Vector> NormalsList { get; set; }
        public List<VertIndexesObj> VertIndexesList { get; set; }

        public ObjectObj()
        {
            ObjectName = null;
            GroupName = null;
            MaterialName = null;
            TextureName = null;
            Smooth = -1; // s off
            VerticesList = new List<Point>();
            UVsList = new List<UVCoordinates>();
            NormalsList = new List<Vector>();
            VertIndexesList = new List<VertIndexesObj>();
        }

        public ObjectObj(ObjectObj obj)
        {
            ObjectName = obj.ObjectName;
            GroupName = obj.GroupName;
            MaterialName = obj.TextureName;
            Smooth = obj.Smooth;
            VerticesList = new List<Point>(obj.VerticesList);
            UVsList = new List<UVCoordinates>(obj.UVsList);
            NormalsList = new List<Vector>(obj.NormalsList);
            VertIndexesList = new List<VertIndexesObj>(obj.VertIndexesList);
        }

    }
    

    class LocalIndexesObj
    {
        public int vIndex = 0;
        public int vtIndex = 0;
        public int vnIndex = 0;
    }

    public class VertIndexesObj
    {
        // f 1/1/1 2/2/2 3/3/3
        public List<VertIndexObj> VertIndexList { get; set; }

        public VertIndexesObj()
        {
            VertIndexList = new List<VertIndexObj>();
        }
    }

    public class VertIndexObj
    {
        public int? V { get; set; }
        public int? Vt { get; set; }
        public int? Vn { get; set; }

        public VertIndexObj()
        {
            V = null; Vt = null; Vn = null;
        }
    }

}
