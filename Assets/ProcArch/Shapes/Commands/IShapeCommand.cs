namespace Andoco.Unity.ProcArch.Shapes.Commands
{
    using System.Linq;
    using Andoco.Unity.ProcArch.Shapes.Configuration;
    
    public interface IShapeCommand
    {
        string Name { get; set; }
    
        Argument[] Arguments { get; set; }
    
        void Execute (IShapeConfiguration configuration);
    }
}
