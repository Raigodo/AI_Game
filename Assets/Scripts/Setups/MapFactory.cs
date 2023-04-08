using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapFactory
{
    public static MapEntityMutable Create(int dimenstions, int foodCount){
        List<Vector2> foodPositions = new List<Vector2> {
            new Vector2(0, 0),
            new Vector2(0, dimenstions-1),
            new Vector2(dimenstions-1, 0),
            new Vector2(dimenstions-1, dimenstions-1),
            new Vector2Int( 0, Mathf.CeilToInt((dimenstions-1)/2) ),
            new Vector2Int( dimenstions-1, Mathf.CeilToInt(dimenstions/2) ),
            new Vector2Int( Mathf.CeilToInt((dimenstions-1)/2), Mathf.CeilToInt((dimenstions-1)/2) )
        };
        var spawnPositions = (new Vector2(Mathf.CeilToInt(dimenstions/2f), 0), new Vector2(Mathf.CeilToInt(dimenstions/2f), dimenstions-1));
        return new MapEntityMutable(dimenstions, foodPositions, spawnPositions);
    }
}
