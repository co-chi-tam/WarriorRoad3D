using System;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	[Serializable]
	public class CFSMComponent : CComponent {

		[SerializeField]	protected TextAsset m_FSMJsonText;
		[SerializeField]	protected string m_CurrentState;

		protected FSMManager m_FSMManager;

		public new void Init(ISimpleContext context) {
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_FSMJsonText.text);
			this.m_FSMManager.RegisterState ("IdleState", 	new FSMIdleState(context));
			this.m_FSMManager.RegisterState ("ActiveState", new FSMActiveState(context));
			this.m_FSMManager.RegisterState ("InactiveState", 	new FSMInactiveState(context));
			// Character
			this.m_FSMManager.RegisterState ("CharacterIdleState", new FSMCharacterIdleState(context));
			this.m_FSMManager.RegisterState ("CharacterActiveState", new FSMCharacterActiveState(context));

			this.m_FSMManager.RegisterCondition ("IsActive", 	context.IsActive);
			this.m_FSMManager.RegisterCondition ("IsMoveToTargetBlock", context.IsMoveToTargetBlock);
		}

		public override void UpdateComponent (float dt)
		{
			base.UpdateComponent (dt);
			this.m_FSMManager.UpdateState (dt);
			this.m_CurrentState = this.m_FSMManager.currentStateName;
		}

	}
}