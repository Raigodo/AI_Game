using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateTreeMutable
{
    public StateTreeMutable(MapEntity map, SessionConf conf)
    {
        CurrentStateNode = new MutableStateTreeNode(null, 
            conf.IsPlayerStarting ? new Vector2[1]{map.SpawnPositions.first} : new Vector2[1]{map.SpawnPositions.second},
            conf.IsPlayerStarting ? new Vector2[1]{map.SpawnPositions.second} : new Vector2[1]{map.SpawnPositions.first},
            map.FoodPositions.ToArray()
        );
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
    public MutableStateTreeNode(MutableStateTreeNode parent, 
        Vector2[] playerVisitedPositions, Vector2[] AIVisitedPositions, 
        Vector2[] foodPositions)
    {
        Parent = parent;
        Children = new List<MutableStateTreeNode>();
        PlayerScore = 0;
        AIScore = 0;
        PlayerVisitedPositions = playerVisitedPositions;
        this.AIVisitedPositions = AIVisitedPositions;
    }
    public MutableStateTreeNode Parent;
    public List<MutableStateTreeNode> Children;

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
    public IEnumerable<StateTreeNode> Children => _mutableEntity.Children.Select(item => new StateTreeNode(item));
    public StateTreeNode Parent => new StateTreeNode(_mutableEntity.Parent);
}