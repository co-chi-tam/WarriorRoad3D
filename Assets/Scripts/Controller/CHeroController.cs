using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	public class CHeroController : CCharacterController {
		
		[Header ("Component")]
		[SerializeField]	protected CJumperComponent m_JumpComponent;
		[SerializeField]	protected CSimpleSkillSlotComponent m_SkillSlotComponent;

		protected CBlockController m_NextBlock;
		protected float m_AttackDelay = 0f;

		protected override void Awake ()
		{
			base.Awake ();
			this.m_JumpComponent.Init ();
		}

		protected override void Start ()
		{
			base.Start ();
		}

		protected override void RegisterComponent ()
		{
			base.RegisterComponent ();
			this.m_ListComponents.Add (this.m_JumpComponent);
			this.m_ListComponents.Add (this.m_SkillSlotComponent);
		}

		#region FSM

		public override bool IsActive ()
		{
			return base.IsActive ();
		}

		public override bool IsMoveToTargetBlock() {
			return this.currentBlock == this.targetBlock;
		}

		public override bool HaveEnemy ()
		{
			base.HaveEnemy ();
			if (this.m_TargetEnemy == null)
				return false;
			return this.m_TargetEnemy.GetActive ();
		}

		#endregion

		#region Control

		public override void StartIdle() {
			base.StartIdle ();
		}

		public override void UpdateAction(float dt) {
			base.UpdateAction (dt);
			this.MoveToBlock (dt);
			var guestBlock = this.targetBlock.blockGuest;
			if (guestBlock != null) {
				this.SetTargetEnemy (guestBlock);
			}
		}

		protected virtual void MoveToBlock(float dt) {
			var nextBlockIndex = this.GetStep() + 1;
			var currentBlockCtrl = this.currentBlock;
			var nextBlockCtrl = this.m_MapManager.CalculateCurrentBlock (nextBlockIndex);
			if (nextBlockCtrl == null)
				return;
			this.m_NextBlock = nextBlockCtrl;
			var direction = nextBlockCtrl.GetMovePointPosition() - this.GetPosition();
			var maxLength = nextBlockCtrl.GetMovePointPosition() - currentBlockCtrl.GetMovePointPosition();
			if (direction.sqrMagnitude < 0.01f) {
				this.currentBlock = nextBlockCtrl;
				this.SetStep (nextBlockIndex);
			} else {
				var movePos = (direction.normalized * 1.5f * dt) + this.GetPosition();
				this.SetPosition (movePos);
				this.SetRotation (nextBlockCtrl.GetMovePointPosition());
				this.SetJumpCurve (direction.sqrMagnitude / maxLength.sqrMagnitude);
			}
		}

		public override void UpdateAttackAction(float dt) {
			base.UpdateAttackAction (dt);
			var target = this.m_TargetEnemy as CCharacterController;
			if (target != null) {
				if (this.m_AttackDelay < 0f) {
					this.m_AttackDelay = 1f / this.m_CharacterData.characterAttackSpeed;
					// TEST
					this.m_SkillSlotComponent.ActiveSkillSlot (0, target);
				} else {
					this.m_AttackDelay -= dt;
				}
				this.SetRotation (target.GetPosition());
			}
		}

		#endregion

		#region Getter && Setter

		public override void SetJumpCurve(float time) {
			base.SetJumpCurve (time);
			this.m_JumpComponent.SetJumpCurve (time);
		}

		public override void SetData (CObjectData value)
		{
			base.SetData (value);
			// TEST
			this.m_CharacterData.characterSkillSlots = new CSkillData[] { 
				new CSkillData () {
					uID = "502ec8465441f1d108b8c963ec402b08",
					objectName = "Normal Attack",
					objectAvatar = "NormalAttack-avatar",
					objectModel = "NormalAttack-model",
					skillDelay = 2f,
					skillTime = 1f,
					skillEffectTime = 0.2f,
					skillTriggers = new CSkillEffect[] {
						new CSkillEffect () {
							skillValue = 1,
							skillMethod = "ApplyDamage"
						}
					}
				}
			};
			this.m_SkillSlotComponent.Init (this, this.m_CharacterData.characterSkillSlots);
		}

		#endregion

		
	}
}
