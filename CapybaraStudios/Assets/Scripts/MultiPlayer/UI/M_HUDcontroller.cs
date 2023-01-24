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
    [SerializeField] private TextMeshProUGUI deathText;
    private bool dead = false;

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
        if(dead) Death("", false);
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
        if(dead) Death("", false);
        if (!_gameIsPaused)
        {
            tabMenuUI.SetActive(!tabMenuUI.activeSelf);
        }
    }

    public void Death(string killer, bool dead)
    {
        this.dead = dead;
        deathMenuUI.SetActive(dead);
        if(dead == false) return;
        Debug.Log(killer);
        if(killer.Equals("")) {
            deathText.text = "You killed yourself :(";
            return;
        }
        deathText.text = killer + " killed you";
    }

    public void Resume()
    {
        print("Resume");
        if(_input) _input.Resume();
        Cursor.lockState = CursorLockMode.Locked;
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