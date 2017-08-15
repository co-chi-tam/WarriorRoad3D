using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMCharacterIdleState : FSMIdleState
	{

		protected CCharacterController m_Controller;

		public FSMCharacterIdleState(IContext context) : base (context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState()
		{
			base.StartState ();
			this.m_Controller.StartIdle ();
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
