using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMCharacterActiveState : FSMActiveState
	{

		protected CCharacterController m_Controller;

		public FSMCharacterActiveState(IContext context) : base (context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState()
		{
			base.StartState ();
			this.m_Controller.InvokeAction ("StartActiveState");
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
			this.m_Controller.UpdateAction (dt);
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}
