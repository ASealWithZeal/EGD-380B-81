using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StatButtons : MonoBehaviour, ISelectHandler
{
    public int num;
    public HubMenuDisplay menuDisplay = null;

    public Color[] colors;

    public void OnSelect(BaseEventData baseData)
    {
        menuDisplay.ChangeText(num);
    }

    public void ChangeSelectedColor(int i)
    {
        ColorBlock col = gameObject.GetComponent<Button>().colors;
        col.selectedColor = colors[i];
        col.pressedColor = colors[i];
        gameObject.GetComponent<Button>().colors = col;
    }
}
