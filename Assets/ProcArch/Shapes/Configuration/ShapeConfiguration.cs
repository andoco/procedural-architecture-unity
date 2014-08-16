namespace Andoco.Unity.ProcArch.Shapes.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Andoco.Core.Graph;
    using Andoco.Core.Graph.Tree;
    using Andoco.Unity.Framework.Core.Logging;
    using Andoco.Unity.ProcArch.Shapes;
    using Andoco.Unity.ProcArch.Shapes.Geometry.Volumes;
    using Andoco.Unity.ProcArch.Shapes.Rules;
    using Andoco.Unity.ProcArch.Shapes.Successors;
    
    public class ShapeConfiguration : IShapeConfiguration
    {
        private readonly ILog log = LogManager.GetCurrentClassLogger ();
        private readonly Stack<IScope> scopeStack = new Stack<IScope> ();
        private int counter;
        private readonly IDictionary<string, ShapeRule> rules;
        private readonly IDictionary<string, string> globalArgs = new Dictionary<string, string> ();
        private ShapeNode currentNode;
    
        public ShapeConfiguration (IDictionary<string, ShapeRule> rules)
        {
            this.rules = rules;
            this.scopeStack.Push (new Scope ());
        }
    
        #region IShapeConfiguration implementation
    
        public IDictionary<string, ShapeRule> Rules {
            get { return this.rules; }
        }
        
        public IScope CurrentScope {
            get { return this.scopeStack.Peek (); }
        }
    
        public ShapeNode RootNode { get; private set; }
        
        public ShapeNode CurrentNode {
            set {
                this.currentNode = value;
                this.SetScope (new Scope (this.currentNode.Value.Transform));
            }
            get {
                return this.currentNode;
            }
        }
    
        public void AddGlobalArgs(IDictionary<string, string> args)
        {
            foreach (var arg in args) {
                this.globalArgs [arg.Key] = arg.Value;
            }
        }
        
        public void PushScope()
        {
            this.scopeStack.Push (new Scope (this.CurrentScope));
            this.log.Trace (string.Format ("PUSH: {0}", this.scopeStack.Peek ()));
        }
    
        public void PopScope()
        {
            this.log.Trace (string.Format ("POP: {0}", this.scopeStack.Peek ()));
            this.scopeStack.Pop ();
        }
    
        public void SetScope(IScope scope)
        {
            this.scopeStack.Clear ();
            this.scopeStack.Push (scope);
        }
    
		public void TranslateScope(Size x, Size y, Size z)
        {
			var delta = Size.ToVector3(x, y, z, this.CurrentScope.Transform.Scale);

            this.CurrentScope.Transform.Position += this.CurrentScope.Transform.Rotation * delta;
            this.log.Trace (string.Format ("TRANSFORMED: {0} {1}", delta, this.CurrentScope.Transform));
        }
        
        public void RotateScope(Vector3 delta)
        {
            this.CurrentScope.Transform.Rotation *= Quaternion.Euler (delta);
            this.log.Trace (string.Format ("ROTATED: {0} {1}", delta, this.CurrentScope.Transform));
        }
        
        public void ScaleScope(Size x, Size y, Size z)
        {
            var currentScale = this.CurrentScope.Transform.Scale;
    
            var newX = x.IsRelative ? currentScale.x * x.Value : x.Value;
            var newY = y.IsRelative ? currentScale.y * y.Value : y.Value;
            var newZ = z.IsRelative ? currentScale.z * z.Value : z.Value;
    
            this.CurrentScope.Transform.Scale = new Vector3 (newX, newY, newZ);
    
            //      var s = this.CurrentScope.Transform.Scale;
            //      s.Scale(scale);
            //      this.CurrentScope.Transform.Scale = s;
            this.log.Trace (string.Format ("SCALED: {0},{1},{2} {3}", x, y, z, this.CurrentScope.Transform));
        }
    
        public void AddRule(ShapeRule rule, IList<Argument> args)
        {
            this.log.Trace (string.Format ("RULE: {0}", rule));
            var node = this.NewNode (this.currentNode);
            node.Value.Rule = rule;
            node.Value.Args = args;
    
            this.AddNode (node);
        }
        
        public void AddVolume(string name, Argument[] cmdArgs)
        {
            this.log.Trace (string.Format ("VOLUME: {0}, {1}", name, this.CurrentScope.Transform));
    
            Volume vol;
    
            if (name.Equals ("Obj", StringComparison.InvariantCultureIgnoreCase)) {
                vol = new ObjVolume ();
            } else {
                var typeName = string.Format("Andoco.Unity.ProcArch.Shapes.Geometry.Volumes.{0}Volume", name);
                vol = (Volume)Activator.CreateInstance (Type.GetType(typeName, true, true));
            }
    
            vol.BuildVolume (cmdArgs);
    
            vol.ApplyTransform (this.CurrentScope.Transform);
    
            this.currentNode.Value.Volume = vol;
        }
    
        public void SplitDivideScope(string axis, Size[] sizes, ShapeSymbol[] shapes)
        {
            if (sizes.Length != shapes.Length)
                throw new System.ArgumentException ("The number of supplied shapes does not match the number of size arguments");
            
            var pos = this.CurrentScope.Transform.Position;
            var rot = this.CurrentScope.Transform.Rotation;
            var scale = this.CurrentScope.Transform.Scale;
    
            Func<Vector3, Vector3> startPosAction;
            Func<Vector3, float> axisScaleFunc;
            Func<float, Vector3> deltaAction;
            Func<float, Vector3> newScaleAction;
    
            switch (axis.ToUpper ()) {
            case "X":
                axisScaleFunc = (v) => v.x;
                startPosAction = (s) => new Vector3 (s.x / 2f, 0f, 0f);
                deltaAction = (s) => new Vector3 (s / 2f, 0f, 0f);
                newScaleAction = (s) => new Vector3 (s, scale.y, scale.z);
                break;
            case "Y":
                axisScaleFunc = (v) => v.y;
                startPosAction = (s) => new Vector3 (0f, s.y / 2f, 0f);
                deltaAction = (s) => new Vector3 (0f, s / 2f, 0f);
                newScaleAction = (s) => new Vector3 (scale.x, s, scale.z);
                break;
            case "Z":
                axisScaleFunc = (v) => v.z;
                startPosAction = (s) => new Vector3 (0f, 0f, s.z / 2f);
                deltaAction = (s) => new Vector3 (0f, 0f, s / 2f);
                newScaleAction = (s) => new Vector3 (scale.x, scale.y, s);
                break;
            default:
                throw new ArgumentException (string.Format ("Unsupported subdivision axis \"{0}\"", axis), "axis");
            }
    
            // Start at one end of the selected scope axis.
            var startPos = pos - (rot * startPosAction (scale));
    
            // Calculate total relative and absolute sizes supplied.
            float totalRelSize = Size.SumRelative (sizes);
            float totalAbsSize = Size.SumAbsolute (sizes);
    
            for (int i=0; i < sizes.Length; i++) {
                float size;
    
                // Calculate size of segment based on it being absolute or relative.
                if (sizes [i].IsRelative) {
                    var relativeSize = sizes [i].Value;
                    size = relativeSize * (axisScaleFunc (scale) - totalAbsSize) / totalRelSize;
                } else {
                    size = sizes [i].Value;
                }
    
                var delta = rot * deltaAction (size);
    
                var newScale = newScaleAction (size);
                var newPos = startPos + delta;
    
                var node = this.NewNode (this.currentNode);
                node.Value.Rule = this.rules [shapes [i].Name];
                node.Value.Transform = new SimpleTransform (newPos, rot, newScale);
                node.Value.Args = this.ResolveArgs (shapes [i].UnresolvedArgs).ToList (); //shapes[i].ResolvedArgs.ToList();
                this.AddNode (node);
    
                startPos += delta * 2f; // Move to the end of the current segment.
            }
        }
    
        public void SplitComponent(string query, ShapeSymbol symbol)
        {
            this.log.Trace (string.Format ("COMP: {0}, {1}", query, this.CurrentScope.Transform));
    
            var currentVol = currentNode.Value.Volume;
            var components = currentVol.Query (query);
    
            foreach (var cmp in components) {
                // Get the correct position of the component based on the current scope and volume.
                var newPos = this.CurrentScope.Transform.Position + (this.CurrentScope.Transform.Rotation * Vector3.Scale (currentVol.Transform.Scale, cmp.Transform.Position));
    
                // Get the correct rotation of the component based on the current scope.
                var newRot = this.CurrentScope.Transform.Rotation * cmp.Transform.Rotation;
    
                // Rotate the volume's scale by the rotation of the component so that we can apply the correct scale to the component.
                // Rotating a scale vector can result in negative values, so we need to make sure that they are all positive.
                //          var s = currentVol.Transform.Scale;
                //          s = cmp.Rotation * s;
                //          s = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
                //          var newScale = s;
                //          var newScale = cmp.AxisMap(currentVol.Transform.Scale);
                var newScale = cmp.AxisMap (this.CurrentScope.Transform.Scale);
                newScale = Vector3.Scale (newScale, cmp.Transform.Scale);
                
                var newTrans = new SimpleTransform (newPos, newRot, newScale);
    
                this.log.Trace (string.Format ("**** {0}", newTrans));
    
                var node = this.NewNode (this.currentNode);
                node.Value.Transform = newTrans;
                node.Value.Rule = this.rules [symbol.Name];
                node.Value.Args = this.ResolveArgs (symbol.UnresolvedArgs).ToList (); //symbol.ResolvedArgs.ToList();
                
                this.AddNode (node);
            }
        }
    
        public Argument[] ResolveArgs(IEnumerable<Argument> unresolvedArgs)
        {
            var resolvedArgs = new List<Argument> ();
            
            foreach (var arg in unresolvedArgs) {
                if (this.CurrentNode.Value.Rule.ArgNames.Contains (arg.Value)) {
                    // Variable argument value.
                    var argIndex = this.CurrentNode.Value.Rule.ArgNames.IndexOf (arg.Value);
                    var argVal = this.CurrentNode.Value.Args [argIndex];
                    resolvedArgs.Add (new Argument (arg.Name, argVal.Value));
                } else if (this.globalArgs.ContainsKey (arg.Value)) {
                    resolvedArgs.Add (new Argument (arg.Name, this.globalArgs [arg.Value]));
                } else {
                    // Literal argument value.
                    resolvedArgs.Add (new Argument (arg.Name, arg.Value));
                }
            }
            
            return resolvedArgs.ToArray ();
        }
        
        #endregion
    
        #region Private methods
    
        private string NextNodeId()
        {
            return (this.counter++).ToString ();
        }
    
        private ShapeNode NewNode(ShapeNode parent)
        {
            var node = new ShapeNode (this.NextNodeId (), parent)
            {
                Value = new ShapeNodeValue
                {
                    Transform = new SimpleTransform(this.CurrentScope.Transform)
                }
            };
    
            return node;
        }
    
        private void AddNode(ShapeNode node)
        {
            if (this.RootNode == null) {
                this.RootNode = node;
            } else {
                if (this.currentNode != node.Parent)
                    throw new InvalidOperationException ("The parent of the node is not the current node");
                this.currentNode.ConnectTo (node);
            }
        }
    
        #endregion
    }
}