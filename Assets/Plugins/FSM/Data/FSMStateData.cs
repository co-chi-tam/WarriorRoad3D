using System;
using System.Collections;
using System.Collections.Generic;

namespace FSM
{
    public class FSMStateData
	{
		public string StateName;
		public FSMCondition Condition;
        public List<FSMStateData> ListStates;

        public FSMStateData()
        {
			this.Condition = new FSMCondition ();
            this.StateName = string.Empty;
            this.ListStates = new List<FSMStateData>();
        }
    }
}
