using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class M_EndGameHUD : MonoBehaviour
{
    private void Awake() {
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadMenu()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Scene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}
