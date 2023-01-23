using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class M_HUDcontroller : NetworkBehaviour
{
    private static bool _gameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject deathMenuUI;
    public GameObject gameUI;
    public GameObject tabMenuUI;
    public GameObject settingsMenuUI;
    [SerializeField] M_InputManager _input;

    public override void OnNetworkSpawn() {
        if(!IsOwner) Destroy(gameObject);
        initName();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        deathMenuUI.SetActive(false);
        gameUI.SetActive(true);
        tabMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
    }


    public void DoPause()
    {
        if (_gameIsPaused)
        {
            settingsMenuUI.SetActive(false);
            Resume();
        }
        else
        {
            tabMenuUI.SetActive(false);
            Pause();
        }
    }

    public void Tab()
    {
        if (!_gameIsPaused)
        {
            tabMenuUI.SetActive(!tabMenuUI.activeSelf);
        }
    }

    public void Death()
    {
        tabMenuUI.SetActive(false);
        deathMenuUI.SetActive(true);
        gameUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }


    private IEnumerator DeathFadein(float time)
    {
        if (time > 0)
        {
            yield return new WaitForSeconds(0.1f);
            deathMenuUI.GetComponent<CanvasGroup>().alpha += 0.06666666f;
            time -= 100;
            StartCoroutine(DeathFadein(time));
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        }
    }

    public void Resume()
    {
        print("Resume");
        if(_input) _input.Resume();
        Cursor.lockState = CursorLockMode.Locked;
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        //TODO if bedingung, nur wenn Singleplayer, dann timeScale
        _gameIsPaused = false;
        pauseMenuUI.SetActive(false);
        gameUI.SetActive(true);
    }

    public void Pause()
    {
        tabMenuUI.SetActive(false);
        gameUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        //TODO if bedingung, nur wenn Singleplayer, dann timeScale
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        _gameIsPaused = true;
    }

    public void LoadMenu()
    {
        //TODO if bedingung, nur wenn Singleplayer, dann timeScale
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
    }
    
    public TMP_InputField playerInputField;
    public void initName()
    {
        string name = PlayerPrefs.GetString("CurrentName", "Player");
        playerInputField.text = name;
    }

    public void updateName()
    {
        PlayerPrefs.SetString("CurrentName", playerInputField.text);
    }

    public void RestartLevel()
    {
        Resume();
        GameManager.RestartLevel();
    }
}