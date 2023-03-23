using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInitializationHelper : MonoBehaviour
{
    [SerializeField] List<Monster> _monstersToInitialize;
    [SerializeField] GridMap _map;

    void Start()
    {
        foreach (Monster monster in _monstersToInitialize)
            _map.PlaceMonsterOnCoords(monster, monster.CurrentCoordsOriginCell);
    }
}
