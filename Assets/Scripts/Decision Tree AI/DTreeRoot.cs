using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTreeRoot : DTreeNode
{
    private DTreeNode _child;

    public DTreeRoot()
    {
        _name = "Root";
    }
    public void SetChild(DTreeNode child)
    {
        _child = child;
    }


    public override Action RunNodeProcess()
    {
        Debug.Log(_name);

        if (_child == null)
        {
            Debug.LogWarning("Root child not set, cannot run process");
            return null;
        }

        return _child.RunNodeProcess();
    }
    public override void PrintTree(int nodeLevel=0)
    {
        Debug.Log($"{new string('-', nodeLevel)}{_name}");

        nodeLevel++;

        _child.PrintTree(nodeLevel);
    }
}
