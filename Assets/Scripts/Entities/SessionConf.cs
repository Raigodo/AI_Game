using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionConfMutable : MutableEntity<SessionConf>
{
    public bool IsPlayerStarting = true;
    public int FoodCount = 5;
    public int Dimensions = 5;
    public int MaxTurnCount = 7;

    public override SessionConf GetImmutableInstance() => new SessionConf(this);
}


public class SessionConf{
    public SessionConf(SessionConfMutable value){
        _value = value;
    }
    private SessionConfMutable _value;
    public bool IsPlayerStarting => _value.IsPlayerStarting;
    public int FoodCount => _value.FoodCount;
    public int Dimensions => _value.Dimensions;
    public int MaxTurnCount => _value.MaxTurnCount;
}
