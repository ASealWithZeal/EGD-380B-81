using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharData : MonoBehaviour
{
    public bool targeting;
    [HideInInspector] public bool isInCombat = true;
    public int t1Pos = 0;
    public int t2Pos = 0;

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
    public Stats charStats;
    public bool hasActed = false;

    private void Start()
    {
        if (gameObject.tag == "Player")
            charUI.GetComponent<PlayerStatusUI>().SetNewHP(charStats.currentHP, charStats.maxHP, charStats.currentTP, charStats.maxTP);
        else
            charUI.GetComponent<EnemyUI>().CreateUI(transform);
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
}
