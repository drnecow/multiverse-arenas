using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDependencies : MonoBehaviour
{
    public static CombatDependencies Instance { get; private set; }
    public GridMap Map { get; private set; }
    public MapHighlight Highlight { get; private set; }
    public MonsterHighlighter MonsterHighlighter { get; private set; }
    public MonsterActionTracker MonsterActionTracker { get; private set; }
    public CombatEventsLogger EventsLogger { get; private set; }
    public CombatManager CombatManager { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.Log("CombatDependencies instance already exists, destroying this");
        }
        else
            Instance = this;

        MonsterHighlighter = gameObject.GetComponent<MonsterHighlighter>();
        MonsterActionTracker = gameObject.GetComponent<MonsterActionTracker>();
        EventsLogger = gameObject.GetComponent<CombatEventsLogger>();
        CombatManager = gameObject.GetComponent<CombatManager>();

        CombatManager.OnWinGame += () => EventsLogger.LogScreenInfo("You win", LogColor.Hit);
        CombatManager.OnLoseGame += () => EventsLogger.LogScreenInfo("You lose", LogColor.Miss);
    }

    public void SetMapAndHighlight(GridMap map)
    {
        Map = map;
        Highlight = map.GetComponent<MapHighlight>();
    }
}
