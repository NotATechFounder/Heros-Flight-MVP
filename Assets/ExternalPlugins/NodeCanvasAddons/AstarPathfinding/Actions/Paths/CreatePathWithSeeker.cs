using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;
using UnityEngine;


namespace NodeCanvasAddons.AStarPathfinding
{
	[Category("A* Pathfinding/Paths")]
	[Name("Create Path With Seeker")]
	[Description("Creates a path from the current agent to the destination point taking GUO into consideration")]
	[ParadoxNotion.Design.Icon("PathfindingPath")]
	public class CreatePathWithSeeker : ActionTask
	{
		[RequiredField]
		public BBParameter<Vector3> DestinationPosition;

		[BlackboardOnly]
		public BBParameter<Path> OutputPath = new BBParameter<Path>();

		Seeker m_Seaker;

		protected override string info
		{
			get { return string.Format("Creating basic path \nas {0}", OutputPath); }
		}

		protected override string OnInit()
		{
			m_Seaker = ownerSystemAgent.GetComponent<Seeker>();
			return base.OnInit();
		}

		protected override void OnExecute()
		{
			m_Seaker.StartPath(agent.transform.position, DestinationPosition.value, PathFinishedDelegate);
		}

		private void PathFinishedDelegate(Path path)
		{
			OutputPath.value = path;
			EndAction(!path.error);
		}
	}
}