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

        public Vector3 GetSizeVector()
        {
            return new Vector3(horizNodes * nodeSize, 0f, vertNodes * nodeSize);
        }
    }
}