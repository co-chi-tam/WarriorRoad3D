using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMGameActiveState : FSMBaseState
	{

		protected CGameManager m_GameManager;

		public FSMGameActiveState(IContext context) : base (context)
		{
			this.m_GameManager = context as CGameManager;
		}

		public override void StartState()
		{
			base.StartState ();
		}

		public override void UpdateState(float dt)
		{
			base.UpdateState (dt);
			this.m_GameManager.OnUpdateGame ();
		}

		public override void ExitState()
		{
			base.ExitState ();
		}
	}
}
