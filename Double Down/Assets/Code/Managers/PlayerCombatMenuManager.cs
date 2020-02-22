using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCombatMenuManager : MonoBehaviour
{
    public List<GameObject> playerMenu;
    public List<GameObject> playerAbilitiesMenu;
    public CanvasGroup menuGroup;
    public CanvasGroup abilitiesGroup;

    public void MakeButtonVisible(bool inter)
    {
        StartCoroutine(MakeMenuVisible(inter));
    }

    public void ShowAbilities(bool inter, GameObject chara)
    {
        StartCoroutine(ShowAbilitiesMenu(inter, chara));
    }

    IEnumerator MakeMenuVisible(bool inter)
    {
        float incs = 0.05f;

        if (!inter)
        {
            incs *= -1;
            for (int i = 0; i < playerMenu.Count; ++i)
            {
                playerMenu[i].GetComponent<Button>().interactable = inter;
                playerAbilitiesMenu[i].GetComponent<Button>().interactable = false;
            }
        }

        playerMenu[0].GetComponent<Button>().Select();
        while ((inter && (menuGroup.alpha < 1)) || ((!inter && menuGroup.alpha > 0)))
        {
            menuGroup.alpha += incs;
            yield return new WaitForSeconds(0.0125f);
        }

        while (abilitiesGroup.alpha > 0)
        {
            abilitiesGroup.alpha -= 0.05f;
            yield return new WaitForSeconds(0.0125f);
        }

        if (inter)
        {
            for (int i = 0; i < playerMenu.Count; ++i)
            {
                playerMenu[i].GetComponent<Button>().interactable = inter;
            }
        }

        menuGroup.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        abilitiesGroup.gameObject.transform.localPosition = new Vector3(125, 0, 0);

        menuGroup.blocksRaycasts = inter;

        yield return null;
    }

    IEnumerator ShowAbilitiesMenu(bool inter, GameObject chara)
    {
        float incs = 0.05f;

        // Sets all buttons to be non-interactable
        if (inter)
        {
            incs *= -1;
            playerAbilitiesMenu[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(Managers.TurnManager.Instance.t1[0].GetComponent<PlayerActions>().GetAbilityName(0));
            playerAbilitiesMenu[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(Managers.TurnManager.Instance.t1[0].GetComponent<PlayerActions>().GetAbilityCost(0) + " TP");
            playerAbilitiesMenu[0].GetComponent<AbilityMenuButton>().SetDescriptionText(Managers.TurnManager.Instance.t1[0].GetComponent<PlayerActions>().GetAbilityDescription(0));
            for (int i = 1; i < playerAbilitiesMenu.Count - 1; ++i)
            {
                if (chara.GetComponent<CharData>().learnedAbilities[i])
                {
                    playerAbilitiesMenu[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(Managers.TurnManager.Instance.t1[0].GetComponent<PlayerActions>().GetAbilityName(i));
                    playerAbilitiesMenu[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(Managers.TurnManager.Instance.t1[0].GetComponent<PlayerActions>().GetAbilityCost(i) + " TP");
                    playerAbilitiesMenu[i].GetComponent<AbilityMenuButton>().SetDescriptionText(Managers.TurnManager.Instance.t1[0].GetComponent<PlayerActions>().GetAbilityDescription(i));
                }
                else
                {
                    playerAbilitiesMenu[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("");
                    playerAbilitiesMenu[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("");
                    playerAbilitiesMenu[i].GetComponent<AbilityMenuButton>().SetDescriptionText("");
                }
            }

            for (int i = 0; i < playerMenu.Count; ++i)
            {
                playerMenu[i].GetComponent<Button>().interactable = !inter;
            }
        }

        else
            for (int i = 0; i < playerAbilitiesMenu.Count; ++i)
            {
                playerAbilitiesMenu[i].GetComponent<Button>().interactable = false;
            }

        //playerMenu[0].GetComponent<Button>().Select();
        while ((!inter && menuGroup.alpha < 1) || (inter && menuGroup.alpha > 0))
        {
            menuGroup.alpha += incs;
            menuGroup.gameObject.transform.position += new Vector3(incs * 300, 0, 0);

            abilitiesGroup.alpha -= incs;
            abilitiesGroup.gameObject.transform.position += new Vector3(incs * 300, 0, 0);

            yield return new WaitForSeconds(0.0125f);
        }

        if (!inter)
        {
            for (int i = 0; i < playerAbilitiesMenu.Count; ++i)
            {
                playerMenu[i].GetComponent<Button>().interactable = !inter;
            }
            playerMenu[0].GetComponent<Button>().Select();
        }

        else
        {
            for (int i = 0; i < playerAbilitiesMenu.Count; ++i)
            {
                if (i == 0 && Managers.TurnManager.Instance.t1[0].GetComponent<Stats>().currentTP >= Managers.TurnManager.Instance.t1[0].GetComponent<PlayerActions>().GetAbilityCost(0))
                    playerAbilitiesMenu[0].GetComponent<Button>().interactable = inter;
                else if (i > 0 && chara.GetComponent<CharData>().learnedAbilities[i] && Managers.TurnManager.Instance.t1[0].GetComponent<Stats>().currentTP >= Managers.TurnManager.Instance.t1[0].GetComponent<PlayerActions>().GetAbilityCost(i))
                    playerAbilitiesMenu[i].GetComponent<Button>().interactable = inter;
            }
            playerAbilitiesMenu[playerAbilitiesMenu.Count - 1].GetComponent<Button>().interactable = inter;
            playerAbilitiesMenu[0].GetComponent<Button>().Select();
        }

        menuGroup.blocksRaycasts = !inter;
        abilitiesGroup.blocksRaycasts = inter;

        yield return null;
    }
}
