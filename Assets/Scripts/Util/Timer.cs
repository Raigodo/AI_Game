using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance;


    void Start()
    {
        Instance = this;
    }

    public void DoAfterTimer(Action actionToDo, float secondsWait){
        StartCoroutine(DoAfterTimeRoutine(actionToDo, new WaitForSeconds(secondsWait)));
    }

    private IEnumerator DoAfterTimeRoutine(Action actionToDo, WaitForSeconds waitTime){
        yield return waitTime;
        actionToDo.Invoke();
    }
}
