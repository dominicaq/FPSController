using UnityEngine;

[CreateAssetMenu(fileName = "Default State", menuName = "PlayerStates/Default")]
public class DefaultState : AbstractStateMachine
{
    public override bool EnterState()
    {
        base.EnterState();

        Debug.Log("Entered State");
        return true; 
    }

    public override void Tick()
    {
        Debug.Log("Updating State");
    }

    public override bool ExitState()
    {
        base.ExitState();

        Debug.Log("Exited State");
        return true;
    }
}
