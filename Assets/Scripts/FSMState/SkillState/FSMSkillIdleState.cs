using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMSkillIdleState : FSMIdleState
	{
		protected CSkillController m_Controller;

		public FSMSkillIdleState(IContext context) : base (context)
		{
			this.m_Controller = context as CSkillController;
		}

		public override void StartState()
		{
			base.StartState ();
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
