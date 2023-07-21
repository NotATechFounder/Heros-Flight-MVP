using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions {

	[Description("generates random direction left or right")]
	public class GetDirection2D : ActionTask {
		
		[BlackboardOnly]
		public BBParameter<float> AcceptedDistance = new BBParameter<float>();
		[BlackboardOnly]
		public BBParameter<Transform> Target = new BBParameter<Transform>();
		[BlackboardOnly]
		public BBParameter<Vector2> Direction = new BBParameter<Vector2>();

		Vector2 lastDirection;

		protected override void OnExecute()
		{
			var targetPosition = Target.value.position;
			var agentPosition = agent.transform.position;
			if (Vector2.Distance(agentPosition, targetPosition) > AcceptedDistance.value)
			{
				var random = Random.Range(0, 2);
				Direction.value = random == 0 ? Vector2.left : Vector2.right;
			}
			else
			{
				Direction.value = targetPosition.x < agentPosition.x ? Vector2.left : Vector2.right;
			}
			
		
			EndAction(true);
		}
	}
}