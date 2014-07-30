namespace Andoco.Unity.Framework.Core.Meshes
{
    using UnityEngine;

    public static class MeshGridExtensions
    {
        /// <summary>
        /// Sets the vertex color for each vertex in a quad at a given grid coordinate.
        /// </summary>
        /// <remarks>
        /// Setting quad colors individually is slow due to the need to reassign the Mesh.colors
        /// property every time.
        /// </remarks>
        /// <param name="mesh">The mesh to modify.</param>
        /// <param name="x">The x coordinate of the grid cell.</param>
        /// <param name="y">The y coordinate of the grid cell.</param>
        /// <param name="gridWidth">The number of cells in a grid row.</param>
        /// <param name="c">The vertex color to set.</param>
        public static void SetColorForGridQuad(this Mesh mesh, int x, int y, int gridWidth, Color c)
        {
            var startIndex = (y * gridWidth * 4) + (x * 4);
            var vcolors = mesh.colors;
            for (int i=0; i < 4; i++)
            {
                vcolors[startIndex + i] = c;
            }
            mesh.colors = vcolors;
        }

        /// <summary>
        /// Sets the colors for all quads cells in a grid.
        /// </summary>
        /// <remarks>
        /// Colors will be assigned to vertices in groups of 4, equivalent to visiting
        /// all cells across each row in turn.
        /// </remarks>
        /// <param name="mesh">The mesh to modify.</param>
        /// <param name="colors">The colors to assign to the quads in the grid.</param>
        public static void SetColorsForQuadGrid(this Mesh mesh, Color[] colors)
        {
            int colorIndex = -1;
            Color color = Color.white;

            var numVerts = mesh.vertices.Length;
            var vcolors = (mesh.colors == null | mesh.colors.Length != numVerts) ? new Color[numVerts] : mesh.colors;

            for (int i=0; i < numVerts; i++)
            {
                if (i % 4 == 0)
                {
                    colorIndex++;
                    color = colors[colorIndex];
                }

                vcolors[i] = color;
            }

            mesh.colors = vcolors;
        }
    }
}