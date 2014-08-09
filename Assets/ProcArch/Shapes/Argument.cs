namespace Andoco.Unity.ProcArch.Shapes
{
    public class Argument
    {
        public Argument (string name, string value)
        {
            this.Name = name;
            this.Value = TrimArg (value);
            this.IsNamed = this.Name != null;
        }
    
        public Argument (string value)
            : this(null, value)
        {
        }
    
        public string Name { get; private set; }
    
        public string Value { get; private set; }
    
        public bool IsNamed { get; private set; }
    
        public override string ToString ()
        {
            return string.Format ("[Argument: Name={0}, Value={1}, IsNamed={2}]", Name, Value, IsNamed);
        }
    
        private static string TrimArg (string arg)
        {
            return arg == null ? null : arg.Trim('"');
        }
    }
}