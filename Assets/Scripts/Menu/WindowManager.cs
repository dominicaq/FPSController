using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WindowManager : MonoBehaviour
{
    public GameObject[] windows;
    public GameObject currentWindow;
    private CanvasGroup currentCanvas;
    
    public void Start()
    {
        currentWindow = windows[0];
        currentCanvas = currentWindow.GetComponent<CanvasGroup>();
    }

    public void SwitchToWindow(string target)
    {
        if (currentWindow.name != target)
        {
            GameObject previousWindow = currentWindow;
            for (int i = 0; i < windows.Length; i++)
            {
                if (windows[i].name == target)
                {
                    currentWindow = windows[i];
                    currentCanvas = currentWindow.GetComponent<CanvasGroup>();
                    EnableWindow(currentCanvas);
                    break;
                }
            }

            if (previousWindow.name != target)
            {
                CanvasGroup prevCanvas = previousWindow.GetComponent<CanvasGroup>();
                DisableWindow(prevCanvas);
            }
        }
    }

    private void DisableWindow(CanvasGroup canvas)
    {
        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    private void EnableWindow(CanvasGroup canvas)
    {
        canvas.alpha = 1;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }
}
