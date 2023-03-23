using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTreeBinaryConditional : DTreeNode
{
    private DTreeNode _trueConditionChild;
    private DTreeNode _falseConditionChild;
    private Func<bool> _condition;

    public DTreeBinaryConditional(string name, Func<bool> condition)
    {
        _name = name;
        _condition = condition;
    }
    public void SetTrueConditionChild(DTreeNode trueConditionChild)
    {
        _trueConditionChild = trueConditionChild;
    }
    public void SetFalseConditionChild(DTreeNode falseConditionChild)
    {
        _falseConditionChild = falseConditionChild;
    }


    public override Action RunNodeProcess()
    {
        Debug.Log(_name);

        if (_trueConditionChild == null)
        {
            Debug.LogWarning("True condition child not set, cannot run binary conditional process");
            return null;
        }
        if (_falseConditionChild == null)
        {
            Debug.LogWarning("False condition child not set, cannot run binary conditional process");
            return null;
        }

        bool success = _condition();

        if (success)
        {
            Debug.Log("Yes");
            return _trueConditionChild.RunNodeProcess();
        }
        else
        {
            Debug.Log("No");
            return _falseConditionChild.RunNodeProcess();
        }
    }
    public override void PrintTree(int nodeLevel = 0)
    {
        Debug.Log($"{new string('-', nodeLevel)}{_name}");

        nodeLevel++;

        _trueConditionChild.PrintTree(nodeLevel);
        _falseConditionChild.PrintTree(nodeLevel);
    }
}
