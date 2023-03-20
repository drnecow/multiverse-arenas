using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    [field: SerializeField] public bool IsPassable { get; private set; }
}
