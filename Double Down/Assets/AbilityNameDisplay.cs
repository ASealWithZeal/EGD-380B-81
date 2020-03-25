using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityNameDisplay : MonoBehaviour
{
    public CanvasGroup theGroup;
    public RectTransform displayObj;
    public RectTransform textObj;
    public TextMeshProUGUI text;

    public void ChangeNameDisplay(string newName)
    {
        float fontSize = text.fontSize;
        float widener = newName.Length * 2;

        if (newName.Length < 16)
            widener = 32;

        text.SetText(newName);
        displayObj.sizeDelta = new Vector2((newName.Length * (fontSize / 2)) + (widener), displayObj.sizeDelta.y);
        textObj.sizeDelta = new Vector2((newName.Length * (fontSize / 2)) + (widener), displayObj.sizeDelta.y);
    }

    public void ChangeDisplayOpacity(bool i)
    {
        StartCoroutine(ChangeOpacity(i));
    }

    IEnumerator ChangeOpacity(bool i)
    {
        float mult = 1.0f;
        if (!i)
            mult = -1.0f;

        while ((i && theGroup.alpha < 1) || (!i && theGroup.alpha > 0))
        {
            theGroup.alpha += (0.1f * mult);
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        if (!i)
            theGroup.alpha = 0.0f;
        else
            theGroup.alpha = 1.0f;

        yield return null;
    }
}
