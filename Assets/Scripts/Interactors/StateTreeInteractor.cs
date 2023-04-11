using System.Collections;
using System.Collections.Generic;
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
}
