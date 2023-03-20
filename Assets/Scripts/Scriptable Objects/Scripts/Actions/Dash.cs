using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Dash", menuName = "Scriptable Objects/Common Action/Dash")]
public class Dash : CombatAction
{
    private void Awake()
    {
        _name = "Dash";
        _identifier = CombatActionName.Dash;
    }
}
