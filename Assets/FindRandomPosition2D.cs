using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions {

	public class FindRandomPosition2D : ActionTask {

		
		[RequiredField]
		public BBParameter<float> MaxDistance;
    
		[RequiredField]
		public BBParameter<bool> ShoulduseYAxis;

		[BlackboardOnly]
		public BBParameter<Vector3> WanderPosition = new BBParameter<Vector3>();

		protected override void OnExecute()
		{
			var currentPosition = agent.transform.position;
			var point = Random.insideUnitSphere * MaxDistance.value;
			if (!ShoulduseYAxis.value)
			{
				point.y = currentPosition.y;
			}
			point.z = 0;
			WanderPosition.value = point;
			EndAction(true);
		}
	}
}