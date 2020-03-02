using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StatButtons : MonoBehaviour, ISelectHandler
{
    public int num;
    public HubMenuDisplay menuDisplay = null;

    public void OnSelect(BaseEventData baseData)
    {
        menuDisplay.ChangeText(num);
    }
}
