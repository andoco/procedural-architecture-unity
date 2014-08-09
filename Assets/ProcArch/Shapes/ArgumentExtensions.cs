namespace Andoco.Unity.ProcArch.Shapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public static class ArgumentExtensions
    {
        public static T GetArgOrDefault<T> (this IEnumerable<Argument> args, string name, T defaultValue)
        {
            var arg = args.SingleOrDefault (x => x.Name != null && x.Name.Equals (name, StringComparison.InvariantCultureIgnoreCase));
    
            T result;
            if (arg == null) {
                result = defaultValue;
            } else {
                result = (T)Convert.ChangeType (arg.Value, typeof(T));
            }
    
            return result;
        }
    }
}