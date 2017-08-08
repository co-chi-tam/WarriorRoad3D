using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMCharacterAttackState : FSMBaseState
	{

		protected CCharacterController m_Controller;

		public FSMCharacterAttackState(IContext context) : base (context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState()
		{
			base.StartState ();
			this.m_Controller.InvokeAction ("StartAttackState");
			this.m_Controller.SetAnimation ("AnimParam", 2);
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
			this.m_Controller.UpdateAttackAction (dt);
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}
