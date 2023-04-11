using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
    public enum Entities{
        AI,
        Map,
        SessionConf
    }
    public bool CombatInProgress { get; private set; } = false;


    public static Action OnCombatStartedEvent;
    public static Action OnCombatEndedEvent;


    public static Game Instance;

    private Dictionary<Entities, object> _entitiesMap = new Dictionary<Entities, object>();
    private Dictionary<Type, BaseInteractor> _interactorsMap = new Dictionary<Type, BaseInteractor>();

   public T GetInteractor<T>() where T : BaseInteractor
    {
        if (_interactorsMap.TryGetValue(typeof(T), out BaseInteractor interactor))
            return (T)interactor;
        throw new ArgumentException($"No interactor found for type {typeof(T).Name} ", nameof(T));
    }



    public void Awake(){
        Instance = this;
        
        var entity = new SessionConfMutable();
        _entitiesMap.Add(Entities.SessionConf, entity);
        _interactorsMap.Add(typeof(SessionConfInteractor), new SessionConfInteractor(entity));
    }


    private void OnCombatStarted(){
        SetupOnGameStartEntitesAndInteractors();
        OnCombatStartedEvent?.Invoke();
        foreach (var interactor in _interactorsMap.Values)
            interactor.OnCombatStarted();
    }
    private void OnCombatEnded(){
        foreach (var interactor in _interactorsMap.Values)
            interactor.OnCombatEnded();
        OnCombatEndedEvent?.Invoke();
        RemoveCombatRelatedEntitiesAndInteractors();
    }


    private void SetupOnGameStartEntitesAndInteractors(){
        var confInteractor = GetInteractor<SessionConfInteractor>();
        MapInteractor mapInteractor;
        AIInteractor aiInteractor;
        StateTreeInteractor treeInteractor;

        {    
            var mutableMap = MapFactory.Create(confInteractor.Entity.Dimensions, confInteractor.Entity.FoodCount);
            _entitiesMap.Add(Entities.Map, mutableMap);
            mapInteractor = new MapInteractor(mutableMap);
            _interactorsMap.Add(typeof(MapInteractor), mapInteractor);
        }

        {
            var mutableTree = StateTreeFactory.Create(mapInteractor.Entity, confInteractor.Entity, 
                exposingMaxDepth: confInteractor.Entity.MaxTurnCount);
            treeInteractor = new StateTreeInteractor(mutableTree);
            _interactorsMap.Add(typeof(StateTreeInteractor), treeInteractor);

            var aiEntity = new AIEntityMutable(treeInteractor.Entity);
            _entitiesMap.Add(Entities.AI, aiEntity);

            aiInteractor = new AIInteractor(aiEntity);
            _interactorsMap.Add(typeof(AIInteractor), aiInteractor);
        }

        _interactorsMap.Add(typeof(CombatInteractor), new CombatInteractor(
            mapInteractor, confInteractor, treeInteractor, aiInteractor
        ));
    }

    private void RemoveCombatRelatedEntitiesAndInteractors(){
        _interactorsMap.Remove(typeof(MapInteractor));
        _interactorsMap.Remove(typeof(CombatInteractor));
        _interactorsMap.Remove(typeof(AIInteractor));
        _interactorsMap.Remove(typeof(StateTreeInteractor));
        _entitiesMap.Remove(Entities.Map);
        _entitiesMap.Remove(Entities.AI);

    }


    public void TryStartCombat(){
        if (CombatInProgress)
            return;
        OnCombatStarted();
        CombatInProgress = true;
    } 
    public void TryEndCombat(){
        if (!CombatInProgress)
            return;
        OnCombatEnded();
        CombatInProgress = false;
    }
}