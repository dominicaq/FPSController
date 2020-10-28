using UnityEngine;

public enum ExecutionState
{
    NONE,
    ACTIVE,
    COMPLETED,
    TERMINATED
};

public abstract class AbstractStateMachine : ScriptableObject
{
    public ExecutionState ExecutionState{ get; protected set; }

    public virtual void OnEnable() 
    {
        ExecutionState = ExecutionState.NONE;
    }

    public virtual bool EnterState()
    {
        ExecutionState = ExecutionState.ACTIVE;
        return true;
    }

    public abstract void Tick();

    public virtual bool ExitState()
    {
        ExecutionState = ExecutionState.COMPLETED;
        return true;
    }
}
