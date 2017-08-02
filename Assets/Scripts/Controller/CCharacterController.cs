using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	public class CCharacterController : CObjectController, ISimpleContext {

		[Header ("Control")]
		[SerializeField]	protected Animator m_Animator;

		[Header ("Block")]
		public CBlockController currentBlock;
		public CBlockController nextBlock;
		public CBlockController targetBlock;
		public int blockIndex = 0;

		[Header ("Data")]
		[SerializeField]	protected CHeroData m_CharacterData;

		[Header ("Component")]
		[SerializeField]	protected CJumperComponent m_JumpComponent;
		[SerializeField]	protected CFSMComponent m_FSMComponent;

		protected CMapManager m_MapManager;

		protected override void Awake ()
		{
			base.Awake ();
			this.m_JumpComponent.Init ();
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
			this.m_ListComponents.Add (this.m_JumpComponent);
			this.m_ListComponents.Add (this.m_FSMComponent);
		}

		#region FSM

		public virtual bool IsActive() {
			return this.GetActive ();
		}

		public virtual bool IsMoveToTargetBlock() {
			return this.currentBlock == this.targetBlock;
		}

		#endregion

		#region Control

		public virtual void StartIdle() {
			
		}

		public virtual void UpdateAction(float dt) {
			this.MoveToBlock (dt);
		}

		protected virtual void MoveToBlock(float dt) {
			var nextBlockIndex = this.blockIndex + 1;
			var currentBlockCtrl = this.currentBlock;
			var nextBlockCtrl = this.m_MapManager.CalculateCurrentBlock (nextBlockIndex);
			if (nextBlockCtrl == null)
				return;
			this.nextBlock = nextBlockCtrl;
			var direction = nextBlockCtrl.GetMovePointPosition() - this.GetPosition();
			var maxLength = nextBlockCtrl.GetMovePointPosition() - currentBlockCtrl.GetMovePointPosition();
			if (direction.sqrMagnitude < 0.01f) {
				this.currentBlock = nextBlockCtrl;
				this.blockIndex = nextBlockIndex;
			} else {
				var movePos = (direction.normalized * 1f * dt) + this.GetPosition();
				this.SetPosition (movePos);
				this.SetRotation (nextBlockCtrl.GetMovePointPosition());
				this.SetJumpCurve (direction.sqrMagnitude / maxLength.sqrMagnitude);
			}
		}

		#endregion

		#region Getter && Setter

		public override void SetData (CObjectData value)
		{
			base.SetData (value);
			this.m_CharacterData = value as CHeroData;
		}

		public override CObjectData GetData ()
		{
			return this.m_CharacterData as CObjectData;
		}

		public virtual void SetJumpCurve(float time) {
			this.m_JumpComponent.SetJumpCurve (time);
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

		#endregion
		
	}
}
