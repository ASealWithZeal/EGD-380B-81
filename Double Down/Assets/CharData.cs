using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharData : MonoBehaviour
{
    public string name;

    public bool targeting;
    [HideInInspector] public bool isInCombat = true;
    public int t1Pos = 0;
    public int t2Pos = 0;
    public int charNum = -1;

    [Header("Abilities")]
    public List<bool> learnedAbilities = new List<bool>();

    [Header("Delayed Attack Info")]
    public string delayedAbilityName;
    public bool delayedAttack;
    public int delayTimer;
    public float storedModifier;
    public List<GameObject> target;

    [Header("Sprites")]
    public Sprite[] nonTargetSprite;
    public Sprite[] targetSprite;

    public GameObject charUI;
    public WinCanvasScript charWinUI;
    public Stats charStats;
    public bool hasActed = false;

    private void Start()
    {
        //if (gameObject.tag == "Enemy")
        //    charUI.GetComponent<EnemyUI>().CreateUI(name, transform);
    }

    public void SetCharUI()
    {
        if (gameObject.tag == "Player")
        {
            charUI.GetComponent<PlayerStatusUI>().SetNewHP(name, charStats.level, charStats.currentHP, charStats.MaxHP(), charStats.currentTP, charStats.MaxTP());
            charWinUI?.Init();
        }
        else if (gameObject.tag == "Enemy")
        {
            charUI.GetComponent<EnemyUI>().CreateUI(name, transform);
        }
    }

    public void ChangeHP()
    {
        if (gameObject.tag == "Player")
            charUI.GetComponent<PlayerStatusUI>().ChangeHealth(charStats.HPPercent(), charStats.currentHP);
        else
            charUI.GetComponent<EnemyUI>().ChangeHealth(charStats.HPPercent());
    }

    public void ChangeTP()
    {
        charUI.GetComponent<PlayerStatusUI>().ChangeTP(charStats.TPPercent(), charStats.currentTP);
    }

    // Sets up the user so they can pull off a delayed attack when relevant
    public void SetDelayedAttack(string name, int timer, float mod, List<GameObject> targ)
    {
        for (int i = 0; i < target.Count; ++i)
            target.Remove(target[0]);

        delayedAbilityName = name;
        delayedAttack = true;
        delayTimer = timer;
        storedModifier = mod;

        for (int i = 0; i < targ.Count; ++i)
            target.Add(targ[i]);
    }

    public void Targeted()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
    }

    public void KillChar()
    {
        StartCoroutine(DeathAnim());
    }

    IEnumerator DeathAnim()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        isInCombat = false;

        while (sr.color.a > 0)
        {
            sr.color -= new Color(0.07f, 0.07f, 0.07f, 0.1f);
            yield return new WaitForSeconds(0.025f);
        }

        if (gameObject.tag != "Player")
            charUI.SetActive(false);
        Destroy(gameObject);
        yield return null;
    }

    public void CopyData(CharData c)
    {
        int temp = Mathf.Abs(learnedAbilities.Count - c.learnedAbilities.Count);
        if (temp > 0)
            for (int i = 0; i < temp; ++i)
                learnedAbilities.Add(false);

        for (int i = 0; i < c.learnedAbilities.Count; ++i)
            learnedAbilities[i] = c.learnedAbilities[i];

        isInCombat = c.isInCombat;
        t1Pos = c.t1Pos;
        t2Pos = c.t2Pos;
        charNum = c.charNum;
    }
}
