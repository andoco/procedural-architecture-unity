namespace Andoco.Unity.ProcArch.Shapes
{
    using System.Text;
    using UnityEngine;
    using global::Irony.Parsing;
    
    public interface IShapeProductionSystemBuilder
    {
        IShapeProductionSystem Build (string source);
    }
}
