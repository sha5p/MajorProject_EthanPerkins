using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public static bool IsGamePaused = false;
    Audio_Manager audio_manager;
    public void TogglePause()
    {

        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
        if (IsGamePaused)
        {
            // If the game is currently PAUSED (IsGamePaused is true), RESUME it.

            // 1. Start time
            Time.timeScale = 1f;
            // 2. Hide the UI
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }
            // 3. Update state flag
            IsGamePaused = false;

            Debug.Log("Game Resumed.");
        }
        else
        {
            // If the game is currently RUNNING (IsGamePaused is false), PAUSE it.

            // 1. Stop time
            Time.timeScale = 0f;
            // 2. Show the UI
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);
            }
            // 3. Update state flag
            IsGamePaused = true;

            Debug.Log("Game Paused.");
        }
    }
    public void BackToMenu()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
        Time.timeScale = 1f;
        IsGamePaused = false;
        SceneManager.LoadScene("Menu_Main");
    }
}
