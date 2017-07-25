using System;
using System.Collections;
using System.Collections.Generic;

namespace FSM
{
	public class FSMCondition {

		public List<string> conditionNames;
		public List<EConditionPrefix> conditionPrefixes;
		public List<EConditionOperator> conditionOperators;

		public enum EConditionPrefix: byte
		{
			None = 0,
			Negative = 1
		}

		public enum EConditionOperator: byte
		{
			None = 0,
			And = 1, 
			Or = 2
		}

		public FSMCondition ()
		{
			this.conditionNames = null;
			this.conditionPrefixes = null;
			this.conditionOperators = null;
		}

	}
}
