using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    protected GridMap _map;
    public List<Coords> CurrentCoords { get => GetCurrentCoords(); }
    [field: SerializeField] public bool IsPassable { get; private set; }


    public void SetMap(GridMap map)
    {
        _map = map;
    }
    public virtual List<Coords> GetCurrentCoords()
    {
        return new List<Coords>() { _map.WorldPositionToXY(transform.position) };
    }
}
