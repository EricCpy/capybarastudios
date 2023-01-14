using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scoreboardManager : MonoBehaviour
{
    public TMP_Text leaderboard;

    public void Update()
    {

    }

    private void OnEnable()
    {
        if (GameManager.scores != "")
        {
            leaderboard.text = GameManager.scores;
        }
        else
        {
            leaderboard.text = GameManager.gameManager.GetScores();
        }
    }
}