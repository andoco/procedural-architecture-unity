namespace Andoco.Unity.ProcArch.Shapes.Guards
{
    public class FakeShapeRuleGuardEvaluator : IShapeRuleGuardEvaluator
    {
        public bool Evaluate (string expression)
        {
            return true;
        }
    }
}
