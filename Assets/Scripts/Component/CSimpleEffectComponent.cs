using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CSimpleEffectComponent : CComponent {

		[SerializeField]	protected ISimpleStatusContext m_OwnerContext;
		[SerializeField]	protected ISimpleStatusContext m_TargetContext;
		[SerializeField]	protected CSkillData m_SkillData;

		protected Dictionary<string, Action<object>> m_SkillMethods;

		public new void Init(CSkillData data, ISimpleStatusContext owner) {
			base.Init ();
			this.m_SkillData = data;
			this.m_OwnerContext = owner;

			this.m_SkillMethods = new Dictionary <string, Action<object>> ();
			this.m_SkillMethods ["ApplyDamage"] = this.ApplyDamage;
		}

		public virtual void ApplyEffect() {
			for (int i = 0; i < this.m_SkillData.skillTriggers.Length; i++) {
				var skillTrigger = this.m_SkillData.skillTriggers [i];
				var name = skillTrigger.skillMethod;
				var value = skillTrigger.skillValue;
				if (this.m_SkillMethods.ContainsKey (name)) {
					this.m_SkillMethods [name] (value);
				}
			}
		}

		public virtual void ApplyDamage(object value) {
			if (this.m_OwnerContext == null
				|| this.m_TargetContext == null)
				return;
			var skillValue = (int)value;
			var currentDamage 	= this.m_OwnerContext.GetAttackPoint() + skillValue - this.m_TargetContext.GetDefendPoint ();
			currentDamage 		= Mathf.Clamp (currentDamage, 1, currentDamage);
			var currentHealth 	= this.m_TargetContext.GetHealth () - currentDamage;
			this.m_TargetContext.SetHealth (currentHealth);
		}

		public virtual void AttachTarget(ISimpleStatusContext value) {
			this.m_TargetContext = value;
		}

		public virtual ISimpleStatusContext GetTarget() {
			return this.m_TargetContext;
		}

	}
}
