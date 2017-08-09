using System;
using UnityEngine;

namespace WarriorRoad {
	public interface ISimpleStatusContext {

		void SetHealth(int value);
		int GetHealth();
		int GetMaxHealth();

		void SetAttackPoint(int value);
		int GetAttackPoint();

		void SetAttackSpeed(float value);
		float GetAttackSpeed();

		void SetDefendPoint(int value);
		int GetDefendPoint();

		void SetActive(bool value);
		bool GetActive();

		object GetController();

		Vector3 GetPosition();
		void SetPosition(Vector3 value);

		Vector3 GetRotation();
		void SetRotation (Vector3 value);

	}
}
