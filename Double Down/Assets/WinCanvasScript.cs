using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WinCanvasScript : MonoBehaviour
{
    public Stats charStats;
    public bool done = false;

    [Header("EXP Meter")]
    public TextMeshProUGUI expText;
    public TextMeshProUGUI neededEXPText;
    public Image expMeter;
    public CanvasGroup levelUpGroup;

    [Header("Stats and Gains")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelGainText;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI hpGainText;

    public TextMeshProUGUI tpText;
    public TextMeshProUGUI tpGainText;

    public TextMeshProUGUI atkText;
    public TextMeshProUGUI atkGainText;

    public TextMeshProUGUI defText;
    public TextMeshProUGUI defGainText;

    public TextMeshProUGUI spdText;
    public TextMeshProUGUI spdGainText;

    // Start is called before the first frame update
    void Start()
    {
        levelText.SetText(charStats.level.ToString() + " (");
        hpText.SetText(charStats.maxHP.ToString() + " (");
        tpText.SetText(charStats.maxTP.ToString() + " (");
        atkText.SetText(charStats.atk.ToString() + " (");
        defText.SetText(charStats.def.ToString() + " (");
        spdText.SetText(charStats.spd.ToString() + " (");
    }

    // Update is called once per frame
    public void UpdateUI()
    {
        StartCoroutine(UpdatePlayerUI());
    }

    IEnumerator UpdatePlayerUI()
    {
        expText.SetText("+" + charStats.exp.ToString());
        neededEXPText.SetText("Next: " + charStats.nextLevelExp.ToString());

        bool levelUp = false;
        float fill = charStats.exp / charStats.nextLevelExp;
        if (fill >= 1.0f)
        {
            levelUp = true;
            fill = 1.0f;
        }
        
        while (expMeter.fillAmount < fill)
        {
            expMeter.fillAmount += 0.01f;
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }
        expMeter.fillAmount = fill;

        if (levelUp)
        {
            while (levelUpGroup.alpha < 1.0f)
            {
                levelUpGroup.alpha += 0.1f;
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            levelGainText.SetText("+1 )");
            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            int t = charStats.level + 1;
            int d = charStats.level;
            while (d < t)
            {
                d++;
                levelText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // HP GAIN
            t = charStats.HPGain;
            d = 0;
            while (d < t)
            {
                d++;
                hpGainText.SetText("+" + d.ToString() + " )");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.maxHP + charStats.HPGain;
            d = charStats.maxHP;
            while (d < t)
            {
                d++;
                hpText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // TP GAIN
            t = charStats.TPGain;
            d = 0;
            while (d < t)
            {
                d++;
                tpGainText.SetText("+" + d.ToString() + " )");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.maxTP + charStats.TPGain;
            d = charStats.maxTP;
            while (d < t)
            {
                d++;
                tpText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // ATK GAIN
            t = charStats.atkGain;
            d = 0;
            while (d < t)
            {
                d++;
                atkGainText.SetText("+" + d.ToString() + " )");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.atk + charStats.atkGain;
            d = charStats.atk;
            while (d < t)
            {
                d++;
                atkText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // DEF GAIN
            t = charStats.defGain;
            d = 0;
            while (d < t)
            {
                d++;
                defGainText.SetText("+" + d.ToString() + " )");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.def + charStats.defGain;
            d = charStats.def;
            while (d < t)
            {
                d++;
                defText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // SPD GAIN
            t = charStats.spdGain;
            d = 0;
            while (d < t)
            {
                d++;
                spdGainText.SetText("+" + d.ToString() + " )");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.spd + charStats.spdGain;
            d = charStats.spd;
            while (d < t)
            {
                d++;
                spdText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        done = true;
        yield return null;

    }
}
