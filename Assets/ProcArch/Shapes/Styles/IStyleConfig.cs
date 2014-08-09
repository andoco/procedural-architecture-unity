namespace Andoco.Unity.ProcArch.Shapes.Styles
{
    using UnityEngine;
    using System.Collections.Generic;
    
    public interface IStyleConfig
    {
        //  object GetStyle(string section, string key);
        //
        //  object GetStyleOrDefault(string section, string key, object defaultValue);
    
        object GetStyle (string style, string theme, string key);
        
        //  object GetStyleOrDefault(string style, string theme, string key, object defaultValue);
    }
}
