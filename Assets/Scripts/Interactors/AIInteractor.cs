using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIInteractor : BaseInteractor
{
    public AIInteractor(AIEntityMutable entity)
    {
        _entity = entity;
        Entity = _entity.GetImmutableInstance();
    }

    private AIEntityMutable _entity;
    public AIEntity Entity { get; private set; }

    public Vector2 ChoseNewPosition(){
        var transitionOptions = _entity.GetTransitionOptions();
        StateTreeNode chosenState = null;
        foreach (var stateNode in transitionOptions){
            if (chosenState == null){
                chosenState = stateNode;
                continue;
            }
            if (chosenState.HeuristicEvaluation > stateNode.HeuristicEvaluation) //AI is minimizer
                chosenState = stateNode;
        }
        return chosenState.AIVisitedPositions.Last();
    }

    private Vector2 CanlculateDirection(Vector2 from, Vector2 to){
        return to-from;
    }
}
