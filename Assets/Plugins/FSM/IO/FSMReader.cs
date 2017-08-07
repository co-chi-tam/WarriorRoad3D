using System;
using System.Text;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace FSM
{
    public class FSMReader
    {
        private Dictionary<string, FSMStateData> m_Maps;
        private FSMStateData m_RootStates;
		private FSMStateData m_AnyStates;

        public FSMStateData FSMRootStates
        {
			get { return m_RootStates; }
        }

        public FSMStateData FSMAnyStates
        {
            get { return m_AnyStates; }
        }

		public Dictionary<string, FSMStateData> FSMMaps
		{
			get { return m_Maps; }
		}

        public FSMReader()
        {
            m_Maps = new Dictionary<string, FSMStateData>();
        }

		public void LoadJSON(string jsonText)
        {
			var jsonObject = Json.Deserialize (jsonText) as Dictionary<string, object>;
			var states = jsonObject ["fsm"] as List<object>;
			LoadFSM (states [0] as Dictionary<string, object>, ref m_RootStates);
			LoadFSM (states [1] as Dictionary<string, object>, ref m_AnyStates);
        }

        private void LoadFSM(Dictionary<string, object> data, ref FSMStateData stateData)
        {
            var state = new FSMStateData();
			state.Condition = LoadCondition (data["condition_name"].ToString());
            state.StateName = data["state_name"].ToString();
            m_Maps[state.StateName] = state;
            LoadChild(data["states"] as List<object>, state.ListStates);
            stateData = state;
        }

        private void LoadChild(List<object> value, List<FSMStateData> states)
        {
            for (int i = 0; i < value.Count; i++)
            {
                var data = value[i] as Dictionary<string, object>;
                var stateName 			= data["state_name"].ToString();
				var state 				= new FSMStateData();
				state.Condition 		= LoadCondition (data["condition_name"].ToString());
                if (m_Maps.ContainsKey(stateName))
                {
                    state.StateName 	= stateName;
                    state.ListStates 	= m_Maps[stateName].ListStates;
                }
                else
                {
                    state.StateName 	= stateName;
                    m_Maps[stateName] 	= state;
                    LoadChild(data["states"] as List<object>, state.ListStates);
                }
                states.Add(state);
            }
        }

		private FSMCondition LoadCondition(string value) {
			var fsmCondition = new FSMCondition();

			fsmCondition.conditionNames = new List<string> ();
			fsmCondition.conditionPrefixes = new List<FSMCondition.EConditionPrefix> ();
			fsmCondition.conditionOperators = new List<FSMCondition.EConditionOperator> ();

			var stringBuilder = new StringBuilder ();
			for (int i = 0; i < value.Length; i++) {
				var charTmp = value [i].ToString();
				switch (charTmp) {
				case "&":
					i += 1;
					fsmCondition.conditionNames.Add (stringBuilder.ToString ());
					stringBuilder = new StringBuilder ();
					stringBuilder.Append ("&&");
					break;
				case "|":
					i += 1;
					fsmCondition.conditionNames.Add (stringBuilder.ToString());
					stringBuilder = new StringBuilder ();
					stringBuilder.Append ("||");
					break;
				default:
					stringBuilder.Append (charTmp);
					break;
				}
			}
			if (stringBuilder.Length > 0) {
				fsmCondition.conditionNames.Add (stringBuilder.ToString());
			}
			for (int i = 0; i < fsmCondition.conditionNames.Count; i++) {
				var conditionName = fsmCondition.conditionNames [i];
				var negative = conditionName.IndexOf ("!");
				var conditionAnd = conditionName.IndexOf ("&&");
				var conditionOr = conditionName.IndexOf ("||");
				if (negative != -1) {
					fsmCondition.conditionPrefixes.Add (FSMCondition.EConditionPrefix.Negative);
					conditionName = conditionName.Replace ("!", "");
				} else {
					fsmCondition.conditionPrefixes.Add (FSMCondition.EConditionPrefix.None);
				}
				if (conditionAnd != -1) {
					fsmCondition.conditionOperators.Add (FSMCondition.EConditionOperator.And);
					conditionName = conditionName.Replace ("&&", "");
				} else if (conditionOr != -1) { 
					fsmCondition.conditionOperators.Add (FSMCondition.EConditionOperator.Or);
					conditionName = conditionName.Replace ("||", "");
				} else {
					fsmCondition.conditionOperators.Add (FSMCondition.EConditionOperator.None);
				}
				fsmCondition.conditionNames [i] = conditionName;				
			}
			return fsmCondition;
		}
    }
}
