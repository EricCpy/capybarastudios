using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class multiplayerScoreboard : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<GameObject> entries;

    public void Awake()
    {
        entryContainer = transform.Find("EntryContainer");
        entryTemplate = entryContainer.Find("EntryTemplate");
        entries = new List<GameObject>();
        entryTemplate.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        var templateHeight = 40f;
        var index = 0;
        ScoreManager scoreManager = ScoreManager.Instance;
        var scores = scoreManager.players.Values;
        
        foreach (var player in scores)
        {
            var entryTransform = Instantiate(entryTemplate, entryContainer);
            entries.Add(entryTransform.gameObject);
            var entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * index);
            entryTransform.gameObject.SetActive(true);

            var prefix = "";
            switch (index + 1)
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
                    prefix = index + 1 + "TH";
                    break;
            }

            var currentPlayer = Color.cyan;

            if (player.playerName.Value.ToString() == PlayerPrefs.GetString("CurrentName", "Player"))
            {
                entryTransform.Find("Name").GetComponent<TMP_Text>().color = currentPlayer;
                entryTransform.Find("Rank").GetComponent<TMP_Text>().color = currentPlayer;
                entryTransform.Find("Deaths").GetComponent<TMP_Text>().color = currentPlayer;
                entryTransform.Find("Kills").GetComponent<TMP_Text>().color = currentPlayer;
                entryTransform.Find("Dmg").GetComponent<TMP_Text>().color = currentPlayer;
            }

            entryTransform.Find("Rank").GetComponent<TMP_Text>().text = prefix;
            entryTransform.Find("Name").GetComponent<TMP_Text>().text = player.playerName.Value.ToString();
            entryTransform.Find("Deaths").GetComponent<TMP_Text>().text = player.networkDeaths.Value + "";
            entryTransform.Find("Kills").GetComponent<TMP_Text>().text = player.networkKills.Value + "";
            entryTransform.Find("Dmg").GetComponent<TMP_Text>().text = player.networkDmg.Value + "";


            index++;
        }
    }



    private void OnDisable()
    {
        entries.ForEach(entry => Destroy(entry));
        entries.Clear();
    }
}