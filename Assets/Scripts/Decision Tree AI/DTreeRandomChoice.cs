using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTreeRandomChoice : DTreeNode
{
    private List<DTreeNode> _children;

    public DTreeRandomChoice(string name)
    {
        _name = name;
        _children = new List<DTreeNode>();
    }
    public void AddChild(DTreeNode child)
    {
        _children.Add(child);
    }


    public override Action RunNodeProcess()
    {
        Debug.Log(_name);

        if (_children.Count == 0)
        {
            Debug.LogWarning("No children found, cannot run random choice process");
            return null;
        }

        int childChance = 100 / _children.Count;
        int occurence = UnityEngine.Random.Range(1, 101);

        return _children[Mathf.RoundToInt(occurence / childChance)].RunNodeProcess();
    }
    public override void PrintTree(int nodeLevel = 0)
    {
        Debug.Log($"{new string('-', nodeLevel)}{_name}");

        nodeLevel++;

        foreach (DTreeNode child in _children)
            child.PrintTree(nodeLevel);
    }
}
