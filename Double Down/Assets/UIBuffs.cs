using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BuffType
{
    ATK = 0,
    DEF,
    SPD,
    Aggro
}

public class UIBuffs : MonoBehaviour
{
    private GameObject parent;
    public TextMeshProUGUI countdownText;
    public Image buffImage;
    public Sprite[] buffSprites;
    public BuffType buffType;
    public bool destroyed = false;

    // Update is called once per frame
    public void Init(GameObject parent, int countdown, float value, BuffType type)
    {
        if (countdown < 100)
            countdownText.SetText(countdown.ToString());
        buffType = type;
        this.parent = parent;

        switch (type)
        {
            case BuffType.ATK:
                buffImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
                buffImage.rectTransform.sizeDelta = new Vector2(30, 30);
                buffImage.sprite = buffSprites[0];
                break;
            case BuffType.DEF:
                buffImage.rectTransform.localEulerAngles = new Vector3(0, 0, 45);
                buffImage.rectTransform.sizeDelta = new Vector2(21, 21);
                buffImage.sprite = buffSprites[1];
                break;
            case BuffType.SPD:
                buffImage.rectTransform.localEulerAngles = new Vector3(0, 0, 45);
                buffImage.rectTransform.sizeDelta = new Vector2(21, 21);
                buffImage.sprite = buffSprites[2];
                break;
            case BuffType.Aggro:
                buffImage.rectTransform.localEulerAngles = new Vector3(0, 0, 45);
                buffImage.rectTransform.sizeDelta = new Vector2(21, 21);
                buffImage.sprite = buffSprites[3];
                break;
        }

        CheckArrows(value);
        StartCoroutine(CreateBuff());
    }

    // Updates all buff images at the start of each turn
    // OR when the user receives a new buff
    public void UpdateImage(int countdown, float value)
    {
        if (countdown < 100)
            countdownText.SetText(countdown.ToString());
        if (countdown == 0)
            StartCoroutine(DestroyBuff());
        else
            CheckArrows(value);
    }

    // Checks which arrows appear based on the stat's new value
    private void CheckArrows(float value)
    {
        for (int i = 0; i < 4; ++i)
            transform.GetChild(2 + i).gameObject.SetActive(false);

        if (value > 1.0f && value < 1.5f)
            transform.GetChild(2).gameObject.SetActive(true);
        else if (value >= 1.5f)
            transform.GetChild(3).gameObject.SetActive(true);
        else if (value < 1.0f && value > 0.5f)
            transform.GetChild(4).gameObject.SetActive(true);
        else if (value <= 0.5f)
            transform.GetChild(5).gameObject.SetActive(true);
    }

    IEnumerator CreateBuff()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        RectTransform rt = GetComponent<RectTransform>();

        rt.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        while (group.alpha < 1)
        {
            group.alpha += 0.25f;
            rt.localScale -= new Vector3(0.125f, 0.125f, 0.125f);
            yield return new WaitForSeconds(0.02f);
        }
        group.alpha = 1;
        rt.localScale = new Vector3(1, 1, 1);

        yield return null;
    }

    public void DestroyImmediately()
    {
        StartCoroutine(DestroyBuff());
    }

    IEnumerator DestroyBuff()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        RectTransform rt = GetComponent<RectTransform>();

        rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        while (group.alpha > 0)
        {
            group.alpha -= 0.25f;
            rt.localScale += new Vector3(0.125f, 0.125f, 0.125f);
            yield return new WaitForSeconds(0.02f);
        }
        group.alpha = 0;
        rt.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        yield return new WaitForSeconds(0.2f);
        destroyed = true;

        if (parent.GetComponent<PlayerStatusUI>() != null)
            parent.GetComponent<PlayerStatusUI>().MoveBuffs();
        else if (parent.GetComponent<EnemyUI>() != null)
            parent.GetComponent<EnemyUI>().MoveBuffs();

        yield return null;
    }
}
