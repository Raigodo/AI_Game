using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExample : MonoBehaviour
{
    private const float CELL_PADDING_PERCENTAGE = 0.15f;

    [SerializeField, InspectorName("Cell Prefab")] 
    private MonoBehaviour _cellPrefab;
    [SerializeField, InspectorName("Food Prefab")] 
    private MonoBehaviour _foodPrefab;
    [SerializeField, InspectorName("Cell's holder")] 
    private RectTransform _cellHolder;
    [SerializeField, InspectorName("Food holder")] 
    private RectTransform _foodHolder;
    
    private MapInteractor _mapInteractor;
    private Dictionary<Vector2, GameObject> SpawnedFoodMap = new Dictionary<Vector2, GameObject>();
    private Rect ringSize;
    private float cellPadding;
    private float cellSize;
    

    public void SetupGrid(){
        int dimensions = Game.Instance.GetInteractor<SessionConfInteractor>().Entity.Dimensions;
        ringSize = _cellHolder.rect;
        cellPadding = (ringSize.width*CELL_PADDING_PERCENTAGE) / (dimensions+1);
        cellSize = ringSize.width*(1-CELL_PADDING_PERCENTAGE) / dimensions;

        for (var x=0; x<dimensions; x++)
            for (var y=0; y<dimensions; y++){
                RectTransform cell = Instantiate(_cellPrefab).transform as RectTransform;
                cell.SetParent(_cellHolder);
                cell.localScale = Vector3.one;
                cell.sizeDelta = new Vector2(cellSize, cellSize);
                cell.localPosition = new Vector2(
                    cellPadding*(1+x) + cellSize*x,
                    cellPadding*(1+y) + cellSize*y
                );
            }
    }

    public void Start(){
        SetupGrid();
    }

    public void OnEnable(){
        Game.OnCombatStartedEvent += OnCombatStarted;
        Game.OnCombatEndedEvent += OnCombatEnded;
    }
    public void OnDisable(){
        Game.OnCombatStartedEvent -= OnCombatStarted;
        Game.OnCombatEndedEvent -= OnCombatEnded;
    }


    public void OnCombatStarted(){
        if (_mapInteractor == null) 
            _mapInteractor = Game.Instance.GetInteractor<MapInteractor>();
        SpawnAllFood();
        SpawnAllPlayers();
        Game.OnCombatStartedEvent += OnCombatStarted;
        Game.OnCombatEndedEvent += OnCombatEnded;
        _mapInteractor.OnRemoveFoodEvent += OnRemoveFoodAt;
    }
    public void OnCombatEnded(){
        Game.OnCombatStartedEvent -= OnCombatStarted;
        Game.OnCombatEndedEvent -= OnCombatEnded;
        _mapInteractor.OnRemoveFoodEvent -= OnRemoveFoodAt;
    }

    private void SpawnAllFood(){
        foreach (Vector2 foodPosition in _mapInteractor.Entity.FoodPositions){
            RectTransform food = Instantiate(_foodPrefab).transform as RectTransform;
            food.SetParent(_foodHolder);
            food.localScale = Vector3.one;
            food.localPosition = new Vector2(
                foodPosition.x * (cellSize + cellPadding) + cellPadding + cellSize*0.5f,
                foodPosition.y * (cellSize + cellPadding) + cellPadding + cellSize*0.5f
            );
            SpawnedFoodMap.Add(foodPosition, food.gameObject);
        }
    }
    private void RemoveAllFood(){
        foreach (var foodExample in SpawnedFoodMap.Values)
            Destroy(foodExample);
        SpawnedFoodMap.Clear();
    }
    public void SpawnAllPlayers(){

    }

    public void OnRemoveFoodAt(Vector2 position){
        Destroy(SpawnedFoodMap[position]);
        SpawnedFoodMap.Remove(position);
    }
}
