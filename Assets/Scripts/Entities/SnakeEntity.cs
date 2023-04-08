using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeEntityMutable : MutableEntity<SnakeEntity>
{
    public SnakeEntityMutable(Vector2 startingPosition){
        VisitedPositions = new List<Vector2>{startingPosition};
    }
    public List<Vector2> VisitedPositions {get; private set;}
    public Vector2 CurentPosition => VisitedPositions[VisitedPositions.Count-1];

    public override SnakeEntity GetImmutableInstance() => new SnakeEntity(this);
}

public class SnakeEntity{
    public SnakeEntity(SnakeEntityMutable value){
        _value = value;
    }

    private SnakeEntityMutable _value;
    public IEnumerable<Vector2> VisitedPositions => _value.VisitedPositions;
    public Vector2 CurentPosition => _value.VisitedPositions[_value.VisitedPositions.Count-1];
}