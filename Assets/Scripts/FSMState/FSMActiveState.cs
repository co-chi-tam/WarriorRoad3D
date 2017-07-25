using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMActiveState : FSMBaseState
	{
		public FSMActiveState(IContext context) : base (context)
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
