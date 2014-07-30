using UnityEngine;
using System.Collections.Generic;

public interface IStyleConfig
{
	object GetStyle(string section, string key);

	object GetStyleOrDefault(string section, string key, object defaultValue);
}
