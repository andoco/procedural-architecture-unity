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
    }
}