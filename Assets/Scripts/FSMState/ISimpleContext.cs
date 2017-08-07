using System;
using UnityEngine;
using FSM;

namespace WarriorRoad {
	public interface ISimpleContext : IContext {

		// COMMON
		bool IsActive();
		// CHARACTER
		bool IsMoveToTargetBlock();
		bool HaveEnemy();
		// SKILL
		bool IsActionCompleted();

	}
}
