using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTreeMutable
{
    public StateTreeMutable(MapEntity map)
    {
        // _root = new MutableGameTreeNode(null, );
    }
    public MutableStateTreeNode CurrentStateNode;
}

public class StateTree{
    public StateTree(StateTreeMutable value)
    {
        _value = value;
    }

    private StateTreeMutable _value;
    public StateTreeNode CurrentStateNode => new StateTreeNode(_value.CurrentStateNode);
}



public class MutableStateTreeNode{
    public MutableStateTreeNode(MutableStateTreeNode parent, MutableStateTreeNode[] children, 
        Vector2[] playerVisitedPositions, Vector2[] AIVisitedPositions, 
        Vector2[] foodPositions, int heuristicEvaluation)
    {
        Parent = parent;
        Children = children;
        PlayerScore = 0;
        AIScore = 0;
        PlayerVisitedPositions = playerVisitedPositions;
        this.AIVisitedPositions = AIVisitedPositions;
        HeuristicEvaluation = heuristicEvaluation;
    }
    public MutableStateTreeNode Parent;
    public MutableStateTreeNode[] Children;

    public int PlayerScore { get; private set; }
    public int AIScore { get; private set; }
    public Vector2[] PlayerVisitedPositions { get; private set; }
    public Vector2[] AIVisitedPositions { get; private set; }
    public Vector2[] FoodPositions { get; private set; }
    public int HeuristicEvaluation;
}

public class StateTreeNode{
    public StateTreeNode(MutableStateTreeNode mutableEntity)
    {
        _mutableEntity = mutableEntity;
    }
    private MutableStateTreeNode _mutableEntity;
    public int PlayerScore => _mutableEntity.PlayerScore;
    public int AIScore => _mutableEntity.AIScore;
    public IEnumerable<Vector2> PlayerVisitedPositions => _mutableEntity.PlayerVisitedPositions;
    public IEnumerable<Vector2> AIVisitedPositions => _mutableEntity.AIVisitedPositions;
    public IEnumerable<Vector2> FoodPositions => _mutableEntity.FoodPositions;
    public int HeuristicEvaluation => _mutableEntity.HeuristicEvaluation;
    public IEnumerable<StateTreeNode> Children => Array.ConvertAll(_mutableEntity.Children, item => new StateTreeNode(item));
    public StateTreeNode Parent => new StateTreeNode(_mutableEntity.Parent);
}