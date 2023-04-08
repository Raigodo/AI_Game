using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInteractor : BaseInteractor
{
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
    private bool _isPlayerTurn;
    public BaseSnakeInteractor CurrentTurnSnake {get; private set;}
    public int PlayerScore { get; private set; } = 0;
    public int EnemyScore { get; private set; } = 0;

    public void TryMoveSnake(Vector2 direction){
        BaseSnakeInteractor actor;
        if (_isPlayerTurn)
            actor = _playerInteractor;
        else
            actor = _enemyInteractor;

        if (!actor.IsDirectionAllowedToMove(direction))
            return;
        
        actor.Move(direction);
        CheckForFood();
        EndCurrentTurn();
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
        }
    }



}
