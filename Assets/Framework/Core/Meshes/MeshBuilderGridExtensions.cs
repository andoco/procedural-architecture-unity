namespace Andoco.Unity.Framework.Core.Meshes
{
    using System;
    using UnityEngine;

    public static class MeshBuilderGridExtensions
    {
        public static void BuildSimpleGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int segmentCount)
        {
            BuildSimpleGrid(meshBuilder, cellWidth, cellLength, segmentCount, (x, y) => 0f);
        }

        public static void BuildSimpleGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int segmentCount, Func<int, int, float> heightFunc)
        {
            for (int i = 0; i < segmentCount; i++)
            {
                float z = cellLength * i;
                
                for (int j = 0; j < segmentCount; j++)
                {
                    float x = cellWidth * j;
                    
                    Vector3 offset = new Vector3(x, heightFunc(j, i), z);

                    meshBuilder.BuildQuad(offset, cellWidth, cellLength);
                }
            }
        }
        
        public static void BuildGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int segmentCount)
        {
            //Loop through the rows:
            for (int i = 0; i <= segmentCount; i++)
            {
                //incremented values for the Z position and V coordinate:
                float z = cellLength * i;
                float v = (1.0f / segmentCount) * i;
                
                //Loop through the collumns:
                for (int j = 0; j <= segmentCount; j++)
                {
                    //incremented values for the X position and U coordinate:
                    float x = cellWidth * j;
                    float u = (1.0f / segmentCount) * j;
                    
                    //The position offset for this quad:
                    Vector3 offset = new Vector3(x, 0f, z);

                    //build quads that share vertices:
                    Vector2 uv = new Vector2(u, v);
                    bool buildTriangles = i > 0 && j > 0;
                    
                    BuildQuadForGrid(meshBuilder, offset, uv, buildTriangles, segmentCount + 1);
                }
            }
        }

        /// <summary>
        /// Builds a smooth surfaced grid with the origin in the bottom-left corner.
        /// </summary>
        /// <remarks>
        /// The grid is comprised of quads with no shared vertices, allowing flat shading.
        /// </remarks>
        /// <param name="meshBuilder">Mesh builder.</param>
        /// <param name="cellWidth">Cell width.</param>
        /// <param name="cellLength">Cell length.</param>
        /// <param name="segmentCount">Segment count.</param>
        /// <param name="heightFunc">Height func.</param>
        public static void BuildSmoothGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int segmentCount, Func<int, int, float> heightFunc)
        {
            int baseIndex = 0;

            for (int i = 0; i < segmentCount; i++)
            {
                for (int j = 0; j < segmentCount; j++)
                {
                    var x = cellWidth * j;
                    var z = cellLength * i;

                    meshBuilder.Vertices.Add(new Vector3(x, heightFunc(j, i), z));
                    meshBuilder.Vertices.Add(new Vector3(x, heightFunc(j, i + 1), z + cellLength));
                    meshBuilder.Vertices.Add(new Vector3(x + cellWidth, heightFunc(j + 1, i + 1), z + cellLength));
                    meshBuilder.Vertices.Add(new Vector3(x + cellWidth, heightFunc(j + 1, i), z));
                    
                    meshBuilder.Normals.Add(Vector3.up);
                    meshBuilder.Normals.Add(Vector3.up);
                    meshBuilder.Normals.Add(Vector3.up);
                    meshBuilder.Normals.Add(Vector3.up);

                    meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
                    meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);

                    baseIndex += 4;
                }
            }
        }

        /// <summary>
        /// Builds a single quad as part of a mesh grid.
        /// </summary>
        /// <param name="meshBuilder">The mesh builder currently being added to.</param>
        /// <param name="position">A position offset for the quad. Specifically the position of the corner vertex of the quad.</param>
        /// <param name="uv">The UV coordinates of the quad's corner vertex.</param>
        /// <param name="buildTriangles">Should triangles be built for this quad? This value should be false if this is the first quad in any row or collumn.</param>
        /// <param name="vertsPerRow">The number of vertices per row in this grid.</param>
        public static void BuildQuadForGrid(this IMeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow)
        {
            meshBuilder.Vertices.Add(position);
            meshBuilder.UVs.Add(uv);
            
            if (buildTriangles)
            {
                int baseIndex = meshBuilder.Vertices.Count - 1;
                
                int index0 = baseIndex;
                int index1 = baseIndex - 1;
                int index2 = baseIndex - vertsPerRow;
                int index3 = baseIndex - vertsPerRow - 1;
                
                meshBuilder.AddTriangle(index0, index2, index1);
                meshBuilder.AddTriangle(index2, index3, index1);
            }
        }
    }
}