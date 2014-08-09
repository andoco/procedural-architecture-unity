namespace Andoco.Unity.ProcArch.Shapes
{
    using System;
    using System.Collections.Generic;
    using Andoco.Unity.ProcArch.Shapes.Configuration;
    using Andoco.Unity.ProcArch.Shapes.Rules;
    
    public interface IShapeProductionSystem
    {
        IDictionary<string, ShapeRule> Rules { get; }
    
        IDictionary<string, string> DefaultArgs { get; }
    
        string Axiom { get; set; }
    
        IShapeConfiguration Run (IList<string> rootArgs, IDictionary<string, string> globalArgs);
    }
}
