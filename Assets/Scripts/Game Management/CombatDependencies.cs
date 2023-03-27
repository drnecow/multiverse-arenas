using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDependencies : MonoBehaviour
{
    public static CombatDependencies Instance { get; private set; }
    [field: SerializeField] public GridMap Map { get; private set; }
    [field: SerializeField] public CombatEventsLogger EventsLogger { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.Log("CombatDependencies instance already exists, destroying this");
        }
        else
            Instance = this;
    }
}
