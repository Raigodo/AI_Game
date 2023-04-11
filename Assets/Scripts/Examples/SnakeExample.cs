using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeExample : MonoBehaviour
{
    private const float SNAKE_HEAD_SIZE_PERCENTAGE_OF_CELL = 0.9f;
    private const float SNAKE_BODY_SIZE_PERCENTAGE_OF_CELL = 0.7f;
    private readonly WaitForFixedUpdate ROUTIME_WAIT_TIME = new WaitForFixedUpdate();


    [SerializeField, InspectorName("Body Prefab")] private RectTransform _atomicSnakeBodyPrefab;
    [SerializeField, InspectorName("Head Prefab")] private RectTransform _snakeHeadPrefab;
    [SerializeField, InspectorName("Map Example")]  private MapExample _mapExample;
    [SerializeField, InspectorName("Is Player")] private bool _isPlayer;
    private CombatInteractor _combatInteractor;
    private StateTreeInteractor _treeInteractor;
    private Vector2 _spawnPosition;

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
        _treeInteractor = Game.Instance.GetInteractor<StateTreeInteractor>();
        _spawnPosition = _isPlayer ? _treeInteractor.Entity.CurrentStateNode.PlayerVisitedPositions.First()
            : _treeInteractor.Entity.CurrentStateNode.AIVisitedPositions.First();
        SpawnNewBodyPart(_snakeHeadPrefab, SNAKE_HEAD_SIZE_PERCENTAGE_OF_CELL, _spawnPosition);

        _combatInteractor.OnMoveActionPerformedEvent += UpdateSnakePositions;
    }
    private void OnCombatEnded(){
        _combatInteractor.OnMoveActionPerformedEvent -= UpdateSnakePositions;
        _combatInteractor = null;
        _treeInteractor = null;
        DeleteAllBodyParts();
    }

    private void UpdateSnakePositions(){
        DeleteAllBodyParts();
        SpawnAllBodyParts();
    }

    private void SpawnNewBodyPart(RectTransform prefab, float sizePercentage, Vector2 position){
        RectTransform bodyPart = Instantiate(prefab).transform as RectTransform;
        bodyPart.SetParent(transform);
        bodyPart.localScale = Vector3.one;
        bodyPart.localPosition = _mapExample.ToWorldPosition(position);
        bodyPart.sizeDelta = Vector2.one * (_mapExample.cellSize * sizePercentage);
    }
    private void SpawnAllBodyParts(){
        var visitedPositions = _isPlayer ? _treeInteractor.Entity.CurrentStateNode.PlayerVisitedPositions 
            : _treeInteractor.Entity.CurrentStateNode.AIVisitedPositions;
        SpawnNewBodyPart(_snakeHeadPrefab, SNAKE_HEAD_SIZE_PERCENTAGE_OF_CELL, visitedPositions.Last());
        for (int i=0; i<visitedPositions.Count()-1; i++){
            SpawnNewBodyPart(_atomicSnakeBodyPrefab, SNAKE_BODY_SIZE_PERCENTAGE_OF_CELL, visitedPositions.ElementAt(i));
        }
    }
    private void DeleteAllBodyParts(){
        for (int i=transform.childCount-1; i>=0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    
}
