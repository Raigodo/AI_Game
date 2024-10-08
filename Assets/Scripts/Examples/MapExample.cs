using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExample : MonoBehaviour
{
    private const float CELL_PADDING_PERCENTAGE = 0.15f;
    private const float FOOD_SIZE_PERCENTAGE_OF_CELL = 0.5f;

    [SerializeField, InspectorName("Cell Prefab")] 
    private MonoBehaviour _cellPrefab;
    [SerializeField, InspectorName("Food Prefab")] 
    private MonoBehaviour _foodPrefab;
    [SerializeField, InspectorName("Cell's holder")] 
    private RectTransform _cellHolder;
    [SerializeField, InspectorName("Food holder")] 
    private RectTransform _foodHolder;
    
    private Dictionary<Vector2, GameObject> SpawnedFoodMap = new Dictionary<Vector2, GameObject>();
    private Rect ringSize;
    private StateTree _tree;
    private MapInteractor _mapInteractor;
    public float cellPadding {get; private set;}
    public float cellSize {get; private set;}
    

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
        _tree = Game.Instance.GetInteractor<StateTreeInteractor>().Entity;
        _mapInteractor = Game.Instance.GetInteractor<MapInteractor>();
        _mapInteractor.OnDisplayStateEvent += DisplayState;
    }
    public void OnCombatEnded(){
        _mapInteractor.OnDisplayStateEvent -= DisplayState;
        _mapInteractor = null;
        _tree = null;
        RemoveAllFood();
    }

    public void DisplayState(StateTreeNode node){
        RemoveAllFood();
        SpawnAllStateFood(node);
    }

    private void SpawnAllStateFood(StateTreeNode node){
        foreach (Vector2 foodPosition in node.FoodPositions){
            RectTransform food = Instantiate(_foodPrefab).transform as RectTransform;
            food.SetParent(_foodHolder);
            food.localScale = Vector3.one;
            food.sizeDelta = new Vector2(cellSize*0.5f, cellSize*0.5f);
            food.localPosition = ToWorldPosition(foodPosition);
            SpawnedFoodMap.Add(foodPosition, food.gameObject);
        }
    }
    private void RemoveAllFood(){
        foreach (var foodExample in SpawnedFoodMap.Values)
            Destroy(foodExample);
        SpawnedFoodMap.Clear();
    }

    public Vector2 ToWorldPosition(Vector2 position){
        return new Vector2(
                position.x * (cellSize + cellPadding) + cellPadding + cellSize*0.5f,
                position.y * (cellSize + cellPadding) + cellPadding + cellSize*0.5f
        );
    }
}
