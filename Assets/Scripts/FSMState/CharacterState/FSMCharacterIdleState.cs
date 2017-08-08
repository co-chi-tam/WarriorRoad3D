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
			this.m_Controller.InvokeAction ("StartIdleState");
			this.m_Controller.SetAnimation ("AnimParam", 0);
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
