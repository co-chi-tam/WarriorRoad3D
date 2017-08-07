using System;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	[Serializable]
	public class CFSMComponent : CComponent {

		[SerializeField]	protected TextAsset m_FSMJsonText;
		[SerializeField]	protected string m_CurrentState;

		protected FSMManager m_FSMManager;
		protected bool m_FSMAlready = false;

		public new void Init(ISimpleContext context) {
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_FSMJsonText.text);
			// Common
			this.m_FSMManager.RegisterState ("IdleState", 				new FSMIdleState(context));
			this.m_FSMManager.RegisterState ("ActiveState", 			new FSMActiveState(context));
			this.m_FSMManager.RegisterState ("InactiveState", 			new FSMInactiveState(context));
			// Character
			this.m_FSMManager.RegisterState ("CharacterIdleState", 		new FSMCharacterIdleState(context));
			this.m_FSMManager.RegisterState ("CharacterActiveState", 	new FSMCharacterActiveState(context));
			this.m_FSMManager.RegisterState ("CharacterAttackState", 	new FSMCharacterAttackState(context));
			this.m_FSMManager.RegisterState ("CharacterInactiveState", 	new FSMCharacterInactiveState(context));
			// Skill
			this.m_FSMManager.RegisterState ("SkillIdleState", 			new FSMSkillIdleState(context));
			this.m_FSMManager.RegisterState ("SkillActiveState", 		new FSMSkillActiveState(context));
			this.m_FSMManager.RegisterState ("SkillInactiveState", 		new FSMSkillInactiveState(context));
			// Character
			this.m_FSMManager.RegisterCondition ("IsActive", 			context.IsActive);
			this.m_FSMManager.RegisterCondition ("IsMoveToTargetBlock", context.IsMoveToTargetBlock);
			this.m_FSMManager.RegisterCondition ("HaveEnemy", 			context.HaveEnemy);
			// Skill
			this.m_FSMManager.RegisterCondition ("IsActionCompleted",	context.IsActionCompleted);
		}

		public override void UpdateComponent (float dt)
		{
			if (this.m_FSMAlready == false)
				return;
			base.UpdateComponent (dt);
			this.m_FSMManager.UpdateState (dt);
			this.m_CurrentState = this.m_FSMManager.currentStateName;
		}

		public virtual void StartFirstState() {
			var firstState = this.m_FSMManager.firstStateName;
			this.m_FSMManager.SetState (firstState);
		}

		public virtual void ActiveFSM(bool value) {
			this.m_FSMAlready = value;
		}

	}
}