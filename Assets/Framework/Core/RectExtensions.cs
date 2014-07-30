namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    public static class RectExtensions
    {
        public static Vector2[] ToPoly(this Rect rect)
        {
            return new [] {
                new Vector2(rect.xMax, rect.yMin),
                new Vector2(rect.xMax, rect.yMax),
                new Vector2(rect.xMin, rect.yMax),
                new Vector2(rect.xMin, rect.yMin)
            };
        }
    }
}