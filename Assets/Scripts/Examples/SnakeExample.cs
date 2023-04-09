using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeExample : MonoBehaviour
{
    private const float SNAKE_HEAD_SIZE_PERCENTAGE_OF_CELL = 0.9f;
    private const float SNAKE_BODY_SIZE_PERCENTAGE_OF_CELL = 0.7f;
    private const float SNAKE_ATOMIC_MOVE_TIME = 100f; //move multiplier
    private readonly WaitForFixedUpdate ROUTIME_WAIT_TIME = new WaitForFixedUpdate();


    [SerializeField, InspectorName("Body Prefab")] private RectTransform _atomicSnakeBodyPrefab;
    [SerializeField, InspectorName("Head Prefab")] private RectTransform _snakeHeadPrefab;
    [SerializeField, InspectorName("Map Example")]  private MapExample _mapExample;
    [SerializeField, InspectorName("Is Player")] private bool _isPlayer;
    private BaseSnakeInteractor _snakeInteractor;
    private CombatInteractor _combatInteractor;
    private Vector2 _spawnPosition;
    private Coroutine _coroutine_UpdateSnake;

    public void OnEnable(){
        Game.OnCombatStartedEvent += OnCombatStarted;
        Game.OnCombatEndedEvent += OnCombatEnded;

    }
    public void OnDisable(){
        Game.OnCombatStartedEvent -= OnCombatStarted;
        Game.OnCombatEndedEvent -= OnCombatEnded;
    }

    private void OnCombatStarted(){
        _combatInteractor = Game.Instance.GetInteractor<CombatInteractor>();
        if (_isPlayer) _snakeInteractor = Game.Instance.GetInteractor<PlayerInteractor>();
        else _snakeInteractor = Game.Instance.GetInteractor<EnemyInteractor>();
        _spawnPosition = _snakeInteractor.Entity.CurentPosition;
        SpawnNewBodyPart(_snakeHeadPrefab, SNAKE_HEAD_SIZE_PERCENTAGE_OF_CELL);

        _combatInteractor.OnSnakeMovedEvent += OnAnySnakeMoved;
    }
    private void OnCombatEnded(){
        _snakeInteractor = null;
        _combatInteractor.OnSnakeMovedEvent -= OnAnySnakeMoved;
    }
    
    private void OnAnySnakeMoved(Vector2 direction){
        if (_combatInteractor.CurrentTurnSnake != _snakeInteractor)
            return;
        if (_coroutine_UpdateSnake != null)
            return;

        _coroutine_UpdateSnake = StartCoroutine(UpdateSnakeRoutine());
    }

    private void SpawnNewBodyPart(RectTransform prefab, float sizePercentage){
        RectTransform bodyPart = Instantiate(prefab).transform as RectTransform;
        bodyPart.SetParent(transform);
        bodyPart.localScale = Vector3.one;
        bodyPart.localPosition = _mapExample.ToWorldPosition(_spawnPosition);
        bodyPart.sizeDelta = Vector2.one * (_mapExample.cellSize * sizePercentage);
    }
    private void DeleteLastBodyPart(){

    }

    private IEnumerator UpdateSnakeRoutine(){
        var nextHeadWorldPosition = _mapExample.ToWorldPosition(_snakeInteractor.Entity.CurentPosition);
        while (nextHeadWorldPosition != (Vector2)transform.GetChild(0).localPosition){
            Transform bodyPart;
            Vector2 nextWorldPosition;
            int i = 0;
            do{
                bodyPart = transform.GetChild(i);
                nextWorldPosition = _mapExample.ToWorldPosition(
                    _snakeInteractor.Entity.VisitedPositions.ElementAt(_snakeInteractor.Entity.VisitedPositionsCount-1-i));
                bodyPart.localPosition = Vector2.MoveTowards(bodyPart.localPosition, nextWorldPosition, Time.fixedDeltaTime * SNAKE_ATOMIC_MOVE_TIME);
                i++;
            }while(i < transform.childCount);
            yield return ROUTIME_WAIT_TIME;
        }
        _combatInteractor.OnSnakeInteractorFinishedUpdateRoutine();
        _coroutine_UpdateSnake = null;
    }
}
