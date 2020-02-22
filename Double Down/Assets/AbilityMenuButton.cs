using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AbilityMenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI description = null;
    public GameObject descriptionObject = null;

    public void OnSelect(BaseEventData data)
    {
        descriptionObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData data)
    {
        descriptionObject.SetActive(false);
    }

    public void SetDescriptionText(string s)
    {
        description.SetText(s);
    }
}
