using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform respawnPoint;
    private Vector3 respawnPosition;
    public GameObject player;
    public event CharacterSpawnedDelegate OnCharacterSpawned;
    public static int kills = 0;
    public static int damageDone = 0;
    public static string time = TimeSpan.Zero.ToString(@"hh\:mm\:ss");

    public delegate void CharacterSpawnedDelegate(GameObject player);

    private GameObject currentPlayer;

    private static Transform _dummy;
    public Transform dummy;

    public AudioSource themeOne;
    public AudioSource themeTwo;
    public AudioSource themeOneIntense;
    public AudioSource themeTwoIntense;
    public AudioSource themeThree;
    public AudioSource themeThreeIntense;
    private PlayerStats playerStats;
    private string saveLocation;
    private List<ScoreEntry> _scoreEntryList;
    public static GameManager gameManager;
    public static string scores;
    public static List<ScoreEntry> scorelist;
    private static string levelName;
    private int sceneIndex;
    public static string url = "http://127.0.0.1:8000/leaderboard/api/"; // change this for leaderboard submission

    private void Start()
    {
        levelName = SceneManager.GetActiveScene().name;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        scores = "";
        scorelist = new List<ScoreEntry>();
        gameManager = this;
        _scoreEntryList = new List<ScoreEntry>();
        saveLocation = "timeTable" + SceneManager.GetActiveScene().name;
        string json = PlayerPrefs.GetString(saveLocation);
        if ((json != ""))
        {
            _scoreEntryList = JsonUtility.FromJson<Scores>(json).scores;
        }

        if (sceneIndex == 1)
        {
            themeOne.Play();
        }

        if (sceneIndex == 3)
        {
            themeThree.Play();
        }


        _dummy = dummy;

        respawnPosition = respawnPoint.position;
        kills = 0;
        damageDone = 0;
        time = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
        if (!GameObject.Find("Player"))
        {
            SpawnPlayer();
        }
        else
        {
            currentPlayer = GameObject.Find("Player");
        }
    }

    public void Update()
    {
        time = TimeSpan.FromSeconds(Time.timeSinceLevelLoad).ToString(@"hh\:mm\:ss");


        playerStats = player.GetComponent<PlayerStats>();
        if (playerStats.currentHealth < playerStats.maxHealth)
        {
            if (themeOne.isPlaying)
            {
                themeOneIntense.Play();
                if (themeOneIntense.isPlaying)
                {
                    themeOne.Stop();
                }
            }

            if (themeTwo.isPlaying)
            {
                themeTwoIntense.Play();
                if (themeTwoIntense.isPlaying)
                {
                    themeTwo.Stop();
                }
            }

            if (themeThree.isPlaying)
            {
                themeThreeIntense.Play();
                if (themeThreeIntense.isPlaying)
                {
                    themeThree.Stop();
                }
            }
        }
        else
        {
            if (themeOneIntense.isPlaying)
            {
                themeOne.Play();
                themeOneIntense.Stop();
            }

            if (themeTwoIntense.isPlaying)
            {
                themeTwo.Play();
                themeTwoIntense.Stop();
            }

            if (themeThreeIntense.isPlaying)
            {
                themeThree.Play();
                themeThreeIntense.Stop();
            }
        }
    }

    public void Respawn()
    {
        Time.timeScale = 1;
        Destroy(currentPlayer);
        SpawnPlayer();
    }

    public static void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(levelName);
    }

    public void SpawnPlayer()
    {
        currentPlayer = Instantiate(player, respawnPosition, Quaternion.identity);
        OnCharacterSpawned?.Invoke(currentPlayer);
    }

    public void changeRespawn(Transform t)
    {
        changeRespawn(t.position);
    }

    public void changeRespawn(Vector3 p)
    {
        respawnPosition = p;
    }


    public static void triggerRespawn(Vector3 pos)
    {
        Instantiate(_dummy, pos, Quaternion.Euler(0, 90, 0));
    }

    public void teleport()
    {
        themeOne.Stop();
        themeTwo.Play();
    }

    public void CompleteLevel()
    {
        scores = GetScores();
        scorelist = GetScore();
        registerScore();
    }

    public String registerScore()
    {
        var playerName = PlayerPrefs.GetString("CurrentName", "Player");

        var message = "New Personal Best!";
        //lapLogger.text += "\n" + CourseTimer.toString(time);
        foreach (ScoreEntry highscoreEntry in _scoreEntryList)
        {
            if (highscoreEntry.playerName == playerName &&
                TimeSpan.Parse(highscoreEntry.time).TotalSeconds > TimeSpan.Parse(time).TotalSeconds)
            {
                if (_scoreEntryList[0] == highscoreEntry)
                {
                    message = "New Highscore!";
                }

                _scoreEntryList.Remove(highscoreEntry);
                break;
            }
            else if (highscoreEntry.playerName == playerName && TimeSpan.Parse(highscoreEntry.time).TotalSeconds <=
                     TimeSpan.Parse(time).TotalSeconds)
            {
                return "";
            }
        }

        if (_scoreEntryList.Count == 0)
        {
            message = "New Highscore!";
        }

        _scoreEntryList.Add(new ScoreEntry
        {
            playerName = playerName, time = time, kills = kills, damageDone = damageDone,
            madeAt = DateTime.Now
        });
        SortTimers();
        saveScores();
        return message;
    }


    public string GetScores()
    {
        if (scores != "") return scores;
        var sb = new StringBuilder();
        sb.Append(String.Format("{0,-4} {1,-15} {2,-8} {3,-5} {4,-6}\n", "Rank", "Name",
            "time", "kills", "damage"));
        if (_scoreEntryList == null)
        {
            return "";
        }

        var tmp = new List<ScoreEntry>();
        tmp.AddRange(_scoreEntryList);
        tmp.Add(new ScoreEntry
        {
            playerName = "current", time = time, kills = kills,
            damageDone = damageDone, madeAt = DateTime.Now
        });
        tmp.Sort((o1, o2) =>
            (int)((TimeSpan.Parse(o1.time).TotalSeconds - TimeSpan.Parse(o2.time).TotalSeconds)));

        foreach (var entry in tmp)
        {
            int length = Math.Min(entry.playerName.Length, 10);
            string prefix = "";
            switch (tmp.IndexOf(entry) + 1)
            {
                case 1:
                    prefix = "1ST";
                    break;
                case 2:
                    prefix = "2ND";
                    break;
                case 3:
                    prefix = "3RD";
                    break;

                default:
                    prefix = tmp.IndexOf(entry) + 1 + "TH";
                    break;
            }

            string s = String.Format("{0,-4} {1,-15} {2,-8} {3,-5} {4,-6}\n", prefix,
                entry.playerName.Substring(0, length),
                entry.time, entry.kills, entry.damageDone);
            sb.Append(s);
        }

        return sb.ToString();
    }

    public List<ScoreEntry> GetScore()
    {
        var tmp = new List<ScoreEntry>();
        tmp.AddRange(_scoreEntryList);
        tmp.Add(new ScoreEntry
        {
            playerName = "current", time = time, kills = kills,
            damageDone = damageDone, madeAt = DateTime.Now
        });
        tmp.Sort((o1, o2) =>
            (int)((TimeSpan.Parse(o1.time).TotalSeconds - TimeSpan.Parse(o2.time).TotalSeconds)));

        return tmp;
    }

    private void SortTimers()
    {
        _scoreEntryList.Sort((o1, o2) =>
            (int)((TimeSpan.Parse(o1.time).TotalSeconds - TimeSpan.Parse(o2.time).TotalSeconds)));
        while (_scoreEntryList.Count > 10) // nur die top 10 und keine doppelten einträge 
        {
            _scoreEntryList.RemoveRange(10, 1);
        }
    }

    public void ResetTimes()
    {
        PlayerPrefs.SetString(saveLocation, "");
        PlayerPrefs.Save();
        _scoreEntryList.Clear();
    }

    public void saveScores()
    {
        Scores highscores = new Scores { scores = _scoreEntryList };
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(saveLocation, json);
        PlayerPrefs.Save();
        ScoreEntry tmp = new ScoreEntry()
        {
            damageDone = damageDone, kills = kills, madeAt = DateTime.Now,
            playerName = PlayerPrefs.GetString("CurrentName", "Player"), time = time,
            map = SceneManager.GetActiveScene().name
        };

        string json2 = JsonUtility.ToJson(tmp);
        Upload(json2);
        //TODO, submit score to database
    }

    IEnumerator Upload(string json)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", json);

        using (UnityWebRequest www = UnityWebRequest.Post(url, json))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("data upload complete!");
            }
        }
    }


    private class Scores
    {
        public List<ScoreEntry> scores;
    }

    [System.Serializable]
    public class ScoreEntry
    {
        public string time;
        public int kills;
        public int damageDone;
        public string playerName;
        public DateTime madeAt;
        public string map;
    }
}