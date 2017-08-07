using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUIGameManager : CMonoSingleton<CUIGameManager> {

	
		public virtual void OnStartRoll() {
			// TODO
		}

		public virtual void OnRollPressed() {
			CGameManager.Instance.OnPlayerRollDice ();
		}
		
	}
}
