using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SnakeExample : MonoBehaviour
{
    private const float SNAKE_HEAD_SIZE_PERCENTAGE_OF_CELL = 1.1f;
    private const float SNAKE_BODY_SIZE_PERCENTAGE_OF_CELL = 0.8f;
    private readonly WaitForFixedUpdate ROUTIME_WAIT_TIME = new WaitForFixedUpdate();


    [Header("Prefabs")]
    [SerializeField, InspectorName("Body Prefab")] private RectTransform _atomicSnakeBodyPrefab;
    [SerializeField, InspectorName("Head Prefab")] private RectTransform _snakeHeadPrefab;
    [SerializeField, InspectorName("Map Example")]  private MapExample _mapExample;
    [Header("Dynamic Properties")]
    [SerializeField, InspectorName("Is Player")] private bool _isPlayer;
    [SerializeField, InspectorName("Snake Color")] private Color _bodyColor;
    private MapInteractor _mapInteractor;
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
        var currentNode = Game.Instance.GetInteractor<StateTreeInteractor>().Entity.CurrentStateNode;
        _mapInteractor = Game.Instance.GetInteractor<MapInteractor>();
        _spawnPosition = _isPlayer ? currentNode.PlayerVisitedPositions.First() : currentNode.AIVisitedPositions.First();
        _mapInteractor.OnDisplayStateEvent += DisplayStateActor;
    }
    private void OnCombatEnded(){
        _mapInteractor.OnDisplayStateEvent -= DisplayStateActor;
        DeleteAllBodyParts();
    }


    public void DisplayStateActor(StateTreeNode node){
        DeleteAllBodyParts();
        SpawnAllBodyParts(node);
    }

    private void SpawnNewBodyPart(RectTransform prefab, float sizePercentage, Vector2 position){
        RectTransform bodyPart = Instantiate(prefab).transform as RectTransform;
        bodyPart.SetParent(transform);
        bodyPart.localScale = Vector3.one;
        bodyPart.GetComponent<Image>().color = _bodyColor;
        bodyPart.localPosition = _mapExample.ToWorldPosition(position);
        bodyPart.sizeDelta = Vector2.one * (_mapExample.cellSize * sizePercentage);
    }
    private void SpawnAllBodyParts(StateTreeNode node){
        var visitedPositions = _isPlayer ? node.PlayerVisitedPositions : node.AIVisitedPositions;
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
