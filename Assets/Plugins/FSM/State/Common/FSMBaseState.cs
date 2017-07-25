using UnityEngine;
using System.Collections;
using FSM;

public class FSMBaseState : IState {

	public FSMBaseState(IContext context)
	{
		
	}
	
	public virtual void StartState()
	{

	}
	
	public virtual void UpdateState(float dt)
	{
		
	}
	
	public virtual void ExitState()
	{
		
	}

}
