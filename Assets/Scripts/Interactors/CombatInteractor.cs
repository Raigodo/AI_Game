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
        EnemyInteractor enemyInteractor
    )
    {
        Game I = Game.Instance;
        _playerInteractor = playerInteractor;
        _enemyInteractor = enemyInteractor;
        CurrentTurnSnake = confInteractor.Entity.IsPlayerStarting ? _playerInteractor : _enemyInteractor as BaseSnakeInteractor;
        _mapInteractor = mapInteractor;
    }


    private PlayerInteractor _playerInteractor;
    private EnemyInteractor _enemyInteractor;
    private MapInteractor _mapInteractor;
    private bool _exampleIsUpdating = false;
    private Coroutine _coroutine_endTurnPending; 

    public BaseSnakeInteractor CurrentTurnSnake {get; private set;}
    public int PlayerScore { get; private set; } = 0;
    public int EnemyScore { get; private set; } = 0;
    

    public Action<Vector2> OnSnakeMovedEvent;
    public Action OnAnyScoreChangedEvent;


    public void TryMoveSnake(Vector2 direction){

        if (!CurrentTurnSnake.IsDirectionAllowedToMove(direction))
            return;
        if (_exampleIsUpdating)
            return;
        
        CurrentTurnSnake.Move(direction);
        _exampleIsUpdating = true;
        OnSnakeMovedEvent?.Invoke(direction);
        _coroutine_endTurnPending = Game.Instance.StartCoroutine(StartPendingEndTurnRoutine());
    }

    private void EndCurrentTurn(){
        if (CurrentTurnSnake == _playerInteractor)
            CurrentTurnSnake = _enemyInteractor;
        else
            CurrentTurnSnake = _playerInteractor;
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
        // EndCurrentTurn();
    }

    public void OnSnakeInteractorFinishedUpdateRoutine(){
        _exampleIsUpdating = false;
    }


}
