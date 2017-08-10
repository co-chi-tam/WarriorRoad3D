using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMGameIdleState : FSMBaseState
	{

		protected CGameManager m_GameManager;

		public FSMGameIdleState(IContext context) : base (context)
		{
			this.m_GameManager = context as CGameManager;
		}

		public override void StartState()
		{
			base.StartState ();
			this.m_GameManager.OnLoadingCompleted ();
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
