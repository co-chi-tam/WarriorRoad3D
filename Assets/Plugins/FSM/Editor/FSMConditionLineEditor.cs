using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FSMGraphNode {
	public class FSMConditionLineEditor {

		#region Properties

		public string conditionName;
		public FSMNodeEditor sourceNode;
		public FSMNodeEditor targetNode;
		public bool haveDraw;

		#endregion

		#region Contructor

		public FSMConditionLineEditor ()
		{
			conditionName = "ConditionName"; 
			sourceNode = null;
			targetNode = null;
		}

		public FSMConditionLineEditor (string name)
		{
			conditionName = name; 
			sourceNode = null;
			targetNode = null;
		}

		public FSMConditionLineEditor (string name, FSMNodeEditor source, FSMNodeEditor target)
		{
			conditionName = name; 
			sourceNode = source;
			targetNode = target;
		}

		#endregion

		#region Main methods

		public virtual void OnDraw(FSMNodeEditor node, Event currentEvent) {
			var pos = currentEvent.mousePosition; 
			var startTangent = new Vector2 (node.nodeRect.xMax, node.nodeRect.center.y);
			Handles.BeginGUI ();
			Handles.DrawBezier(node.nodeRect.center, pos, 
				startTangent, 
				new Vector2(pos.x, pos.y), Color.red, null, 5f);
			var center = (startTangent + pos) / 2f;
			Handles.Label (center, conditionName);
			sourceNode = node;
			targetNode = null;
			Handles.EndGUI ();
		}

		public virtual void OnDraw(Event currentEvent) {
			if (sourceNode == null || targetNode == null)
				return;
			OnDraw (sourceNode.nodeRect, targetNode.nodeRect);
		}

		public virtual void OnDraw(FSMNodeEditor node1, FSMNodeEditor node2, Event currentEvent) {
			OnDraw (node1.nodeRect, node2.nodeRect);
			sourceNode = node1;
			targetNode = node2;
		}

		private void OnDraw(Rect source, Rect target) {
			var startTangent = new Vector2 (source.xMax, target.center.y);
			var endTangent = new Vector2 (source.x, target.y);
			Handles.BeginGUI ();
			Handles.DrawBezier(source.center, target.center, 
				startTangent, endTangent, Color.green, null, 5f);
			var center = (startTangent + endTangent) / 2f;
			Handles.Label (center, conditionName);
			Handles.EndGUI ();
		}

		#endregion

		#region Getter & Setter

		public virtual string GetName() {
			return conditionName;
		}

		public virtual FSMNodeEditor GetSourceNode() {
			return sourceNode;
		}

		public virtual FSMNodeEditor GetTargetNode() {
			return targetNode;
		}

		#endregion

	}
}
