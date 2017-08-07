using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CSimpleSkillSlotComponent : CComponent {

		[Header("Data")]
		[SerializeField]	protected CSkillData[] m_SkillSlot;

		protected ISimpleStatusContext m_Owner;
		protected float[] m_DelayTemps;
		protected CSkillController[] m_SkillSlotCtrls;

		public new void Init (ISimpleStatusContext owner, CSkillData[] slots)
		{
			base.Init ();
			this.m_Owner = owner;
			this.m_SkillSlot = slots;
			this.m_DelayTemps = new float[slots.Length];
			this.m_SkillSlotCtrls = new CSkillController[slots.Length];
			for (int i = 0; i < this.m_DelayTemps.Length; i++) {
				var skillData = slots [i];
				this.m_DelayTemps [i] = skillData.skillDelay;
				this.m_SkillSlotCtrls[i] = GameObject.Instantiate (
					Resources.Load <CSkillController> ("ObjectPrefabs/" + skillData.objectModel));
				this.m_SkillSlotCtrls [i].SetData (skillData);
			}
		}

		public override void UpdateComponent (float dt)
		{
			base.UpdateComponent (dt);
			if (this.m_DelayTemps == null)
				return;
			for (int i = 0; i < this.m_DelayTemps.Length; i++) {
				if (this.m_DelayTemps [i] <= 0f)
					continue;
				this.m_DelayTemps [i] -= dt;
			}
		}

		public virtual void ActiveSkillSlot(int slot, params ISimpleStatusContext[] targets) {
			if (slot < 0
				|| slot > this.m_SkillSlot.Length - 1)
				return;
			if (this.m_DelayTemps [slot] > 0f)
				return;
			var skillData = this.m_SkillSlot [slot];
			var skillDelay = skillData.skillDelay;
			this.m_DelayTemps [slot] = skillDelay;
			this.m_SkillSlotCtrls[slot].SetOwner (this.m_Owner.GetController() as CObjectController);
			this.m_SkillSlotCtrls[slot].SetTargetEnemy (targets[0].GetController() as CObjectController);
			this.m_SkillSlotCtrls[slot].SetActive (true);
		}
		
	}
}
