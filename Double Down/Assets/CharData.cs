using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharData : MonoBehaviour
{
    public bool targeting;

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

    private void Start()
    {
        if (gameObject.tag == "Player")
            charUI.GetComponent<PlayerStatusUI>().SetNewHP(charStats.currentHP, charStats.maxHP, charStats.currentTP, charStats.maxTP);
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

    public void TargetThis()
    {
        targeting = true;
    }

    public void StopTargetingThis()
    {
        targeting = false;
    }
}
