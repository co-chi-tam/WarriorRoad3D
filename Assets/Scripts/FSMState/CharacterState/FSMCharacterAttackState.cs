using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMCharacterAttackState : FSMIdleState
	{

		protected CCharacterController m_Controller;

		public FSMCharacterAttackState(IContext context) : base (context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState()
		{
			base.StartState ();
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
