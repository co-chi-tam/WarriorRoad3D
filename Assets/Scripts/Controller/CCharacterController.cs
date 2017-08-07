using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	public class CCharacterController : CObjectController, ISimpleContext, ISimpleStatusContext {

		[Header ("Control")]
		[SerializeField]	protected Animator m_Animator;

		[Header ("Block")]
		public CBlockController currentBlock;

		[Header ("Data")]
		[SerializeField]	protected CCharacterData m_CharacterData;

		[Header ("Component")]
		[SerializeField]	protected CFSMComponent m_FSMComponent;

		protected CMapManager m_MapManager;

		protected override void Awake ()
		{
			base.Awake ();
			this.m_FSMComponent.Init (this);
		}

		protected override void Start() {
			base.Start ();
			this.SetAnimation ("AnimParam", 20);
			this.m_MapManager = CMapManager.GetInstance ();
		}

		protected override void RegisterComponent ()
		{
			base.RegisterComponent ();
			this.m_ListComponents.Add (this.m_FSMComponent);
		}

		#region FSM

		public override bool IsActive() {
			base.IsActive ();
			return this.GetActive () && this.GetHealth () > 0f;
		}

		public virtual bool IsMoveToTargetBlock() {
			return this.currentBlock != null;
		}

		public virtual bool HaveEnemy() {
			return false;
		}

		public virtual bool IsActionCompleted() {
			return false;
		}

		#endregion

		#region Control

		public virtual void StartIdle() {
			
		}

		public virtual void UpdateAction(float dt) {
			
		}

		public virtual void UpdateAttackAction (float dt) {
		
		}

		public virtual void ApplySkill (string name, object[] values) {
			
		}

		#endregion

		#region Getter && Setter

		public override void SetData (CObjectData value)
		{
			base.SetData (value);
			this.m_CharacterData = value as CCharacterData;
			this.m_FSMComponent.ActiveFSM (true);
		}

		public override CObjectData GetData ()
		{
			return this.m_CharacterData as CObjectData;
		}

		public virtual void SetJumpCurve(float time) {
			
		}

		public override void SetAnimation (string name, object param)
		{
			base.SetAnimation (name, param);
			if (this.m_Animator == null)
				return;
			if (param is int) {
				this.m_Animator.SetInteger (name, (int)param);
			} else if (param is bool) {
				this.m_Animator.SetBool (name, (bool)param);
			} else if (param is float) {
				this.m_Animator.SetFloat (name, (float)param);
			} else if (param == null) {
				this.m_Animator.SetTrigger (name);
			}
		}

		public virtual void SetStep (int value)
		{
			this.m_CharacterData.characterStep = value;
		}

		public virtual int GetStep ()
		{
			return this.m_CharacterData.characterStep;
		}

		public void SetHealth (int value)
		{
			var totalHealth = value <= 0 
				? 0 
				: value >= this.m_CharacterData.characterMaxHealthPoint 
				? this.m_CharacterData.characterMaxHealthPoint 
				: value;
			
			totalHealth = totalHealth > this.m_CharacterData.maxHealthPoint 
				? this.m_CharacterData.maxHealthPoint 
				: totalHealth;
			
			this.m_CharacterData.characterHealthPoint = totalHealth;
		}

		public int GetHealth ()
		{
			return this.m_CharacterData.characterHealthPoint;
		}

		public int GetMaxHealth ()
		{
			return this.m_CharacterData.characterMaxHealthPoint;
		}

		public void SetAttackPoint (int value)
		{
			var totalPoint = value <= 0 
				? 0 
				: value >= this.m_CharacterData.maxAttackPoint 
				? this.m_CharacterData.maxAttackPoint 
				: value;
			this.m_CharacterData.characterAttackPoint = totalPoint;
		}

		public int GetAttackPoint ()
		{
			return this.m_CharacterData.characterAttackPoint;
		}

		public void SetAttackSpeed (float value)
		{
			var totalPoint = value <= 0f 
				? 0f
				: value >= this.m_CharacterData.maxAttackSpeed 
				? this.m_CharacterData.maxAttackSpeed 
				: value;
			this.m_CharacterData.characterAttackSpeed = totalPoint;
		}

		public float GetAttackSpeed ()
		{
			return this.m_CharacterData.characterAttackSpeed;
		}

		public void SetDefendPoint (int value)
		{
			var totalPoint = value <= 0 
				? 0 
				: value >= this.m_CharacterData.maxDefendPoint 
				? this.m_CharacterData.maxDefendPoint 
				: value;
			this.m_CharacterData.characterDefendPoint = totalPoint;
		}

		public int GetDefendPoint ()
		{
			return this.m_CharacterData.characterDefendPoint;
		}

		public virtual object GetController() {
			return this;
		}

		#endregion
		
	}
}
