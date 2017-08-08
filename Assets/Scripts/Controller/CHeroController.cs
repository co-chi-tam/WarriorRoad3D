using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	public class CHeroController : CCharacterController {
		
		[Header ("Component")]
		[SerializeField]	protected CJumperComponent m_JumpComponent;

		protected CBlockController m_NextBlock;

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
		}

		#region FSM

		public override bool IsActive ()
		{
			return base.IsActive ();
		}

		public override bool IsMoveToTargetBlock() {
			return this.currentBlock == this.targetBlock;
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

		#endregion

		#region Getter && Setter

		public override void SetJumpCurve(float time) {
			base.SetJumpCurve (time);
			this.m_JumpComponent.SetJumpCurve (time);
		}

		#endregion

		
	}
}
