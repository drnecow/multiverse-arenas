using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }
    public void PreserveThis()
    {
        DontDestroyOnLoad(this);
    }

    private int _allyRats;
    private int _enemyRats;

    public int AllyRats { get => _allyRats; set => _allyRats = value; }
    public int EnemyRats { get => _enemyRats; set => _enemyRats = value; }


    private void Awake()
    {
        _allyRats = 1;
        _enemyRats = 1;

        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
}
