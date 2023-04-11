using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultHeaderExample : MonoBehaviour
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
        int playerScore = currentNode.PlayerScore;
        int aiScore = currentNode.AIScore;
        if (aiScore == playerScore)
            _text.text = "Draw";
        else if (aiScore > playerScore)
            _text.text = "You Lose";
        else
            _text.text = "You Win";
    }
}
