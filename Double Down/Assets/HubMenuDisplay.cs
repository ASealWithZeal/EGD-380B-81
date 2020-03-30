using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MenuPage
{
    Stats = 0,
    Abilities = 1
}

public class HubMenuDisplay : MonoBehaviour
{
    public GameObject menu;
    public GameObject[] pageArray;
    public CanvasGroup theGroup;
    private MenuPage menuPage = 0;
    private bool open = false;

    // Stats information
    public List<Button> statButtons = null;
    public List<string> statTextString = null;
    public TextMeshProUGUI[] statsString = null;
    public TextMeshProUGUI[] statsAddString = null;

    // Abilities Information
    public List<Button> abilitiesButtons = null;
    public List<string> abilitiesTextString = null;
    public List<TextMeshProUGUI> abilitiesNameString = null;
    public List<TextMeshProUGUI> abilitiesCostString = null;
    public TextMeshProUGUI abilitiesBottomText = null;

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
        if (Managers.MovementManager.Instance.canMoveChars && Input.GetKeyDown(KeyCode.Escape) && !open)
            OpenMenu();
        else if (Input.GetKeyDown(KeyCode.Escape) && open)
            StartCoroutine(ClosingMenu());
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && open)
            SwapPages(menuPage - 1);
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && open)
            SwapPages(menuPage + 1);
    }

    public void ChangeText(int i)
    {
        switch (menuPage)
        {
            case MenuPage.Stats:
                bottomText.SetText(statTextString[i]);
                break;
            case MenuPage.Abilities:
                abilitiesBottomText.SetText(abilitiesTextString[i]);
                break;
        }
    }

    private void SwapPages(MenuPage newPage)
    {
        // Normalizes the pages and sets the new page number
        if (newPage < 0)
            newPage = (MenuPage)1;
        else if (newPage > (MenuPage)1)
            newPage = 0;
        menuPage = newPage;

        // Swaps out the page accordingly
        for (int i = 0; i < 2; ++i)
            pageArray[i].SetActive(false);

        switch (newPage)
        {
            case MenuPage.Stats:
                pageArray[0].SetActive(true);
                for (int i = 0; i < statButtons.Count; ++i)
                    statButtons[i].interactable = true;
                statButtons[0].Select();
                break;
            case MenuPage.Abilities:
                pageArray[1].SetActive(true);
                for (int i = 0; i < abilitiesButtons.Count; ++i)
                    abilitiesButtons[i].interactable = true;
                abilitiesButtons[0].Select();
                break;
        }
    }

    private void OpenMenu()
    {
        Managers.MovementManager.Instance.canMoveChars = false;
        //SetTextPerCharacter(Managers.TurnManager.Instance.playerCharsList[0].GetComponent<Stats>());
        SetTextPerCharacter(Managers.TurnManager.Instance.t1[0].GetComponent<Stats>(), Managers.TurnManager.Instance.t1[0].GetComponent<CharData>());
        StartCoroutine(OpeningMenu());
    }

    private void SetTextPerCharacter(Stats charStats, CharData charData)
    {
        // General Information
        charNameData[0].SetText(charData.name);
        charNameData[1].SetText("Lv " + charStats.level.ToString());
        charPortrait.color = charData.colors[0];


        // Level Information
        int num = 0;
        for (int i = 0; i < charStats.level - 1; ++i)
            num += charStats.nextLevelExp[i];
        expString[0].SetText("EXP: "+ (num + charStats.exp).ToString());
        if (charStats.level < charStats.nextLevelExp.Count + 1)
        {
            expBar.fillAmount = (float)charStats.exp / charStats.nextLevelExp[charStats.level - 1];
            expString[1].SetText((charStats.nextLevelExp[charStats.level - 1] - charStats.exp).ToString());
        }
        else
        {
            expBar.fillAmount = 1;
            expString[1].SetText("--");
        }


        // Stats Information
        statsString[0].SetText(charStats.MaxHP().ToString());
        statsString[1].SetText(charStats.MaxTP().ToString());
        statsString[2].SetText(charStats.Attack().ToString());
        statsString[3].SetText(charStats.Defense().ToString());
        statsString[4].SetText(charStats.Speed().ToString());

        // Added Stats Information
        SetButtonData(0, charStats.maxHP, 0, charStats.HPPassives);
        SetButtonData(2, charStats.maxTP, 0, charStats.TPPassives);
        SetButtonData(4, charStats.atk, charStats.atkBonus, charStats.atkPassives);
        SetButtonData(6, charStats.def, charStats.defBonus, charStats.defPassives);
        SetButtonData(8, charStats.spd, 0, charStats.spdPassives);

        // Abilities Information
        int a = 0;
        abilitiesButtons[a].gameObject.SetActive(true);
        SetAbilityButtonData(a, 0, charData);
        a++;
        for (int i = 0; i < charData.learnedAbilities.Count; ++i)
            if (charData.learnedAbilities[i])
            {
                if (i == 0)
                    a = 0;
                abilitiesButtons[a].gameObject.SetActive(true);
                SetAbilityButtonData(a, i, charData);
                a++;
            }
    }

    private void SetButtonData(int startButton, int stat, int statBonus, float mult)
    {
        string s = null;

        // Original Stat
        statsAddString[startButton].SetText(stat.ToString());

        // Enhanced Stat
        if ((stat * mult) + statBonus < 10)
            s = " " + ((int)(stat * mult) + statBonus).ToString() + " )";
        else if ((stat * mult) + statBonus < 100)
            s = ((int)(stat * mult) + statBonus).ToString() + " )";
        else
            s = ((int)(stat * mult) + statBonus).ToString() + ")";
        statsAddString[startButton + 1].SetText(s);
    }

    private void SetAbilityButtonData(int abilityButton, int abilityNum, CharData charData)
    {
        abilitiesNameString[abilityButton].SetText(charData.gameObject.GetComponent<PlayerActions>().GetAbilityName(abilityNum));

        if (charData.gameObject.GetComponent<PlayerActions>().GetAbilityActive(abilityNum))
        {
            abilitiesCostString[abilityButton].SetText(charData.gameObject.GetComponent<PlayerActions>().GetAbilityCost(abilityNum) + " TP");
            abilitiesButtons[abilityButton].GetComponent<StatButtons>().ChangeSelectedColor(0);
        }
        else
        {
            abilitiesCostString[abilityButton].SetText("");
            abilitiesButtons[abilityButton].GetComponent<StatButtons>().ChangeSelectedColor(1);
        }
        abilitiesTextString[abilityButton] = charData.gameObject.GetComponent<PlayerActions>().GetAbilityDescription(abilityNum);
    }

    IEnumerator OpeningMenu()
    {
        menuPage = 0;
        pageArray[0].SetActive(true);
        while (theGroup.alpha < 1)
        {
            theGroup.alpha += 0.1f;
            yield return new WaitForSeconds(0.0125f);
        }

        for (int i = 0; i < statButtons.Count; ++i)
            statButtons[i].interactable = true;
        yield return new WaitForSeconds(0.0125f);
        statButtons[0].Select();

        open = true;
        yield return null;
    }

    IEnumerator ClosingMenu()
    {
        for (int i = 0; i < statButtons.Count; ++i)
            statButtons[i].interactable = false;
        for (int i = 0; i < abilitiesButtons.Count; ++i)
        {
            abilitiesButtons[i].interactable = false;
            abilitiesButtons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < pageArray.Length; ++i)
            pageArray[i].SetActive(false);

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
