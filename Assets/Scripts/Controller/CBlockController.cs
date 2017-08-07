using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CBlockController : CObjectController {

		[Header("Animator")]
		[SerializeField]	protected Animator m_BlockAnimator;

		[Header("Block point")]
		public GameObject movePoint;
		public GameObject enemyPoint;

		[Header("Block Guest")]
		public CCharacterController blockGuest;

		public virtual Vector3 GetMovePointPosition() {
			return this.movePoint.transform.position;
		}

		public virtual Vector3 GetEnemyPointPosition() {
			return this.enemyPoint.transform.position;
		}
		
	}
}
