using System;
using UnityEngine;


public enum ControlState
{
    Main,
    Swimming,
    Climbing,
    NONE
};

public class PlayerStateManager : MonoBehaviour
{
    #region Componenets
    public ControlState currentControlState = ControlState.NONE;
    public BaseController currentController;
    [System.NonSerialized] public BaseController[] availableControllers;
    
    #endregion

    private void Awake()
    {
        availableControllers = GetComponents<BaseController>();

        SwitchController(availableControllers[0]);
        currentControlState = ControlState.Main;
    }

    private void Update()
    {
        if(currentController)
            currentController.Tick();
    }

    public void SwitchController(BaseController newController)
    {
        if(newController == currentController)
            return;
        
        if(currentControlState != ControlState.NONE)
        {
            newController.currentGravity = currentController.currentGravity;
            newController.inputVector = currentController.inputVector;
            currentController.OnExit();
        }

        currentController = newController;
        currentController.OnEnter();
    }

    // Redo this?
    private void OnTriggerEnter(Collider other)
    {
        if (currentControlState != ControlState.Swimming && other.CompareTag("Ladder"))
        {
            SwitchController(availableControllers[1]);
            currentControlState = ControlState.Climbing;
        }
            
        if (other.CompareTag("Water"))
        {
            SwitchController(availableControllers[2]);
            currentControlState = ControlState.Swimming;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder") && currentControlState == ControlState.Climbing)
        {
            SwitchController(availableControllers[0]);
            currentControlState = ControlState.Main;
        }

        if (other.CompareTag("Water") && currentControlState == ControlState.Swimming)
        {
            SwitchController(availableControllers[0]);
            currentControlState = ControlState.Main;
        }
    }
}
