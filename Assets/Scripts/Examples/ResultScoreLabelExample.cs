using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultScoreLabelExample : MonoBehaviour
{
    [SerializeField, InspectorName("Is Player Score")] private bool _isPlayerScore;
    
    private Text _text;

    private void Awake(){
        _text = transform.GetComponent<Text>();
    }

    private void OnEnable(){
        //enables only when combat is already started
        UpdateScore();
    }


    public void UpdateScore(){
        var currentNode = Game.Instance.GetInteractor<StateTreeInteractor>().Entity.CurrentStateNode;
        _text.text = _isPlayerScore ? currentNode.PlayerScore.ToString() : currentNode.AIScore.ToString();
    }
}
