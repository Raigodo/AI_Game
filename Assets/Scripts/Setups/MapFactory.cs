using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapFactory
{
    public static MapEntityMutable Create(int dimenstions, int foodCount){
        List<Vector2> foodPositions = new List<Vector2>();
        var spawnPositions = (new Vector2(Mathf.CeilToInt((dimenstions-1)/2f), 0), new Vector2(Mathf.CeilToInt((dimenstions-1)/2f), dimenstions-1));

        for (int i=0; i<foodCount; i++)
            foodPositions.Add(GetRandomFreePosition(foodPositions, spawnPositions.Item1, spawnPositions.Item2, dimenstions));
        return new MapEntityMutable(dimenstions, foodPositions, spawnPositions);
    }

    private static Vector2 GetRandomFreePosition(IEnumerable<Vector2> usedPositions, 
        Vector2 firstSpawnpoint, Vector2 secondSpawnpoint, int dimensions){
        Vector2 newPos = new Vector2(Random.Range(0, dimensions-1), Random.Range(0, dimensions-1));

        while(usedPositions.Contains(newPos) || newPos == firstSpawnpoint || newPos == secondSpawnpoint)
            newPos = new Vector2(Random.Range(0, dimensions-1), Random.Range(0, dimensions-1));
        
        return newPos;
    }
}
