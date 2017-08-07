using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMCharacterInactiveState : FSMBaseState
	{
		
		protected CCharacterController m_Controller;

		public FSMCharacterInactiveState(IContext context) : base (context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState()
		{
			base.StartState ();
			this.m_Controller.SetActive (false);
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
