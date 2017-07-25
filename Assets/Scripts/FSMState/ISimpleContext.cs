using System;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	public interface ISimpleContext : IContext {

		bool IsActive();
		bool IsMoveToTargetBlock();
	
	}
}
