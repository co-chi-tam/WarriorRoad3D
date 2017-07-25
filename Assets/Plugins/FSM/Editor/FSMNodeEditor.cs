using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FSMGraphNode {
	public class FSMNodeEditor {

		#region Properties

		public string nodeName;
		public Rect nodeRect;
		public ENodeType IsNodeType = ENodeType.Node;

		protected float m_WidthRect = 150f;
		protected float m_HeightRect = 100f; 

		public FSMGraphEditor rootGraph;
		public List<FSMConditionLineEditor> conditionLines;

		public bool OnDrawCondition;

		private FSMConditionLineEditor m_CurrentLine;

		public enum ENodeType
		{
			Node = 0,
			Root = 1,
			AnyNode = 2
		}

		#endregion

		#region Contructor

		public FSMNodeEditor (FSMGraphEditor root, ENodeType nodeType = ENodeType.Node)
		{
			nodeName = "NODE EMPTY";
			nodeRect = new Rect (100f, 100f, m_WidthRect, m_HeightRect);

			rootGraph = root;
			this.IsNodeType = nodeType;
			conditionLines = new List<FSMConditionLineEditor> ();
		}

		public FSMNodeEditor (string name, FSMGraphEditor root,  ENodeType nodeType = ENodeType.Node)
		{
			nodeName = name;
			nodeRect = new Rect (100f, 100f, m_WidthRect, m_HeightRect);

			rootGraph = root;
			this.IsNodeType = nodeType;
			conditionLines = new List<FSMConditionLineEditor> ();
		}

		public FSMNodeEditor (string name, float x, float y, FSMGraphEditor root,  ENodeType nodeType = ENodeType.Node)
		{
			nodeName = name;
			nodeRect = new Rect (x, y, m_WidthRect, m_HeightRect);

			rootGraph = root;
			this.IsNodeType = nodeType;
			conditionLines = new List<FSMConditionLineEditor> ();
		}

		#endregion

		#region Main methods

		public virtual void OnDraw(int id, Event currentEvent) {
			nodeRect = GUI.Window (id, nodeRect, NodeFunction, nodeName);
			if (GUI.Button (new Rect (nodeRect.width + nodeRect.x - 50f, nodeRect.height + nodeRect.y + 1f, 20, 20), "x")) {
				if (IsNodeType == ENodeType.Node) {
					rootGraph.DeleteNode (this);
				}
			}
			var createConditionRect = new Rect (nodeRect.width + nodeRect.x - 25f, nodeRect.height + nodeRect.y + 1f, 25, 20);
			if (GUI.Button (createConditionRect, OnDrawCondition ? "[]" : "->")) {
				m_CurrentLine = new FSMConditionLineEditor ();
				OnDrawCondition = true;
			}
			if (currentEvent.type == EventType.MouseUp) {
				m_CurrentLine = null;
				OnDrawCondition = false;
			}
			if (OnDrawCondition && m_CurrentLine != null) {
				m_CurrentLine.OnDraw (this, currentEvent);
				for (int i = 0; i < FSMGraphEditor.nodeList.Count; i++) {
					var node = FSMGraphEditor.nodeList [i];
					if (node != this 
						&& node.nodeRect.Contains (currentEvent.mousePosition)) {
						var isDuplicatedNode = false;
						for (int x = 0; x < conditionLines.Count; x++) {
							if (conditionLines [x].sourceNode == this && conditionLines [x].targetNode == node) {
								isDuplicatedNode = true;
								break;
							}
						}
						if (isDuplicatedNode == false) {
							OnDrawCondition = false;
							m_CurrentLine.haveDraw = true;
							m_CurrentLine.OnDraw (this, node, currentEvent);
							if (IsNodeType == ENodeType.Node) {
								conditionLines.Add (m_CurrentLine);
								FSMGraphEditor.lineList.Add (m_CurrentLine);
							} else {
								if (conditionLines.Contains (m_CurrentLine) == false) {
									conditionLines.Clear ();
									m_CurrentLine.conditionName = IsNodeType == ENodeType.Root ? "IsRoot" : "ConditionName";
									conditionLines.Add (m_CurrentLine);
									FSMGraphEditor.lineList.Add (m_CurrentLine);
								}
							}
							m_CurrentLine = null;
						}
					}
				}
			}
			for (int i = 0; i < conditionLines.Count; i++) {
				conditionLines [i].OnDraw (currentEvent);
			}
		}

		public virtual void NodeFunction(int id) {
			nodeRect.height = m_HeightRect + conditionLines.Count * 35f;
			GUILayout.BeginVertical ();
			nodeName = GUILayout.TextField (nodeName);
			GUILayout.Label ("");
			for (int i = 0; i < conditionLines.Count; i++) {
				GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("-" + conditionLines[i].targetNode.GetName());
				if (GUILayout.Button ("x")) {
					conditionLines.RemoveAt (i);
					rootGraph.DeleteLine (conditionLines[i]);
					i--;
				}
				GUILayout.EndHorizontal ();
				conditionLines[i].conditionName = GUILayout.TextField (conditionLines[i].conditionName);
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}

//		public virtual void DrawNodeMenu() {
//			GenericMenu menu = new GenericMenu ();
//			menu.AddItem (new GUIContent ("Create Node Condition"), false, CreateNodeCondition, "CreateNodeCondition");
//			menu.AddSeparator("");
//			menu.AddItem (new GUIContent ("Delete Node"), false, CreateNodeCondition, "DeleteNode");
//			menu.ShowAsContext ();
//		}
//
//		public virtual void CreateNodeCondition(object obj) {
//			Debug.Log ("Debug " + obj);
//		}
//
//		public virtual void DeleteNode(object obj) {
//			Debug.Log ("Delete " + obj);
//		}

		#endregion

		#region Getter & Setter

		public virtual string GetName() {
			return nodeName;
		}

		#endregion

	}
}
