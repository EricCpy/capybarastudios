using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Core.Singletons;
using Unity.Netcode;

public class ScoreManager : Singleton<ScoreManager>
{   
    public Dictionary<ulong, M_PlayerStats> players;

}
