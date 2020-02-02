using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallWinCanvasScript : MonoBehaviour
{
    public List<WinCanvasScript> charScripts;
    private CanvasGroup theGroup = null;

    // Start is called before the first frame update
    void Start()
    {
        theGroup = GetComponent<CanvasGroup>();
        theGroup.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowWinCanvas()
    {
        StartCoroutine(ShowCanvas());
    }

    IEnumerator ShowCanvas()
    {
        while (theGroup.alpha < 1.0f)
        {
            theGroup.alpha += 0.1f;
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        for (int i = 0; i < charScripts.Count; ++i)
            charScripts[i].UpdateUI();

        for (int i = 0; i < charScripts.Count; ++i)
            while (!charScripts[i].done)
            {
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

        // TEMP
        yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements * 100);

        Managers.SceneChangeManager.Instance.ChangeScene("WinScene");

        yield return null;
    }
}
