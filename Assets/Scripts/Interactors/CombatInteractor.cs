using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInteractor : BaseInteractor
{
    private readonly WaitForFixedUpdate ROUTIME_WAIT_TIME = new WaitForFixedUpdate();

    public CombatInteractor(
        MapInteractor mapInteractor,
        SessionConfInteractor confInteractor,
        PlayerInteractor playerInteractor,
        EnemyInteractor enemyInteractor,
        AIInteractor aiInteractor
    )
    {
        Game I = Game.Instance;
        _playerInteractor = playerInteractor;
        _enemyInteractor = enemyInteractor;
        IsPlayerTurn = confInteractor.Entity.IsPlayerStarting;
        CurrentTurnSnake = IsPlayerTurn ? _playerInteractor : _enemyInteractor as BaseSnakeInteractor;
        _mapInteractor = mapInteractor;
        MaxTurns = confInteractor.Entity.MaxTurnCount;
        RemainingTurns = MaxTurns;
        _aiInteractor = aiInteractor;
    }


    private PlayerInteractor _playerInteractor;
    private EnemyInteractor _enemyInteractor;
    private MapInteractor _mapInteractor;
    private AIInteractor _aiInteractor;
    private bool _exampleIsUpdating = false;
    private Coroutine _coroutine_endTurnPending; 

    public BaseSnakeInteractor CurrentTurnSnake {get; private set;}
    public bool IsPlayerTurn {get; private set;}
    public int PlayerScore { get; private set; } = 0;
    public int EnemyScore { get; private set; } = 0;
    public int MaxTurns { get; private set; }
    public int RemainingTurns { get; private set; }

    

    public Action<Vector2> OnSnakeMovedEvent;
    public Action OnAnyScoreChangedEvent;
    public Action OnRemainingTurnsChangedEvent;


    public void TryMoveSnake(Vector2 direction){

        if (!CurrentTurnSnake.IsDirectionAllowedToMove(direction))
            return;
        if (_exampleIsUpdating)
            return;
        if (IsAttackMove(direction)){
            EndCombat();
            return;
        }
        
        
        CurrentTurnSnake.Move(direction);
        _exampleIsUpdating = true;
        OnSnakeMovedEvent?.Invoke(direction);
        _coroutine_endTurnPending = Game.Instance.StartCoroutine(StartPendingEndTurnRoutine());
    }

    private void EndCurrentTurn(){
        if (IsPlayerTurn){
            RemainingTurns -= 1;
            CurrentTurnSnake = _enemyInteractor;
        }
        else
            CurrentTurnSnake = _playerInteractor;
        OnRemainingTurnsChangedEvent?.Invoke();
        IsPlayerTurn = !IsPlayerTurn;

        if (!CurrentTurnSnake.HasPossibleMoves()){
            EndCurrentTurn();
            return;
        }
        
        if (!IsPlayerTurn) {
            Vector2 direction = _aiInteractor.ChoseMoveDirection();
            Debug.Log(direction);
            TryMoveSnake(direction);
        }
    }

    public override void OnCombatStarted()
    {
        base.OnCombatStarted();
    }

    private void CheckForFood(){
        if (_mapInteractor.Entity.FoodPositions.Contains(CurrentTurnSnake.Entity.CurentPosition)){
            _mapInteractor.RemoveFoodAt(CurrentTurnSnake.Entity.CurentPosition);
            PlayerScore += 1;
            OnAnyScoreChangedEvent?.Invoke();
        }
    }

    private IEnumerator StartPendingEndTurnRoutine(){
        while (_exampleIsUpdating){
            yield return ROUTIME_WAIT_TIME;
        }
        CheckForFood();
        EndCurrentTurn();
        ProcessEndCombatConditions();
    }

    public void OnSnakeInteractorFinishedUpdateRoutine(){
        _exampleIsUpdating = false;
    }

    private void ProcessEndCombatConditions(){
        if (RemainingTurns > 0)
            return;
        EndCombat();
    }

    private void EndCombat(){
        Game.Instance.TryEndCombat();
    }

    private bool IsAttackMove(Vector2 direction){
        BaseSnakeInteractor opponent = IsPlayerTurn ? _enemyInteractor : _playerInteractor as BaseSnakeInteractor;
        // Debug.Log(opponent.Entity.VisitedPositions.AsString());
        if (opponent.Entity.VisitedPositions.Contains(CurrentTurnSnake.Entity.CurentPosition + direction))
            return true;
        return false;
    }
}
