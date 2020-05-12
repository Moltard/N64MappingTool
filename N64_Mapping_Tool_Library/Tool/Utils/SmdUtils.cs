using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Utils
{
    public class SmdHelper
    {
        
        private const string defaultVersion = "version 1";
        private const string defaultBone = "0 \"root\" -1";
        private const string defaultTime = "time 0";
        private const string defaultPosition = "0 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000";

        private const string nodes = "nodes";
        private const string skeleton = "skeleton";
        private const string triangles = "triangles";
        private const string end = "end";

        public static string GetNewHeader() { return defaultVersion; }

        public static string GetNewNodes() { return nodes; }
        public static string GetNewBone() { return defaultBone; }
        public static string GetNewBone(int id, string boneName, int parentId)
        {
            // <int|ID> "<string|Bone Name>" <int|Parent ID>
            return string.Format("{0} \"{1}\" {2}", id, boneName, parentId); 
        }
        
        public static string GetNewSkeleton() { return skeleton; }
        public static string GetNewTime() { return defaultTime; }
        public static string GetNewTime(int t) { return string.Format("time {0}", t); }
        public static string GetNewPosition() { return defaultPosition; }
        public static string GetNewPosition(int id, string coords, string rotation) {
            // <int|bone ID> <float|PosX PosY PosZ> <float|RotX RotY RotZ>
            return string.Format("{0} {1} {2}", id, coords, rotation);
        }
        
        public static string GetNewTriangles() { return triangles; }
        public static string GetNewTriangle(int parentId, string coords, string normal, string uv)
        {
            // <int|Parent bone> <float|PosX PosY PosZ> <normal|NormX NormY NormZ> <normal|U V> [<int|links> <int|Bone ID> <normal|Weight>]
            return string.Format("{0} {1} {2} {3}", parentId, coords, normal, uv);
        }

        public static string GetNewEnd() { return end; }
       
    }
}
