using UnityEngine;
using System.Collections.Generic;

public interface IStyleConfig
{
	T GetStyle<T>(string section, string key);

	T GetStyleOrDefault<T>(string section, string key, T defaultValue);
}
