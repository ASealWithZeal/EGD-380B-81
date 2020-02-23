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
        yield return new WaitForSeconds(0.75f);

        while (theGroup.alpha < 1.0f)
        {
            theGroup.alpha += 0.1f;
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        for (int i = 0; i < charScripts.Count; ++i)
        {
            if (charScripts[i].charStats != null)
                charScripts[i].UpdateUI(earnedEXP);
            else
                charScripts[i].done = true;
        }

        for (int i = 0; i < charScripts.Count; ++i)
            while (!charScripts[i].done)
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

        // TEMP
        Managers.SceneChangeManager.Instance.EndCombat();

        yield return null;
    }
}
