using UnityEngine;

[CreateAssetMenu(fileName = "Swim State", menuName = "PlayerStates/Swim")]
public class SwimState : AbstractStateMachine
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