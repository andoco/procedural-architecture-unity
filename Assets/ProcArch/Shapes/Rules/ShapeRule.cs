using System.Collections.Generic;
using System.Linq;

public sealed class ShapeRule
{
	public ShapeRule()
	{
		this.Successors = new List<SuccessorList>();
	}

	public string Symbol { get; set; }

	public IList<string> ArgNames { get; set; }

	public string GuardCondition { get; set; }

	public IList<SuccessorList> Successors { get; private set; }

	public IEnumerable<IShapeSuccessor> PickSuccessors()
	{
		SuccessorList chosenSet = null;
		
		if (this.Successors.Count > 1)
		{
			var total = this.Successors.Sum(x => x.Probability);
			
			if (total != 1f)
				throw new System.InvalidOperationException("Sum of probabilities should equal 1");
			
			var weightedPick = UnityEngine.Random.value;
			foreach (var set in this.Successors)
			{
				if (weightedPick < set.Probability)
				{
					chosenSet = set;
					break;
				}
				weightedPick -= set.Probability;
			}
		}
		else
		{
			chosenSet = this.Successors.SingleOrDefault();
		}

		if (chosenSet == null)
			throw new System.InvalidOperationException("Failed to pick a SuccessorSet");

		return chosenSet.Successors;
	}

	public override string ToString ()
	{
		return string.Format ("[ShapeRule: Symbol={0}, Args={1}, GuardCondition={1}, Successors={2}]", Symbol, FormatArgs(), GuardCondition, FormatSuccessors());
	}

	private string FormatArgs()
	{
		return string.Format("[{0}]", string.Join(",", this.ArgNames.ToArray()));
	}

	private string FormatSuccessors()
	{
		return this.Successors.Count.ToString();
	}
}