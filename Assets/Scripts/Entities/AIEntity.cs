using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIEntityMutable
{
    public AIEntityMutable(StateTree tree)
    {
        StateTree = tree;
    }

    public StateTree StateTree;

    public AIEntity GetImmutableInstance() => new AIEntity(this);

    public IEnumerable<StateTreeNode> GetTransitionOptions(){
        return StateTree.CurrentStateNode.Children;
    }

    
    public Vector2 GetCurrentPosition() => StateTree.CurrentStateNode.AIVisitedPositions.Last();
}

public class AIEntity{
    public AIEntity(AIEntityMutable value)
    {
        _value = value;
    }
    private AIEntityMutable _value;
    public StateTree StateTree => _value.StateTree;
    public Vector2 GetCurrentPosition() => _value.GetCurrentPosition();
}