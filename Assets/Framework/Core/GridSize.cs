namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Size information for a 2D grid of nodes.
    /// </summary>
    [System.Serializable]
    public class GridSize
    {
        public float nodeSize;
        public int horizNodes;
        public int vertNodes;

		public GridSize()
		{
		}

		public GridSize(float nodeSize, int width, int depth)
		{
			this.nodeSize = nodeSize;
			this.horizNodes = width;
			this.vertNodes = depth;
		}

        public Vector3 GetSizeVector()
        {
            return new Vector3(horizNodes * nodeSize, 0f, vertNodes * nodeSize);
        }

        public Bounds GetBounds(Vector3 offset)
        {
            var s = this.GetSizeVector();
            var center = offset + (s / 2f);

            return new Bounds(center, new Vector3(s.x, 0f, s.z));
        }

        /// <summary>
        /// Gets the position of the bottom-left corner of the cell in world coordinates.
        /// </summary>
        public Vector3 GetCellPosition(int cellX, int cellY, Vector3 offset)
        {
            var worldX = offset.x + cellX * nodeSize;
            var worldZ = offset.z + cellY * nodeSize;

            return new Vector3(worldX, offset.y, worldZ);
        }

        /// <summary>
        /// Gets the 2D cell index vector for a world position.
        /// </summary>
        public IntVector2 GetCellIndex(Vector3 pos, Vector3 offset)
        {
            var x = (int)((pos.x - offset.x) / this.nodeSize);
            var y = (int)((pos.z - offset.z) / this.nodeSize);

            return new IntVector2(x, y);
        }

        /// <summary>
        /// Gets the bounds of a cell in world coordinates.
        /// </summary>
        public Bounds GetCellBounds(int cellX, int cellY, Vector3 offset)
        {
            var pos = this.GetCellPosition(cellX, cellY, offset);
            var size = new Vector3(nodeSize, 0f, nodeSize);

            return new Bounds(pos + size / 2f, size);
        }
    }
}