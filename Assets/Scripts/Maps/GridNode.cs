using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    private Monster _monster = null;
    private Obstacle _obstacle = null;

    public bool IsFree { get => _monster == null && (_obstacle == null || _obstacle.IsPassable); }
    public Monster Monster { get => _monster; set => _monster = value; }
    public Obstacle Obstacle { get => _obstacle; set => _obstacle = value; }
}
