using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	public class CCharacterController : CObjectController, ISimpleContext, ISimpleStatusContext {

		[Header ("Control")]
		[SerializeField]	protected Animator m_Animator;
		[SerializeField]	protected CObjectController m_TargetEnemy;

		[Header ("Block")]
		public CBlockController currentBlock;
		public CBlockController targetBlock;

		[Header ("Data")]
		[SerializeField]	protected CCharacterData m_CharacterData;

		[Header ("Component")]
		[SerializeField]	protected CFSMComponent m_FSMComponent;
		[SerializeField]	protected CEventComponent m_EventComponent;
		[SerializeField]	protected CSimpleSkillSlotComponent m_SkillSlotComponent;

		protected CMapManager m_MapManager;
		protected float m_AttackDelay = 0f;

		protected override void Awake ()
		{
			base.Awake ();
			this.m_FSMComponent.Init (this);
			this.m_EventComponent.Init ();
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
			this.m_ListComponents.Add (this.m_EventComponent);
			this.m_ListComponents.Add (this.m_SkillSlotComponent);
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
			if (this.m_TargetEnemy == null)
				return false;
			return this.m_TargetEnemy.GetActive ();
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
			var target = this.m_TargetEnemy as CCharacterController;
			if (target != null) {
				target.SetTargetEnemy (this);
				this.SetTargetEnemy (target);
				if (this.m_AttackDelay < 0f) {
					this.m_AttackDelay = this.m_CharacterData.characterAttackSpeed;
					// TEST
					var random = UnityEngine.Random.Range (0, this.m_CharacterData.characterSkillSlots.Length);
					this.m_SkillSlotComponent.ActiveSkillSlot (random, target);
				} else {
					this.m_AttackDelay -= dt;
				}
				this.SetRotation (target.GetPosition());
			}
		}

		public virtual void ApplySkill (string name, object[] values) {
			
		}

		public virtual void InvokeAction(string name) {
			this.m_EventComponent.TriggerCallback (name);
		}

		public virtual void InvokeAction(string name, params object[] prams) {
			this.m_EventComponent.TriggerCallback (name, prams);
		}

		public virtual void AddAction(string name, System.Action callback) {
			this.m_EventComponent.AddCallback (name, (objs) => {
				if (callback != null) {
					callback();
				}
			});
		}

		public virtual void AddAction(string name, System.Action<object[]> callbacks) {
			this.m_EventComponent.AddCallback (name, callbacks);
		}

		#endregion

		#region Getter && Setter

		public override void SetData (CObjectData value)
		{
			base.SetData (value);
			this.m_CharacterData = value as CCharacterData;
			// BASE LEVEL
			var baseLevel = this.m_CharacterData.characterLevel - 1 < 0 
				? 0 
				: this.m_CharacterData.characterLevel - 1 >= this.m_CharacterData.maxLevel 
				? this.m_CharacterData.maxLevel 
				: this.m_CharacterData.characterLevel - 1;
			// ATTACK POINT BASE LEVEL
			this.m_CharacterData.characterAttackPoint += baseLevel * this.m_CharacterData.dataPerLevel.characterAttackPoint;
			// ATTACK SPEED BASE LEVEL
			this.m_CharacterData.characterAttackSpeed += baseLevel * this.m_CharacterData.dataPerLevel.characterAttackSpeed;
			// DEFEND POINT BASE LEVEL
			this.m_CharacterData.characterDefendPoint += baseLevel * this.m_CharacterData.dataPerLevel.characterDefendPoint;
			// MAX HEALTH BASE LEVEL
			this.m_CharacterData.characterMaxHealthPoint += baseLevel * this.m_CharacterData.dataPerLevel.characterMaxHealthPoint;
			// FSM ACTIVE
			this.m_FSMComponent.ActiveFSM (true);
			// TEST
			this.m_CharacterData.characterSkillSlots = new CSkillData[] { 
				new CSkillData () { // DEFAULT SKILL
					uID = "502ec8465441f1d108b8c963ec402b08",
					objectName = "Normal Attack",
					objectAvatar = "NormalAttack-avatar",
					objectModel = "NormalAttack-model",
					characterClasses = new string[] { "Warrior","Archer","Wizard" },
					levelRequire = 0,
					skillDelay = 0.1f,
					skillTime = 0.1f,
					skillEffects = new CSkillEffect[] {
						new CSkillEffect () {
							skillValue = 1,
							skillMethod = "ApplyDamage"
						}
					}
				},
				new CSkillData () {
					uID = "502ec8465441f1d108b8c963ec404a66",
					objectName = "Bash",
					objectAvatar = "BashSkill-avatar",
					objectModel = "BashSkill-model",
					characterClasses = new string[] { "Warrior" },
					levelRequire = 3,
					skillDelay = 3f,
					skillTime = 0.1f,
					skillEffects = new CSkillEffect[] {
						new CSkillEffect () {
							skillValue = 15,
							skillMethod = "ApplyDamage"
						}
					}
				},
				new CSkillData () {
					uID = "202ec346f441f1d1a8b8c963ec404a66",
					objectName = "Fire ball",
					objectAvatar = "FireBall-avatar",
					objectModel = "FireBall-model",
					characterClasses = new string[] { "Wizard" },
					levelRequire = 3,
					skillDelay = 5f,
					skillTime = 1f,
					skillEffects = new CSkillEffect[] {
						new CSkillEffect () {
							skillValue = 25,
							skillMethod = "ApplyDamage"
						}
					}
				},
				new CSkillData () {
					uID = "917e6061-e0ba-4819-b28b-34fa85788f1d",
					objectName = "Strong Arrow",
					objectAvatar = "StrongArrow-avatar",
					objectModel = "StrongArrow-model",
					characterClasses = new string[] { "Archer" },
					levelRequire = 3,
					skillDelay = 3f,
					skillTime = 1f,
					skillEffects = new CSkillEffect[] {
						new CSkillEffect () {
							skillValue = 15,
							skillMethod = "ApplyDamage"
						}
					}
				}
			};
			// SKILL SLOT
			this.m_SkillSlotComponent.Init (this, this.m_CharacterData.characterSkillSlots);
		}

		public override CObjectData GetData ()
		{
			return this.m_CharacterData as CObjectData;
		}

		public override void SetTargetEnemy (CObjectController target)
		{
			base.SetTargetEnemy (target);
			this.m_TargetEnemy = target;
		}

		public override CObjectController GetTargetEnemy ()
		{
			return this.m_TargetEnemy;
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

		public virtual void SetHealth (int value)
		{
			var totalHealth = value <= 0 
				? 0 
				: value >= this.m_CharacterData.characterMaxHealthPoint 
				? this.m_CharacterData.characterMaxHealthPoint 
				: value;
			
			totalHealth = totalHealth > this.m_CharacterData.maxHealthPoint 
				? this.m_CharacterData.maxHealthPoint 
				: totalHealth;

			this.InvokeAction ("UpdateHealth", totalHealth, 
				this.m_CharacterData.characterHealthPoint, 
				this.m_CharacterData.characterMaxHealthPoint);

			this.m_CharacterData.characterHealthPoint = totalHealth;
		}

		public virtual int GetHealth ()
		{
			return this.m_CharacterData.characterHealthPoint;
		}

		public virtual int GetMaxHealth ()
		{
			return this.m_CharacterData.characterMaxHealthPoint;
		}

		public virtual void SetAttackPoint (int value)
		{
			var totalPoint = value <= 0 
				? 0 
				: value >= this.m_CharacterData.maxAttackPoint 
				? this.m_CharacterData.maxAttackPoint 
				: value;
			this.m_CharacterData.characterAttackPoint = totalPoint;
		}

		public virtual int GetAttackPoint ()
		{
			return this.m_CharacterData.characterAttackPoint;
		}

		public virtual void SetAttackSpeed (float value)
		{
			var totalPoint = value <= 0f 
				? 0f
				: value >= this.m_CharacterData.maxAttackSpeed 
				? this.m_CharacterData.maxAttackSpeed 
				: value;
			this.m_CharacterData.characterAttackSpeed = totalPoint;
		}

		public virtual float GetAttackSpeed ()
		{
			return this.m_CharacterData.characterAttackSpeed;
		}

		public virtual void SetDefendPoint (int value)
		{
			var totalPoint = value <= 0 
				? 0 
				: value >= this.m_CharacterData.maxDefendPoint 
				? this.m_CharacterData.maxDefendPoint 
				: value;
			this.m_CharacterData.characterDefendPoint = totalPoint;
		}

		public virtual int GetDefendPoint ()
		{
			return this.m_CharacterData.characterDefendPoint;
		}

		public virtual object GetController() {
			return this;
		}

		#endregion
		
	}
}
