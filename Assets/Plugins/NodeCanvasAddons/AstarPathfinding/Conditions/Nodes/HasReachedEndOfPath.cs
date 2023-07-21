using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions {

	[Description("Check if agent has reached last point in path")]
	public class HasReachedEndOfPath : ConditionTask {

		[RequiredField]
		[BlackboardOnly]
		public BBParameter<Path> Path;
		[RequiredField]
		public BBParameter<float> AcceptableDistance = new BBParameter<float>{value = 0.1f};

		protected override bool OnCheck()
		{
			if (Path == null || Path.isNone || Path.isNull)
			{
				Debug.LogWarning(string.Format("Path object: {0} is not set", Path));
				return false;
			}
			
			var lastIndex = Path.value.vectorPath.Count - 1;
			if (Path.value.vectorPath.Count == 0)
				return true;
		
			var lastWaypoint = Path.value.vectorPath[lastIndex];
			var distance = Vector2.Distance(lastWaypoint, agent.transform.position);
			return distance <= AcceptableDistance.value;
		}
	}
}