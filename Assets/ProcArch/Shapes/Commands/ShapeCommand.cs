namespace Andoco.Unity.ProcArch.Shapes.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Andoco.Unity.ProcArch.Shapes.Configuration;
    using Andoco.Unity.ProcArch.Shapes.Successors;
    
    public class ShapeCommand : IShapeCommand
    {
        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        public string Name { get; set; }
    
        /// <summary>
        /// Gets or sets the arguments to be supplied to the command.
        /// </summary>
        public Argument[] Arguments { get; set; }
    
        /// <summary>
        /// Gets or sets the shape symbols that are present in the command body.
        /// </summary>
        public ShapeSymbol[] Shapes { get; set; }
    
        public void Execute (IShapeConfiguration configuration)
        {
            switch (Name) {
            case "Set":
            case "Vol":
                var volArgs = configuration.ResolveArgs (this.Arguments);
                var volName = volArgs [0].Value;
    //          var volStyle = volArgs.Length > 1 ? volArgs[1].Value : null;
                configuration.AddVolume (volName, volArgs); // TODO: pass named args.
                break;
            case "Trans":
                var axes = configuration.ResolveArgs(this.Arguments).Select(x => Size.Parse(x.Value)).ToArray();
                configuration.TranslateScope(axes[0], axes[1], axes[2]);
                break;
            case "Rot":
                var rotAxes = configuration.ResolveArgs (this.Arguments).Select (x => float.Parse (x.Value)).ToArray ();
                configuration.RotateScope (new Vector3 (rotAxes [0], rotAxes [1], rotAxes [2]));
                break;
            case "Scale":
                var scaleSizes = configuration.ResolveArgs (this.Arguments).Select (x => Size.Parse (x.Value)).ToArray ();
                configuration.ScaleScope (scaleSizes [0], scaleSizes [1], scaleSizes [2]);
                break;
            case "Push":
                configuration.PushScope ();
                break;
            case "Pop":
                configuration.PopScope ();
                break;
            case "Subdiv":
                var subdivArgs = configuration.ResolveArgs (this.Arguments);
                var sizes = subdivArgs.Skip (1).Select (arg => Size.Parse (arg.Value)).ToArray ();
                configuration.SplitDivideScope (subdivArgs [0].Value, sizes, Shapes);
                break;
            case "Comp":
                var compArgs = configuration.ResolveArgs(this.Arguments);
                configuration.SplitComponent(compArgs[0].Value, Shapes[0]);
                break;
            case "Repeat":
                var repeatArgs = configuration.ResolveArgs(this.Arguments);
                var repeatSize = Size.Parse(repeatArgs[1].Value);
                configuration.Repeat(repeatArgs[0].Value, repeatSize, Shapes[0]);
                break;
            default:
                throw new System.ArgumentException(string.Format("Unknown command: {0}", this.Name), "Name");
            }
        }
    }
}