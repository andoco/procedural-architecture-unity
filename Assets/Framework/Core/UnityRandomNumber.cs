namespace Andoco.Unity.Framework.Core
{
    using Andoco.Core;

    using UnityEngine;

    public class UnityRandomNumber : IRandomNumber
    {
        static UnityRandomNumber()
        {
            Instance = new UnityRandomNumber();
        }

        public static UnityRandomNumber Instance { get; private set; }

        #region IRandomNumber implementation

        public double NextDouble ()
        {
            return UnityEngine.Random.value;
        }

        public int Next(int max)
        {
            return Random.Range(0, max);
        }

        public int Next (int min, int max)
        {
            return Random.Range(min, max);
        }

        #endregion
    }
}

