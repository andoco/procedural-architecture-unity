namespace Andoco.Unity.ProcArch.Shapes.Configuration
{
    public class Scope : IScope
    {
        public Scope ()
        {
            this.Transform = new SimpleTransform ();
        }
        
        public Scope (SimpleTransform transform)
        {
            this.Transform = new SimpleTransform (transform);
        }
    
        public Scope (IScope scope)
        {
            this.Transform = new SimpleTransform (scope.Transform);
        }
    
        public SimpleTransform Transform { get; set; }
    
        public override string ToString ()
        {
            return string.Format ("[Scope: Transform={0}]", Transform);
        }
    }
}
