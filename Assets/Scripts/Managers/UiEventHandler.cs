using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject _homeUI;
    [SerializeField] private GameObject _combatUI;


    public void OnEnable(){
        Game.OnCombatStartedEvent += OnCombatStarted;
        Game.OnCombatEndedEvent += OnCombatEnded;
    }
    public void OnDisable(){
        Game.OnCombatStartedEvent -= OnCombatStarted;
        Game.OnCombatEndedEvent -= OnCombatEnded;
    }

    public void Start(){
        _confInteractor = Game.Instance.GetInteractor<SessionConfInteractor>();
    }

    private SessionConfInteractor _confInteractor;

    public void OnFlipStartingSideButton(){
        _confInteractor.FlipStartingSide();
    }

    public void OnStartCombatButton(){
        Game.Instance.TryStartCombat();
    }

    public void OnMoveUpButton() => OnMoveButton(Vector2.up);
    public void OnMoveDownButton() => OnMoveButton(Vector2.down);
    public void OnMoveRightButton() => OnMoveButton(Vector2.right);
    public void OnMoveLeftButton() => OnMoveButton(Vector2.left);

    public void OnMoveButton(Vector2 direction){
        var combatInteractor = Game.Instance.GetInteractor<CombatInteractor>();
        combatInteractor.TryMoveSnake(direction);
    }

    
    private void OnCombatStarted(){
        _homeUI.SetActive(false);
        _combatUI.SetActive(true);
    }
    private void OnCombatEnded(){
        _homeUI.SetActive(true);
        _combatUI.SetActive(false);
    }
}
