using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingTurnsLabelExample : MonoBehaviour
{
    private Text _text;
    
    void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void OnEnable(){
        var combatInteractor = Game.Instance.GetInteractor<CombatInteractor>();
        combatInteractor.OnRemainingTurnsChangedEvent += UpdateLabelText;
        UpdateLabelText(combatInteractor.RemainingTurns);
    }
    public void OnDisable(){
        var combatInteractor = Game.Instance.GetInteractor<CombatInteractor>();
        combatInteractor.OnRemainingTurnsChangedEvent -= UpdateLabelText;
    }


    private void UpdateLabelText(int remainingTurns){
        _text.text = $"Moves left: {remainingTurns}";
    }
}
