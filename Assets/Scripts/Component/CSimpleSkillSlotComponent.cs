using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CSimpleSkillSlotComponent : CComponent {

		#region Fields

		[Header("Data")]
		[SerializeField]	protected CSkillData[] m_SkillSlot;

		protected ISimpleStatusContext m_Owner;
		protected CObjectController m_OwnerController;
		protected float[] m_DelayTemps;
		protected int m_DefaultNormalAttack = 0;

		#endregion

		#region Implemenation Component

		public new void Init (ISimpleStatusContext owner, CSkillData[] slots, int defaultSkill = 0)
		{
			base.Init ();
			this.m_Owner = owner;
			this.m_OwnerController = owner.GetController () as CObjectController;
			this.m_SkillSlot = new CSkillData[slots.Length];
			Array.Copy (slots, this.m_SkillSlot, slots.Length);
			this.m_DefaultNormalAttack = defaultSkill;
			this.m_DelayTemps = new float[slots.Length];
			this.m_DelayTemps [defaultSkill] = 0.1f;
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

		#endregion

		#region Main methods

		public virtual void ActiveSkillSlot(int slot, params ISimpleStatusContext[] targets) {
			if (slot < 0
				|| slot > this.m_SkillSlot.Length - 1)
				return;
			if (this.m_DelayTemps [slot] > 0f)
				slot = this.m_DefaultNormalAttack;
			CHandleEvent.Instance.AddEvent (HandleActiveSkillSlot (slot, targets));
		}

		protected virtual IEnumerator HandleActiveSkillSlot(int slot, params ISimpleStatusContext[] targets) {
			var skillData = this.m_SkillSlot [slot];
			var skillDelay = skillData.skillDelay;
			var skillCtrl = CObjectPoolManager.Get<CSkillController> (skillData.objectName);
			while (skillCtrl == null) {
				try {
					skillCtrl = GameObject.Instantiate (Resources.Load <CSkillController> ("ObjectPrefabs/" + skillData.objectModel));
					CObjectPoolManager.Set <CSkillController> (skillData.objectName, skillCtrl);
				} catch (Exception ex) {
					throw new Exception (ex.Message);
				}
				yield return skillCtrl != null;
				skillCtrl = CObjectPoolManager.Get<CSkillController> (skillData.objectName);
			}
			var targetCtrl = targets [0].GetController () as CObjectController;
			skillCtrl.SetData (skillData);
			skillCtrl.SetOwner (this.m_OwnerController);
			skillCtrl.SetTargetEnemy (targetCtrl);
			skillCtrl.SetActive (true);
			skillCtrl.SetPosition (targetCtrl.GetPosition());
			skillCtrl.SetRotation (this.m_Owner.GetPosition());
			this.m_DelayTemps [slot] = skillDelay;
		}

		#endregion

	}
}
