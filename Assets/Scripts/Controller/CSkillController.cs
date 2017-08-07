using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CSkillController : CObjectController, ISimpleContext {

		[Header("Data")]
		[SerializeField]	protected CSkillData m_SkillData;

		[Header ("Control")]
		[SerializeField]	protected CObjectController m_TargetEnemy;

		[Header("Component")]
		[SerializeField]	protected CSimpleEffectComponent m_EffectComponent;
		[SerializeField]	protected CFSMComponent m_FSMComponent;

		protected ISimpleStatusContext m_Owner;

		protected float m_SkillTime = 0f;

		protected override void Awake ()
		{
			base.Awake ();
			this.m_FSMComponent.Init (this);
		}

		protected override void Update ()
		{
			base.Update ();
			var dt = Time.deltaTime;
			this.m_SkillTime -= dt;
		}

		protected override void RegisterComponent ()
		{
			base.RegisterComponent ();
			this.m_ListComponents.Add (this.m_EffectComponent);
			this.m_ListComponents.Add (this.m_FSMComponent);
		}

		public virtual void StartAction() {
			this.m_EffectComponent.ApplyEffect ();
		}

		#region FSM

		public virtual bool IsMoveToTargetBlock() {
			return true;
		}

		public virtual bool HaveEnemy() {
			return this.m_TargetEnemy != null 
				&& this.m_TargetEnemy.GetActive();
		}

		public virtual bool IsActionCompleted() {
			this.m_SkillTime -= Time.deltaTime;
			return this.m_SkillTime <= 0f;
		}

		#endregion

		public override void SetActive (bool value)
		{
			base.SetActive (value);
			this.m_FSMComponent.ActiveFSM (true);
			if (value && this.m_SkillData != null) {
				this.m_SkillTime = this.m_SkillData.skillTime;
				this.m_EffectComponent.Init (this.m_SkillData, this.m_Owner);
				this.m_FSMComponent.StartFirstState ();
			}
		}

		public override void SetData (CObjectData value)
		{
			base.SetData (value);
			this.m_SkillData = value as CSkillData;
		}

		public override CObjectData GetData ()
		{
			return this.m_SkillData as CSkillData;
		}

		public override void SetTargetEnemy (CObjectController target)
		{
			base.SetTargetEnemy (target);
			this.m_TargetEnemy = target;
			var simpleStatus = target as ISimpleStatusContext;
			if (simpleStatus != null) {
				this.m_EffectComponent.AttachTarget (simpleStatus);
			}
		}

		public override CObjectController GetTargetEnemy ()
		{
			return this.m_TargetEnemy;
		}

		public override void SetOwner(CObjectController value) {
			this.m_Owner = value as ISimpleStatusContext;
		}

		public virtual CObjectController GetOwner() {
			return this.m_Owner.GetController() as CObjectController;
		}

	}
}
