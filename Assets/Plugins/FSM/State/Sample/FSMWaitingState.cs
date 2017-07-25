using UnityEngine;
using System.Collections;
using FSM;

public class FSMWaitingState : FSMBaseState
{
	public FSMWaitingState(IContext context) : base (context)
	{
		
	}
	
	public override void StartState()
	{
		base.StartState ();
	}
	
	public override void UpdateState(float dt)
	{
		base.UpdateState (dt);
	}
	
	public override void ExitState()
	{
		base.ExitState ();
	}
}
