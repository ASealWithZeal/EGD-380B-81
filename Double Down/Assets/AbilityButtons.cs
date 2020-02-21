using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AbilityButtons : MonoBehaviour, ISelectHandler
{
    public string buttonText = null;
    public int ability;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.parent.GetComponent<AbilityLearnScreen>().SetAbilityText(buttonText);
    }

    public void SelectAbilityButton()
    {
        transform.parent.GetComponent<AbilityLearnScreen>().LearnAbility(ability);
    }

    public void SelectPassiveButton(int passive)
    {
        transform.parent.GetComponent<AbilityLearnScreen>().LearnPassiveAbility(ability, passive);
    }
}
