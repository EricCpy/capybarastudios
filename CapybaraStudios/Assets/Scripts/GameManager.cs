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

    public GameObject currentPlayer;

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
    public static string url = "https://floriank.pythonanywhere.com/leaderboard/api/"; // change this for leaderboard submission

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

        if (levelName == "Singleplayer")
        {
            themeOne.Play();
        }

        if (levelName == "AntarcticStation")
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
            if (highscoreEntry.playername == playerName &&
                TimeSpan.Parse(highscoreEntry.playtime).TotalSeconds > TimeSpan.Parse(time).TotalSeconds)
            {
                if (_scoreEntryList[0] == highscoreEntry)
                {
                    message = "New Highscore!";
                }

                _scoreEntryList.Remove(highscoreEntry);
                break;
            }
            else if (highscoreEntry.playername == playerName && TimeSpan.Parse(highscoreEntry.playtime).TotalSeconds <=
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
            playername = playerName,
            playtime = time,
            killcount = kills,
            damagedealt = damageDone,
            time = DateTime.Now
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
            playername = "current",
            playtime = time,
            killcount = kills,
            damagedealt = damageDone,
            time = DateTime.Now
        });
        tmp.Sort((o1, o2) =>
            (int)((TimeSpan.Parse(o1.playtime).TotalSeconds - TimeSpan.Parse(o2.playtime).TotalSeconds)));

        foreach (var entry in tmp)
        {
            int length = Math.Min(entry.playername.Length, 10);
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
                entry.playername.Substring(0, length),
                entry.time, entry.killcount, entry.damagedealt);
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
            playername = "current",
            playtime = time,
            killcount = kills,
            damagedealt = damageDone,
            time = DateTime.Now
        });
        tmp.Sort((o1, o2) =>
            (int)((TimeSpan.Parse(o1.playtime).TotalSeconds - TimeSpan.Parse(o2.playtime).TotalSeconds)));

        return tmp;
    }

    private void SortTimers()
    {
        _scoreEntryList.Sort((o1, o2) =>
            (int)((TimeSpan.Parse(o1.playtime).TotalSeconds - TimeSpan.Parse(o2.playtime).TotalSeconds)));
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
        string nameOfMap = "";
        if (SceneManager.GetActiveScene().name == "Singleplayer") nameOfMap = "Space";
        else nameOfMap = "Arctic";
        ScoreEntry tmp = new ScoreEntry()
        {
            damagedealt = damageDone,
            killcount = kills,
            time = DateTime.Now,
            playername = PlayerPrefs.GetString("CurrentName", "Player"),
            playtime = time,
            mapname = nameOfMap
        };

        string json2 = JsonUtility.ToJson(tmp);
        StartCoroutine(Upload(json2));
    }

    IEnumerator Upload(string json)
    {
        using UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
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


    private class Scores
    {
        public List<ScoreEntry> scores;
    }

    [System.Serializable]
    public class ScoreEntry
    {
        public string playtime;
        public int killcount;
        public int damagedealt;
        public string playername;
        public DateTime time;
        public string mapname;
    }
}