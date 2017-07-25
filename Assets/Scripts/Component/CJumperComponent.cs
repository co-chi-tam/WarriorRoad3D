using System;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CJumperComponent : CComponent {

		[Header ("Jump control")]
		[SerializeField]	protected GameObject m_JumpObject;
		[SerializeField]	protected float m_JumpHeight = 1f;
		[SerializeField]	protected AnimationCurve m_JumpCurve;

		public virtual void SetJumpCurve(float time) {
			var jump = this.m_JumpCurve.Evaluate (time);
			var currentPos = this.m_JumpObject.transform.position;
			currentPos.y = jump * this.m_JumpHeight;
			this.m_JumpObject.transform.position = currentPos;
		}

	}
}
