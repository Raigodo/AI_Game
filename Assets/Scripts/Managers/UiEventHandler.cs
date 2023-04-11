using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiEventHandler : MonoBehaviour
{
    public static UiEventHandler Instance;
    [SerializeField] private GameObject _homeUI;
    [SerializeField] private GameObject _combatResults;
    [SerializeField] private GameObject _combatPanel;
    [SerializeField] private GameObject _treePanel;

    private GameObject _previousPanel;
    private SessionConfInteractor _confInteractor;


    public void OnEnable(){
        Instance = this;
        Game.OnCombatStartedEvent += OnCombatStarted;
        Game.OnCombatFinished_ConfirmationPendingEvent += OnCombatFinishedAndPendingConfirmation;
        Game.OnCombatEndedEvent += OnConfirmCombatEnded;
    }
    public void OnDisable(){
        Game.OnCombatStartedEvent -= OnCombatStarted;
        Game.OnCombatFinished_ConfirmationPendingEvent -= OnCombatFinishedAndPendingConfirmation;
        Game.OnCombatEndedEvent -= OnConfirmCombatEnded;
        Instance = null;
    }

    public void Start(){
        _confInteractor = Game.Instance.GetInteractor<SessionConfInteractor>();
    }


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

    public void OnConfirmEndCombat() => Game.Instance.OnEndCombatConfirmed();

    public void OnMoveButton(Vector2 direction){
        var combatInteractor = Game.Instance.GetInteractor<CombatInteractor>();
        combatInteractor.PlayerMoveInput(direction);
    }

    public void OnViewTreeViewPanel_FromCombat(){
        _previousPanel = _combatPanel;
        _combatPanel.SetActive(false);
        _treePanel.SetActive(true);
    }
    public void OnViewTreePanel_FromResulting(){
        _previousPanel = _combatResults;
        _combatResults.SetActive(false);
        _treePanel.SetActive(true);
    }
    public void OnHideTreePanel(){
        _previousPanel.SetActive(true);
        _treePanel.SetActive(false);
        var currentState = Game.Instance.GetInteractor<StateTreeInteractor>().Entity.CurrentStateNode;
        Game.Instance.GetInteractor<MapInteractor>().DisplayState(currentState);
    }

    private void OnCombatStarted(){
        _homeUI.SetActive(false);
        _combatPanel.SetActive(true);
    }
    private void OnCombatFinishedAndPendingConfirmation(){
        _combatResults.SetActive(true);
        _combatPanel.SetActive(false);
    }
    private void OnConfirmCombatEnded(){
        _homeUI.SetActive(true);
        _combatResults.SetActive(false);
    }

    public void ShowStateNodeExampleOnMap(StateTreeNode node){
        Game.Instance.GetInteractor<MapInteractor>().DisplayState(node);
    }

    public void Update(){
        if (Input.GetKeyDown(KeyCode.Space) && Game.Instance.CombatInProgress){
            var _combatInteractor = Game.Instance.GetInteractor<CombatInteractor>();
            _combatInteractor.ProcessAITurn();
        }
    }
}
