using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIInteractor : BaseInteractor
{
    public AIInteractor(AIEntityMutable entity, 
        SessionConfInteractor confInteractor,
        BaseSnakeInteractor snakeInteractor)
    {
        _entity = entity;
        _snakeInteractor = snakeInteractor;
        Entity = _entity.GetImmutableInstance();
    }

    private AIEntityMutable _entity;
    private BaseSnakeInteractor _snakeInteractor;
    private bool _isMinimizer;
    public AIEntity Entity { get; private set; }

    public Vector2 ChoseMoveDirection(){
        StateTreeNode chosenState = null;
        foreach (var stateNode in _entity.GetTransitionOptions()){
            if (chosenState == null){
                chosenState = stateNode;
                continue;
            }
            if (chosenState.HeuristicEvaluation < stateNode.HeuristicEvaluation)
                chosenState = stateNode;
        }
        return chosenState.AIVisitedPositions.Last() - _snakeInteractor.Entity.CurentPosition;
    }
}
