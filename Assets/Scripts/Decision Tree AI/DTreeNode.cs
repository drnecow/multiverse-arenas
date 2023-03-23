using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTreeNode 
{
    protected string _name;


    public virtual Action RunNodeProcess()
    {
        return null;
    }

    public virtual void PrintTree(int nodeLevel=0)
    {

    }
}
