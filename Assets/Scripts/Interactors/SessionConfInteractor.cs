using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SessionConfInteractor : BaseInteractor
{
    public SessionConfInteractor(SessionConfMutable entity)
    {
        _entity = entity;
        Entity = _entity.GetImmutableInstance();
    }

    private SessionConfMutable _entity;
    public SessionConf Entity {get; private set;}
    
    public Action<bool> OnStartingSideChangedEvent;

    public void FlipStartingSide(){
        _entity.IsPlayerStarting = !_entity.IsPlayerStarting;
        OnStartingSideChangedEvent?.Invoke(_entity.IsPlayerStarting);
    }
}
