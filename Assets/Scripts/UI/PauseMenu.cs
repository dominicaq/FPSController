using Managers;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public MenuInputReader inputReader;
    public GameObject pauseMenu, settingsMenu;

	private void OnEnable()
	{
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inputReader.pauseEvent += OnPause;
    }

    private void OnDisable()
	{
        inputReader.pauseEvent -= OnPause;
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameState.isPaused = false;
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameState.isPaused = true;
    }

    private void OnPause()
    {
        if(GameState.isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Settings()
    {
        //pauseMenu.SetActive(false);
        //settingsMenu.SetActive(true);
    }
    
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
