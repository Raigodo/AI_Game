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

    public Action<StateTreeNode> OnDisplayStateEvent;

    public void DisplayState(StateTreeNode node){
        OnDisplayStateEvent?.Invoke(node);
    }
}
