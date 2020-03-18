using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallWinCanvasScript : MonoBehaviour
{
    public List<WinCanvasScript> charScripts;
    public List<AbilityLearnScreen> charLearns = null;
    public List<CanvasGroup> charGroups = null;
    private CanvasGroup theGroup = null;
    public CanvasGroup abilityGroup = null;

    // Start is called before the first frame update
    void Start()
    {
        theGroup = GetComponent<CanvasGroup>();

        theGroup.alpha = 0.0f;
        abilityGroup.alpha = 0.0f;
    }

    public void ShowWinCanvas(int earnedEXP, int inst)
    {
        Managers.TurnManager.Instance.tracker.DestroyEmptyTrackers(false);
        StartCoroutine(ShowCanvas(earnedEXP, inst));
    }

    IEnumerator ShowCanvas(int earnedEXP, int inst)
    {
        yield return new WaitForSeconds(0.75f);

        while (theGroup.alpha < 1.0f)
        {
            theGroup.alpha += 0.1f;
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }
        
        List<WinCanvasScript> tempScript = new List<WinCanvasScript>();
        for (int i = 0; i < charScripts.Count; ++i)
            if (charScripts[i].charStats.gameObject.GetComponent<CharData>().combatInst == inst)
                tempScript.Add(charScripts[i]);

        for (int i = 0; i < tempScript.Count; ++i)
            tempScript[i].UpdateUI(earnedEXP);

        for (int i = 0; i < tempScript.Count; ++i)
            while (!tempScript[i].done)
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

        yield return new WaitForSeconds(2.0f);

        while (charGroups[0].alpha > 0.0f)
        {
            for (int i = 0; i < charGroups.Count; ++i)
                charGroups[i].alpha -= 0.1f;
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        yield return new WaitForSeconds(0.5f);

        // Ability Display
        for (int i = 0; i < charScripts.Count; ++i)
        {
            if (charScripts[i].charStats.startingLevel < charScripts[i].charStats.level)
            {
                charLearns[i].gameObject.SetActive(true);
                charLearns[i].SetNameText();
                charLearns[i].Init();

                while (abilityGroup.alpha < 1.0f)
                {
                    abilityGroup.alpha += 0.1f;
                    yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
                }

                while (!charLearns[i].done)
                    yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
                yield return new WaitForSeconds(0.5f);

                while (abilityGroup.alpha > 0.0f)
                {
                    abilityGroup.alpha -= 0.1f;
                    yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
                }
                charLearns[i].gameObject.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
        }

        while (theGroup.alpha > 0.0f)
        {
            theGroup.alpha -= 0.1f;
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        List<GameObject> l = new List<GameObject>();
        for (int i = 0; i < charScripts.Count; ++i)
            if (charScripts[i].charStats.gameObject.GetComponent<CharData>().isInCombat)
                l.Add(charScripts[i].charStats.gameObject);

        // TEMP
        Managers.SceneChangeManager.Instance.WinCombat(l);

        yield return null;
    }
}
