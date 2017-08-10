using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMGameEndState : FSMBaseState
	{

		protected CGameManager m_GameManager;

		public FSMGameEndState(IContext context) : base (context)
		{
			this.m_GameManager = context as CGameManager;
		}

		public override void StartState()
		{
			base.StartState ();
			this.m_GameManager.OnEndGame ();
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
