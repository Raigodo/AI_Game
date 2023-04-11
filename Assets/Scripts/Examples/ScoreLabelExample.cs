using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLabelExample : MonoBehaviour
{
    [SerializeField, InspectorName("Is Player Score")] private bool _isPlayerScore;
    private Text _text;

    void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void OnEnable(){
        //enables only when combat is already started
        OnCombatStarted();
    }
    public void OnDisable(){
        OnCombatEnded();
    }


    public void OnCombatStarted(){
        var mapInteractor = Game.Instance.GetInteractor<MapInteractor>();
        mapInteractor.OnDisplayStateEvent += UpdateLabelText;
    }
    public void OnCombatEnded(){
        var mapInteractor = Game.Instance.GetInteractor<MapInteractor>();
        mapInteractor.OnDisplayStateEvent -= UpdateLabelText;
    }


    private void UpdateLabelText(StateTreeNode node){
        _text.text = $"{(_isPlayerScore ? node.PlayerScore :  node.AIScore)}";
    }
}
