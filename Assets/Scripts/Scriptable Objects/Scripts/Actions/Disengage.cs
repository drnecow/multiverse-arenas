using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Disengage", menuName = "Scriptable Objects/Common Action/Disengage")]
public class Disengage : CombatAction
{
    private void Awake()
    {
        _name = "Disengage";
        _identifier = CombatActionName.Disengage;
    }
}
