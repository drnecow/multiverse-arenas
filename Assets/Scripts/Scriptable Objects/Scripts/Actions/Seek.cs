using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Seek", menuName = "Scriptable Objects/Common Action/Seek")]
public class Seek : CombatAction
{
    private void Awake()
    {
        _name = "Seek";
        _identifier = CombatActionName.Seek;
    }
}
