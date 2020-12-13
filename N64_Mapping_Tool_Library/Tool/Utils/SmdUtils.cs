using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Utils
{
    internal static class SmdUtils
    {
        
        private const string DefaultVersion = "version 1";
        private const string DefaultBone = "0 \"root\" -1";
        private const string DefaultTime = "time 0";
        private const string DefaultPosition = "0 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000";

        private const string Nodes = "nodes";
        private const string Skeleton = "skeleton";
        private const string Triangles = "triangles";
        private const string End = "end";

        public static string GetNewHeader() { return DefaultVersion; }

        public static string GetNewNodes() { return Nodes; }
        public static string GetNewBone() { return DefaultBone; }
        public static string GetNewBone(int id, string boneName, int parentId)
        {
            // <int|ID> "<string|Bone Name>" <int|Parent ID>
            return $"{id} \"{boneName}\" {parentId}";
        }
        
        public static string GetNewSkeleton() { return Skeleton; }
        public static string GetNewTime() { return DefaultTime; }
        public static string GetNewTime(int t) { return "time " + t; }
        public static string GetNewPosition() { return DefaultPosition; }
        public static string GetNewPosition(int id, string coords, string rotation) {
            // <int|bone ID> <float|PosX PosY PosZ> <float|RotX RotY RotZ>
            return $"{id} {coords} {rotation}";
        }
        
        public static string GetNewTriangles() { return Triangles; }
        public static string GetNewTriangle(int parentId, string coords, string normal, string uv)
        {
            // <int|Parent bone> <float|PosX PosY PosZ> <normal|NormX NormY NormZ> <normal|U V> [<int|links> <int|Bone ID> <normal|Weight>]
            return $"{parentId} {coords} {normal} {uv}";
        }

        public static string GetNewEnd() { return End; }
       
    }
}
