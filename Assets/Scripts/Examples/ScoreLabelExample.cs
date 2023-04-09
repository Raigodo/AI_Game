using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLabelExample : MonoBehaviour
{
    [SerializeField, InspectorName("Is Player Score")] private bool _isPlayerScore;
    private CombatInteractor _combatInteractor;

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
        _combatInteractor = Game.Instance.GetInteractor<CombatInteractor>();
        _combatInteractor.OnAnyScoreChangedEvent += UpdateLabelText;
    }
    public void OnCombatEnded(){
        _combatInteractor.OnAnyScoreChangedEvent -= UpdateLabelText;
        _combatInteractor = null;
    }

    private Text _text;

    private void UpdateLabelText(){
        _text.text = $"{(_isPlayerScore ? _combatInteractor.PlayerScore : _combatInteractor.EnemyScore)}";
    }
}
