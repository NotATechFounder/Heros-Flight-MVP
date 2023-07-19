using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions {

	public class CheckDistanceToTarget : ConditionTask<Transform> {

		[RequiredField]
		public BBParameter<Vector3> Waypoint;

		[RequiredField]
		public BBParameter<float> AcceptableDistance = new BBParameter<float>{value = 0.1f};

		protected override string info
		{
			get { return string.Format("reached {0}?", Waypoint); }
		}
		
		protected override bool OnCheck()
		{
			var distance = Vector3.Distance(Waypoint.value, agent.transform.position);
			return distance <= AcceptableDistance.value;
		}
	}
}