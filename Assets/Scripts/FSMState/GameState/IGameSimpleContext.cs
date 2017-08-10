using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	public interface IGameSimpleContext : IContext {
	
		bool IsLoadingCompleted();
		bool IsPlayerDeath();
		bool IsRoundCompleted();
		
	}
}
