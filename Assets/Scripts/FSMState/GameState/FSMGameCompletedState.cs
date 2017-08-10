using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMGameCompletedState : FSMBaseState
	{

		protected CGameManager m_GameManager;

		public FSMGameCompletedState(IContext context) : base (context)
		{
			this.m_GameManager = context as CGameManager;
		}

		public override void StartState()
		{
			base.StartState ();
			this.m_GameManager.OnCompleteGame ();
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
