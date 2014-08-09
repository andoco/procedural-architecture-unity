namespace Andoco.Unity.ProcArch.Shapes.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Unity.ProcArch.Shapes.Successors;
    
    public class SuccessorList
    {
        public SuccessorList ()
        {
            this.Successors = new List<IShapeSuccessor> ();
        }
    
        public float Probability { get; set; }
    
        public IList<IShapeSuccessor> Successors { get; set; }
    
        public override string ToString ()
        {
            return string.Format ("[SuccessorList: Probability={0}, Successors={1}]", Probability, Successors.Count);
        }
    }
}
