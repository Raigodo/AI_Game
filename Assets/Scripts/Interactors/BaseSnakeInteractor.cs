using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSnakeInteractor : BaseInteractor
{
    public BaseSnakeInteractor(SnakeEntityMutable entity, MapInteractor mapInteractor)
    {
        _entity = entity;
        _mapInteractor = mapInteractor;
        Entity = _entity.GetImmutableInstance();
    }

    private SnakeEntityMutable _entity;
    private MapInteractor _mapInteractor;
    public SnakeEntity Entity { get; private set; }

    public bool IsDirectionAllowedToMove(Vector2 direction){
        var currentPos = _entity.CurentPosition;
        bool isOwnBodyAtPosition = _entity.VisitedPositions.Contains(_entity.CurentPosition + direction);
        bool isFacingMapRightBorder = currentPos.x == (_mapInteractor.Entity.Dimensions-1);
        bool isFacingMapLeftBorder = currentPos.x == 0;
        bool isFacingMapTopBorder = currentPos.y == (_mapInteractor.Entity.Dimensions-1);
        bool isFacingMapBottomBorder = currentPos.y == 0;

        if (isOwnBodyAtPosition 
            || (isFacingMapRightBorder && direction == Vector2.right)
            || (isFacingMapLeftBorder && direction == Vector2.left)
            || (isFacingMapTopBorder && direction == Vector2.up)
            || (isFacingMapBottomBorder && direction == Vector2.down))
            return false;
        return true;
    }

    public void Move(Vector2 direction){
        Vector2 targetPosition = _entity.CurentPosition + direction;
        Debug.Log(_entity.VisitedPositions.AsString());
        if (!IsDirectionAllowedToMove(direction))
            throw new System.Exception($"invalid direction, pointing to already visited position ({targetPosition})");
        
        _entity.VisitedPositions.Add(targetPosition);
    }
}
