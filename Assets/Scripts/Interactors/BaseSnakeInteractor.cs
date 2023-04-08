using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSnakeInteractor : BaseInteractor
{
    public BaseSnakeInteractor(SnakeEntityMutable entity)
    {
        _entity = entity;
        Entity = _entity.GetImmutableInstance();
    }

    private SnakeEntityMutable _entity;
    public SnakeEntity Entity { get; private set; }

    public bool IsDirectionAllowedToMove(Vector2 direction){
        List<Vector2> VisitedPositions = _entity.VisitedPositions;
        var currentPos = VisitedPositions[VisitedPositions.Count-1];
        if (_entity.VisitedPositions.Contains(direction))
            return false;
        return true;
    }

    public void Move(Vector2 direction){
        Vector2 targetPosition = _entity.CurentPosition + direction;
        if (!IsDirectionAllowedToMove(direction))
            throw new System.Exception($"invalid direction, pointing to already visited position ({targetPosition})");
        
        _entity.VisitedPositions.Add(targetPosition);
    }
}
