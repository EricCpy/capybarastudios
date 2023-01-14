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
        leaderboard.text = GameManager.gameManager.GetScores();
    }
}