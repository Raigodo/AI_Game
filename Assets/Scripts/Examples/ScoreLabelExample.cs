using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLabelExample : MonoBehaviour
{
    [SerializeField, InspectorName("Is Player Score")] private bool _isPlayerScore;
    private CombatInteractor _combatInteractor;
    private StateTree _tree;

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
        _tree = Game.Instance.GetInteractor<StateTreeInteractor>().Entity;
        _combatInteractor.OnMoveActionPerformedEvent += UpdateLabelText;
    }
    public void OnCombatEnded(){
        _combatInteractor.OnMoveActionPerformedEvent -= UpdateLabelText;
        _combatInteractor = null;
        _tree = null;
    }

    private Text _text;

    private void UpdateLabelText(){
        _text.text = $"{(_isPlayerScore ? _tree.CurrentStateNode.PlayerScore :  _tree.CurrentStateNode.AIScore)}";
    }
}
