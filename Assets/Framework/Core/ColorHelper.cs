namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    public static class ColorHelper
    {
        public static Color RandomRGB()
        {
            return new Color(Random.value, Random.value, Random.value);
        }

        public static Color HeatMapColor(float value, Color neutral, Color positive1, Color positive2, Color negative1, Color negative2)
        {
            if (value < -1f || value > 1f)
                throw new System.ArgumentOutOfRangeException("value", value, "HeatMap value must be in the range -1..1");

            Color c;
            if (value < -0.5f)
                c = Color.Lerp(negative1, negative2, -(value+0.5f)/0.5f);
            else if (value < 0f)
                c = Color.Lerp(neutral, negative1, -value/0.5f);
            else if (value > 0.5f)
                c = Color.Lerp(positive1, positive2, (value-0.5f)/0.5f);
            else if (value > 0f)
                c = Color.Lerp(neutral, positive1, value/0.5f);
            else
                c = neutral;

            return c;
        }
    }
}