using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SartingSideLabelExample : MonoBehaviour
{
    void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void OnEnable(){
        Game.Instance.GetInteractor<SessionConfInteractor>().OnStartingSideChangedEvent += UpdateLabelText;
    }
    public void OnDisable(){
        Game.Instance.GetInteractor<SessionConfInteractor>().OnStartingSideChangedEvent -= UpdateLabelText;
    }

    public void Start(){
        UpdateLabelText(Game.Instance.GetInteractor<SessionConfInteractor>().Entity.IsPlayerStarting);
    }

    private Text _text;

    private void UpdateLabelText(bool IsPlayerStarting){
        if (IsPlayerStarting)
            _text.text = "Starting side: You";
        else
            _text.text = "Starting side: Enemy";
    }
}
