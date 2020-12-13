using N64Library.Tool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Modifier
{
    public static class WrlModifier
    {
        /// <summary>
        /// Reverse the vertex order of the model
        /// </summary>
        /// <param name="wrlData">Data parsed from the wrl file</param>
        public static void ReverseVertexOrder(WrlData wrlData)
        {
            foreach (Transform transform in wrlData.TransformsList)
            {
                foreach (Shape shape in transform.ShapesList)
                {
                    if (shape.Geometry != null)
                    {
                        CoordIndexesWrl coordIndexes = shape.Geometry.CoordIndexes;
                        CoordIndexesWrl texCoordIndexes = shape.Geometry.TexCoordIndexes;

                        if (coordIndexes != null)
                        {
                            foreach (CoordIndexWrl indexWrl in coordIndexes.IndexesList)
                            {
                                indexWrl.Indexes.Reverse();
                            }
                        }
                        if (texCoordIndexes != null)
                        {
                            foreach (CoordIndexWrl indexWrl in texCoordIndexes.IndexesList)
                            {
                                indexWrl.Indexes.Reverse();
                            }
                        }
                    }
                }
            }
        }
    }
}
