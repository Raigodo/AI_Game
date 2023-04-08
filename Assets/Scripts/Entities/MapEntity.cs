using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEntityMutable : MutableEntity<MapEntity>
{
    public MapEntityMutable(int dimensions, List<Vector2> foodPositions, (Vector2, Vector2) spawnPositions){
        Dimensions = dimensions;
        FoodPositions = foodPositions;
        SpawnPositions = spawnPositions;
    }
    public int Dimensions {get; private set;}
    public List<Vector2> FoodPositions {get; private set;}
    public (Vector2 first, Vector2 second) SpawnPositions {get; private set;} //(minimizer, maximizer)

    public override MapEntity GetImmutableInstance() => new MapEntity(this);
}

public class MapEntity{
    public MapEntity(MapEntityMutable value)
    {
        _value = value;
    }

    private MapEntityMutable _value;
    public int Dimensions => _value.Dimensions;
    public IEnumerable<Vector2> FoodPositions => _value.FoodPositions;
    public (Vector2 first, Vector2 second) SpawnPositions => _value.SpawnPositions;
}