using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StateNodeExample : MonoBehaviour
{
    [SerializeField, InspectorName("Score Label")] private Text _scoreLabel; 
    public StateTreeNode AttachedStateNode { get; private set; }

    private void OnEnable(){
        GetComponent<Button>().onClick.AddListener(OnStateOnClicked);
    }
    private void OnDisable(){
        GetComponent<Button>().onClick.RemoveListener(OnStateOnClicked);
    }

    private void OnStateOnClicked(){
        UiEventHandler.Instance.ShowStateNodeExampleOnMap(AttachedStateNode);
    }

    public void DisplayStateNode(StateTreeNode newStateNode){
        AttachedStateNode = newStateNode;
        _scoreLabel.text = $"{newStateNode.PlayerScore}/{newStateNode.AIScore}";
    }
}
