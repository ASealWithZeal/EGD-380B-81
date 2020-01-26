using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatMenuManager : MonoBehaviour
{
    public List<GameObject> playerMenu;

    public void MakeButtonVisible(bool inter)
    {
        StartCoroutine(MakeMenuVisible(inter));
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
            }
        }

        while ((inter && playerMenu[0].GetComponent<Image>().color.a < 1)
            || (!inter && playerMenu[0].GetComponent<Image>().color.a > 0))
        {
            for (int i = 0; i < playerMenu.Count; ++i)
            {
                playerMenu[i].GetComponent<Image>().color += new Color(0, 0, 0, incs);
                playerMenu[i].transform.GetChild(0).GetComponent<Text>().color += new Color(0, 0, 0, incs);
            }

            yield return new WaitForSeconds(0.005f);
        }

        if (inter)
        {
            for (int i = 0; i < playerMenu.Count; ++i)
            {
                playerMenu[i].GetComponent<Button>().interactable = inter;
            }
        }

        yield return null;
    }
}
