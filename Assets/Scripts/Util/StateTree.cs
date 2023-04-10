using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StateTreeMutable
{
    public StateTreeMutable(MapEntity map, SessionConf conf)
    {
        _map = map;
        _conf = conf;
        CurrentStateNode = new MutableStateTreeNode(null, 
            conf.IsPlayerStarting ? new Vector2[1]{map.SpawnPositions.first} : new Vector2[1]{map.SpawnPositions.second},
            conf.IsPlayerStarting ? new Vector2[1]{map.SpawnPositions.second} : new Vector2[1]{map.SpawnPositions.first},
            map.FoodPositions.ToArray()
        );
    }
    private MapEntity _map;
    private SessionConf _conf;
    public MutableStateTreeNode CurrentStateNode;
    private int _transitionsMade = 0;

    public void TransitionToState(Vector2 newPosition){
        StateTreeFactory.ExposeCurrentState(_map, CurrentStateNode,
            _conf.IsPlayerStarting ? _transitionsMade%2==0 : _transitionsMade%2!=0, 3);
    }
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
        FoodPositions = foodPositions;
    }
    public MutableStateTreeNode Parent;
    public List<MutableStateTreeNode> Children;

    public int PlayerScore { get; private set; }
    public int AIScore { get; private set; }
    public Vector2[] PlayerVisitedPositions { get; private set; }
    public Vector2[] AIVisitedPositions { get; private set; }
    public Vector2[] FoodPositions { get; private set; }
    public int HeuristicEvaluation;

    public void PrintPaths(){
        var message = new StringBuilder();
        for (int i=0; i<PlayerVisitedPositions.Length-1; i++)
            message.Append(PlayerVisitedPositions[i]).Append(" ");
        message.Append(PlayerVisitedPositions[PlayerVisitedPositions.Length-1]);
        Debug.Log(message.ToString());

        
        message.Clear();
        for (int i=0; i<AIVisitedPositions.Length-1; i++)
            message.Append(AIVisitedPositions[i]).Append(" ");
        message.Append(AIVisitedPositions[AIVisitedPositions.Length-1]);
        Debug.Log(message.ToString());
    }
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