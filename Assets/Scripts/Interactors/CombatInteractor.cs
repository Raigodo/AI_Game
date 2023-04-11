using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatInteractor : BaseInteractor
{
    private readonly WaitForFixedUpdate ROUTIME_WAIT_TIME = new WaitForFixedUpdate();

    public CombatInteractor(
        MapInteractor mapInteractor,
        SessionConfInteractor confInteractor,
        StateTreeInteractor treeInteractor,
        AIInteractor aiInteractor
    )
    {
        Game I = Game.Instance;
        IsPlayerTurn = confInteractor.Entity.IsPlayerStarting;
        _mapInteractor = mapInteractor;
        MaxTurns = confInteractor.Entity.MaxTurnCount;
        RemainingTurns = MaxTurns;
        _aiInteractor = aiInteractor;
        _treeInteractor = treeInteractor;
    }


    private MapInteractor _mapInteractor;
    private AIInteractor _aiInteractor;
    private StateTreeInteractor _treeInteractor;
    private bool _exampleIsUpdating = false;
    private Coroutine _coroutine_endTurnPending; 

    public bool IsPlayerTurn {get; private set;}
    public int MaxTurns { get; private set; }
    public int RemainingTurns { get; private set; }

    

    public Action OnMoveActionPerformedEvent;


    public void PlayerMoveInput(Vector2 direction){
        if (!IsPlayerTurn) return;
        if (!_treeInteractor.IsDirectionValid(direction, true)){
            Debug.Log("invalid direction");
            return;
        }
        _treeInteractor.TransitionToState(direction + _treeInteractor.Entity.CurrentStateNode.PlayerVisitedPositions.Last(), true);
        Debug.Log($"Player move {_treeInteractor.Entity.CurrentStateNode.PlayerVisitedPositions.Last()}");
        RemainingTurns--;
        IsPlayerTurn = !IsPlayerTurn;
        OnMoveActionPerformedEvent?.Invoke();
        TryEndCombat();
    }


    public void ProcessAITurn(){
        if (IsPlayerTurn) return;
        _treeInteractor.TransitionToState(_aiInteractor.ChoseNewPosition(), isPlayerTurn:false);
        Debug.Log($"AI move {_treeInteractor.Entity.CurrentStateNode.AIVisitedPositions.Last()}");
        RemainingTurns--;
        IsPlayerTurn = !IsPlayerTurn;
        OnMoveActionPerformedEvent?.Invoke();
        TryEndCombat();
    }

    public void TryEndCombat(){
        if (_treeInteractor.Entity.CurrentStateNode.Children.Count() > 0)
            return;
        Debug.Log("end combat");
        EndCombat();
    }

    private void EndCombat(){
        Game.Instance.TryEndCombat();
    }


    
}
