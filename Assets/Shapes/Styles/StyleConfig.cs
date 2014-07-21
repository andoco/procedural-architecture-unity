using UnityEngine;
using System.Collections.Generic;

public interface IStyleConfig
{
	IDictionary<string, object> GetByName(string style);
}

public class StyleConfig : IStyleConfig
{
	private readonly IDictionary<string, IDictionary<string, object>> styles;

	public const string DefaultStyle = "default";

	public StyleConfig(IDictionary<string, IDictionary<string, object>> styles)
	{
		this.styles = styles;
	}

	public IDictionary<string, object> GetByName(string style)
	{
		return this.styles[style];
	}
}