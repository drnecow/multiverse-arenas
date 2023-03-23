using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTreeLeaf : DTreeNode
{
    private Action _action;

    public DTreeLeaf(string name, Action action)
    {
        _name = name;
        _action = action;
    }


    public override Action RunNodeProcess()
    {
        Debug.Log(_name);

        if (_action == null)
        {
            Debug.LogWarning("Action of leaf not chosen, cannot run process");
            return null;
        }

        return _action;
    }
    public override void PrintTree(int nodeLevel = 0)
    {
        Debug.Log($"{new string('-', nodeLevel)}{_name}");
    }
}
