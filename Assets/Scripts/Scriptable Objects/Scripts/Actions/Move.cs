using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Common Action/Move")]
public class Move : CombatAction
{
    private void Awake()
    {
        _name = "Move";
        _identifier = CombatActionName.Move;
    }
}
