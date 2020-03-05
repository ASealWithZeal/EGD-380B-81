using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventTextBox : MonoBehaviour
{
    public TextMeshProUGUI eventText = null;
    public CanvasGroup group = null;
    public GameObject passedEvent = null;
    public List<Button> buttons = null;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttons.Count; ++i)
            buttons[i].interactable = false;
    }

    public void PassEventIn(string s, GameObject theEvent)
    {
        Managers.MovementManager.Instance.canMoveChars = false;
        eventText.SetText(s);
        passedEvent = theEvent;
        StartCoroutine(ShowBox());
    }

    public void PassEventOut(bool @bool)
    {
        StartCoroutine(HideBox(@bool));
    }

    IEnumerator ShowBox()
    {
        while (group.alpha < 1)
        {
            group.alpha += 0.1f;
            yield return new WaitForSeconds(0.0125f);
        }

        for (int i = 0; i < buttons.Count; ++i)
            buttons[i].interactable = true;
        yield return new WaitForSeconds(0.0125f);
        buttons[0].Select();
    }

    IEnumerator HideBox(bool @bool)
    {
        for (int i = 0; i < buttons.Count; ++i)
            buttons[i].interactable = false;

        while (group.alpha > 0)
        {
            group.alpha -= 0.1f;
            yield return new WaitForSeconds(0.0125f);
        }

        passedEvent.GetComponent<EventObj>().PassResponse(@bool);
    }
}
