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

    public Action<int> OnRemainingTurnsChangedEvent;

    private MapInteractor _mapInteractor;
    private AIInteractor _aiInteractor;
    private StateTreeInteractor _treeInteractor;
    private Coroutine _coroutine_endTurnPending; 

    public bool IsPlayerTurn {get; private set;}
    public int MaxTurns { get; private set; }
    public int RemainingTurns { get; private set; }


    public override void OnCombatStarted()
    {
        base.OnCombatStarted();
        _mapInteractor.DisplayState(_treeInteractor.Entity.CurrentStateNode);
        if (!IsPlayerTurn)
            Timer.Instance.DoAfterTimer(()=>ProcessAITurn(), 0.2f);
    }


    public void PlayerMoveInput(Vector2 direction){
        if (!IsPlayerTurn) return;
        if (!_treeInteractor.IsDirectionValid(direction, true)){
            return;
        }
        _treeInteractor.TransitionToState(direction + _treeInteractor.Entity.CurrentStateNode.PlayerVisitedPositions.Last(), true);
        // Debug.Log($"Player move {_treeInteractor.Entity.CurrentStateNode.PlayerVisitedPositions.Last()}");
        RemainingTurns--;
        OnRemainingTurnsChangedEvent?.Invoke(RemainingTurns);
        IsPlayerTurn = false;
        _mapInteractor.DisplayState(_treeInteractor.Entity.CurrentStateNode);
        bool endSucceded = TryEndCombat();
        if (!endSucceded)
            Timer.Instance.DoAfterTimer(()=>ProcessAITurn(), 0.2f);
    }

    private void ProcessAITurn(){
        if (IsPlayerTurn) return;
        _treeInteractor.TransitionToState(_aiInteractor.ChoseNewPosition(), isPlayerTurn:false);
        // Debug.Log($"AI move {_treeInteractor.Entity.CurrentStateNode.AIVisitedPositions.Last()}");
        RemainingTurns--;
        OnRemainingTurnsChangedEvent?.Invoke(RemainingTurns);
        IsPlayerTurn = true;
        _mapInteractor.DisplayState(_treeInteractor.Entity.CurrentStateNode);
        TryEndCombat();
    }

    public bool TryEndCombat(){
        if (_treeInteractor.Entity.CurrentStateNode.Children.Count() > 0)
            return false;
        EndCombat();
        return true;
    }

    private void EndCombat(){
        Game.Instance.TryEndCombat();
    }


    
}
