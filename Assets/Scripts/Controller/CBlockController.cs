using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CBlockController : CObjectController {

		[SerializeField]	public GameObject movePoint;
		[SerializeField]	public GameObject enemyPoint;
		[SerializeField]	protected Animator m_BlockAnimator;

		public virtual Vector3 GetMovePointPosition() {
			return this.movePoint.transform.position;
		}

		public virtual Vector3 GetEnemyPointPosition() {
			return this.enemyPoint.transform.position;
		}
		
	}
}
