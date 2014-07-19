using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

public interface IScope
{
//	Matrix4x4 Matrix { get; set; }

	SimpleTransform Transform { get; set; }
	
	Volume Volume { get; set; }
}
