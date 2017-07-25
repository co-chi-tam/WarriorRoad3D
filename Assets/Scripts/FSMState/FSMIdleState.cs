using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMIdleState : FSMBaseState
	{
		public FSMIdleState(IContext context) : base (context)
		{

		}

		public override void StartState()
		{
			base.StartState ();
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}
