using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HubMenuDisplay : MonoBehaviour
{
    public GameObject menu;
    public CanvasGroup theGroup;
    private bool open = false;

    // Stats information
    public List<Button> statButtons = null;
    public List<string> statTextString = null;
    public TextMeshProUGUI[] statsString = null;
    public TextMeshProUGUI[] statsAddString = null;

    // General information
    public TextMeshProUGUI[] charNameData = null;
    public Image charPortrait;

    // Level-up information
    public TextMeshProUGUI[] expString = null;
    public Image expBar = null;

    // Bottom Text
    public TextMeshProUGUI bottomText = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !open)
            OpenMenu();
        else if (Input.GetKeyDown(KeyCode.Escape) && open)
            StartCoroutine(ClosingMenu());
    }

    public void ChangeText(int i)
    {
        bottomText.SetText(statTextString[i]);
    }

    private void OpenMenu()
    {
        Managers.MovementManager.Instance.canMoveChars = false;
        //SetTextPerCharacter(Managers.TurnManager.Instance.playerCharsList[0].GetComponent<Stats>());
        SetTextPerCharacter(GameObject.Find("Char1").GetComponent<Stats>());
        StartCoroutine(OpeningMenu());
    }

    private void SetTextPerCharacter(Stats charStats)
    {
        // General Information
        charNameData[0].SetText(charStats.gameObject.GetComponent<CharData>().name);
        charNameData[1].SetText("Lv " + charStats.level.ToString());
        // portrait


        // Level Information
        int num = 0;
        for (int i = 0; i < charStats.level - 1; ++i)
            num += charStats.nextLevelExp[i];
        expString[0].SetText("EXP: "+ (num + charStats.exp).ToString());
        if (charStats.level < charStats.nextLevelExp.Count)
            expString[1].SetText(charStats.nextLevelExp[charStats.level - 1].ToString());
        else
            expString[1].SetText("MAX");
        expBar.fillAmount = (float)charStats.exp / charStats.nextLevelExp[charStats.level - 1];


        // Stats Information
        statsString[0].SetText(charStats.MaxHP().ToString());
        statsString[1].SetText(charStats.MaxTP().ToString());
        statsString[2].SetText(charStats.Attack().ToString());
        statsString[3].SetText(charStats.Defense().ToString());
        statsString[4].SetText(charStats.Speed().ToString());

        // Added Stats Information
        SetButtonData(0, charStats.maxHP, charStats.HPPassives);
        SetButtonData(2, charStats.maxTP, charStats.TPPassives);
        SetButtonData(4, charStats.atk, charStats.atkPassives);
        SetButtonData(6, charStats.def, charStats.defPassives);
        SetButtonData(8, charStats.spd, charStats.spdPassives);
    }

    private void SetButtonData(int startButton, int stat, float mult)
    {
        string s = null;

        // Original Stat
        statsAddString[startButton].SetText(stat.ToString());

        // Enhanced Stat
        if (stat * mult < 10)
            s = " " + ((int)(stat * mult)).ToString() + " )";
        else if (stat * mult < 100)
            s = ((int)(stat * mult)).ToString() + " )";
        else
            s = ((int)(stat * mult)).ToString() + ")";
        statsAddString[startButton + 1].SetText(s);
    }

    IEnumerator OpeningMenu()
    {
        while (theGroup.alpha < 1)
        {
            theGroup.alpha += 0.1f;
            yield return new WaitForSeconds(0.0125f);
        }

        for (int i = 0; i < statButtons.Count; ++i)
            statButtons[i].interactable = true;
        yield return new WaitForSeconds(0.0125f);
        Debug.Log(statButtons[0].interactable);
        statButtons[0].Select();

        open = true;
        yield return null;
    }

    IEnumerator ClosingMenu()
    {
        for (int i = 0; i < statButtons.Count; ++i)
            statButtons[i].interactable = false;

        while (theGroup.alpha > 0)
        {
            theGroup.alpha -= 0.1f;
            yield return new WaitForSeconds(0.0125f);
        }

        Managers.MovementManager.Instance.canMoveChars = true;

        open = false;
        yield return null;
    }
}
