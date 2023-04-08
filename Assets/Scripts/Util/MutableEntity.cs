using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MutableEntity<T>
{
    public abstract T GetImmutableInstance();
}
