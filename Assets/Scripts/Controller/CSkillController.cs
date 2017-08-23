using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CSkillController : CObjectController, ISimpleContext {

		#region Fields

		[Header("Data")]
		[SerializeField]	protected CSkillData m_SkillData;

		[Header ("Control")]
		[SerializeField]	protected CObjectController m_TargetEnemy;

		[Header("Component")]
		[SerializeField]	protected CSimpleEffectComponent m_EffectComponent;
		[SerializeField]	protected CFSMComponent m_FSMComponent;

		protected ISimpleStatusContext m_Owner;
		protected CObjectController m_OwnerController;

		protected float m_SkillTime = 0f;

		#endregion

		#region Implementation Monobehaviour

		protected override void Awake ()
		{
			base.Awake ();
			this.m_FSMComponent.Init (this);
			this.m_EffectComponent.Init (this.m_SkillData, this.m_Owner);
		}

		#endregion

		#region Implementation Controller

		protected override void RegisterComponent ()
		{
			base.RegisterComponent ();
			this.m_ListComponents.Add (this.m_EffectComponent);
			this.m_ListComponents.Add (this.m_FSMComponent);
		}

		public virtual void StartAction() {
			this.m_EffectComponent.ApplyEffect ();
		}

		#endregion

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

		public override bool IsActive ()
		{
			return base.IsActive () && this.m_Owner.GetActive ();
		}

		#endregion

		#region Getter && Setter

		public override void SetActive (bool value)
		{
			base.SetActive (value);
			this.m_FSMComponent.ActiveFSM (true);
			if (value && this.m_SkillData != null) {
				this.m_SkillTime = this.m_SkillData.skillTime;
				this.m_FSMComponent.StartFirstState ();
				this.m_EffectComponent.SetData (this.m_SkillData);
			}
			this.gameObject.SetActive (value);	
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
			this.m_OwnerController = value;
			this.m_EffectComponent.SetOwner (this.m_Owner);
		}

		public override CObjectController GetOwner() {
			return this.m_Owner.GetController() as CObjectController;
		}

		#endregion

	}
}
