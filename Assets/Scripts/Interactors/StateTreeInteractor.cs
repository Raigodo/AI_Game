using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateTreeInteractor : BaseInteractor
{
    public StateTreeInteractor(StateTreeMutable tree)
    {
        _entity = tree;
        Entity = new StateTree(_entity);
    }

    private StateTreeMutable _entity;
    public StateTree Entity {get; private set;}

    public void TransitionToState(Vector2 newPosition, bool isPlayerTurn){
        _entity.TransitionToState(newPosition, isPlayerTurn);
    }

    public bool IsDirectionValid(Vector2 direction, bool isItValidForPlayer){
        var currentPosition = isItValidForPlayer ? _entity.CurrentStateNode.PlayerVisitedPositions.Last() 
            : _entity.CurrentStateNode.AIVisitedPositions.Last();
        var possibleStatesToTransition = _entity.CurrentStateNode.Children;
        Vector2 childCurrentPosition;
        foreach (var child in possibleStatesToTransition){
            childCurrentPosition = isItValidForPlayer ? child.PlayerVisitedPositions.Last() : child.PlayerVisitedPositions.Last();
            if (direction.Equals(childCurrentPosition - currentPosition)){
                return true;
            }
        }
        return false;
    }
}
