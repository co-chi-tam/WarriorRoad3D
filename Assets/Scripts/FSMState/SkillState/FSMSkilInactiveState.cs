using UnityEngine;
using System.Collections;
using FSM;

namespace WarriorRoad {
	public class FSMSkillInactiveState : FSMIdleState
	{

		protected CSkillController m_Controller;

		public FSMSkillInactiveState(IContext context) : base (context)
		{
			this.m_Controller = context as CSkillController;
		}

		public override void StartState()
		{
			base.StartState ();
			var skillData = this.m_Controller.GetData () as CSkillData;
			CObjectPoolManager.Set <CSkillController> (skillData.objectName, this.m_Controller);
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
