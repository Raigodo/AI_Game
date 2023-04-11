using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TreeViewPanelExample : MonoBehaviour
{
    [SerializeField, InspectorName("State Node Prefab")] private RectTransform _stateNodePrefab;
    [SerializeField, InspectorName("Content Holder")] private RectTransform _contentHolder;
    [SerializeField, InspectorName("SpawnPositions")] private Transform[] _spawnPositions = new Transform[4];


    public void OnEnable(){
        var currentNode = Game.Instance.GetInteractor<StateTreeInteractor>().Entity.CurrentStateNode;
        SpawnAllNodes(currentNode);
    }
    public void OnDisable(){
        DeleteAllNodes();
    }


    private void SpawnAllNodes(StateTreeNode node){
        SpawnStateNode(node, 0);
        for (int i=0; i<node.Children.Count(); i++){
            SpawnStateNode(node.Children.ElementAt(i), i+1);
        }
    }
    private void DeleteAllNodes(){
        for (int i=_contentHolder.childCount-1; i>=0; i--)
            Destroy(_contentHolder.GetChild(i).gameObject);
    }

    private void SpawnStateNode(StateTreeNode node, int spawnPositionIndex){
        RectTransform nodeExample = Instantiate(_stateNodePrefab);
        nodeExample.SetParent(_contentHolder);
        nodeExample.localScale = Vector3.one;
        nodeExample.position = _spawnPositions[spawnPositionIndex].position;
        nodeExample.GetComponent<StateNodeExample>().DisplayStateNode(node);
    }
}