using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
    public enum Entities{
        Player,
        Enemy,
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
        var conf = confInteractor.Entity;
        MapInteractor mapInteractor;
        PlayerInteractor playerInteractor;
        EnemyInteractor enemyInteractor;
        AIInteractor aiInteractor;

        {    
            var mutableMap = MapFactory.Create(conf.Dimensions, conf.FoodCount);
            _entitiesMap.Add(Entities.Map, mutableMap);
            mapInteractor = new MapInteractor(mutableMap);
            _interactorsMap.Add(typeof(MapInteractor), mapInteractor);
        }

        {
            var mapEntity = mapInteractor.Entity;
            var mutableSnake =  new SnakeEntityMutable(conf.IsPlayerStarting ? mapEntity.SpawnPositions.first : mapEntity.SpawnPositions.second);
            _entitiesMap.Add(Entities.Player, mutableSnake);
            playerInteractor = new PlayerInteractor(mutableSnake, mapInteractor);
            _interactorsMap.Add(typeof(PlayerInteractor), playerInteractor);

            mutableSnake =  new SnakeEntityMutable(conf.IsPlayerStarting ? mapEntity.SpawnPositions.second : mapEntity.SpawnPositions.first);
            _entitiesMap.Add(Entities.Enemy, mutableSnake);
            enemyInteractor = new EnemyInteractor(mutableSnake, mapInteractor);
            _interactorsMap.Add(typeof(EnemyInteractor), enemyInteractor);
        }

        {
            var aiEntity = new AIEntityMutable(null);
            _entitiesMap.Add(Entities.AI, aiEntity);
            aiInteractor = new AIInteractor(aiEntity, confInteractor, enemyInteractor);
            _interactorsMap.Add(typeof(AIInteractor), aiInteractor);
        }

        _interactorsMap.Add(typeof(CombatInteractor), new CombatInteractor(
            mapInteractor, confInteractor, playerInteractor, enemyInteractor, aiInteractor
        ));

    }

    private void RemoveCombatRelatedEntitiesAndInteractors(){
        _interactorsMap.Remove(typeof(PlayerInteractor));
        _interactorsMap.Remove(typeof(EnemyInteractor));
        _interactorsMap.Remove(typeof(MapInteractor));
        _interactorsMap.Remove(typeof(CombatInteractor));
        _interactorsMap.Remove(typeof(AIInteractor));
        _entitiesMap.Remove(Entities.Player);
        _entitiesMap.Remove(Entities.Enemy);
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