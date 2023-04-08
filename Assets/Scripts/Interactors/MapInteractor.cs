using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapInteractor : BaseInteractor
{
    public MapInteractor(MapEntityMutable entity)
    {
        _entity = entity;
        Entity = _entity.GetImmutableInstance();
    }

    private MapEntityMutable _entity;
    public MapEntity Entity { get; private set; }

    public bool IsFoodAtPosition(Vector2 checkPosition){
        if (_entity.FoodPositions.Contains(checkPosition))
            return true;
        return false;
    }

    public Action<Vector2> OnRemoveFoodEvent;

    public void RemoveFoodAt(Vector2 position){
        _entity.FoodPositions.Remove(position);
    }
}
