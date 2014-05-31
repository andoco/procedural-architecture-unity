using System.Text;
using UnityEngine;
using Irony.Parsing;

public interface IShapeProductionSystemBuilder
{
	IShapeProductionSystem Build(string source);
}
