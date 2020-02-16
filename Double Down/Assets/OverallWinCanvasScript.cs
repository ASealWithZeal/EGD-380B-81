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

    public void ShowWinCanvas(int earnedEXP)
    {
        StartCoroutine(ShowCanvas(earnedEXP));
    }

    IEnumerator ShowCanvas(int earnedEXP)
    {
        while (theGroup.alpha < 1.0f)
        {
            theGroup.alpha += 0.1f;
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        for (int i = 0; i < charScripts.Count; ++i)
            charScripts[i].UpdateUI(earnedEXP);

        for (int i = 0; i < charScripts.Count; ++i)
            while (!charScripts[i].done)
            {
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

        // TEMP
        yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements * 300);
        Managers.SceneChangeManager.Instance.EndCombat();

        yield return null;
    }
}
