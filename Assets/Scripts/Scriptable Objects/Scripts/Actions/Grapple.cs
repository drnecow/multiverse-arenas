using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Grapple", menuName = "Scriptable Objects/Common Action/Grapple")]
public class Grapple : CombatAction
{
    private void Awake()
    {
        _name = "Grapple";
        _identifier = CombatActionName.Grapple;
    }
}
