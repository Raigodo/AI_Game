using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingTurnsLabelExample : MonoBehaviour
{
    void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void OnEnable(){
        _combatInteractor = Game.Instance.GetInteractor<CombatInteractor>();
        _confInteractor = Game.Instance.GetInteractor<SessionConfInteractor>();
        _combatInteractor.OnMoveActionPerformedEvent += UpdateLabelText;
    }
    public void OnDisable(){
        _combatInteractor.OnMoveActionPerformedEvent -= UpdateLabelText;
        _combatInteractor = null;
    }

    public void Start(){
        UpdateLabelText();
    }

    private Text _text;
    private CombatInteractor _combatInteractor;
    private SessionConfInteractor _confInteractor;

    private void UpdateLabelText(){
        _text.text = $"Moves left: {_combatInteractor.RemainingTurns}";
    }
}
