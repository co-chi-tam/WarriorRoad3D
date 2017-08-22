using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMSkillActiveState : FSMIdleState
	{

		protected CSkillController m_Controller;

		public FSMSkillActiveState(IContext context) : base (context)
		{
			this.m_Controller = context as CSkillController;
		}

		public override void StartState()
		{
			base.StartState ();
			if (this.m_Controller != null) {
				var target = this.m_Controller.GetTargetEnemy () as CCharacterController;
				if (target != null 
					&& target.GetHealth() > 0) {
					this.m_Controller.SetPosition (target.GetPosition ());
					this.m_Controller.StartAction ();
				}
			}
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
