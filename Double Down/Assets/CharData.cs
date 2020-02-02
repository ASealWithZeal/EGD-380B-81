using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharData : MonoBehaviour
{
    public bool targeting;
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

    public void TargetThis()
    {
        targeting = true;
    }

    public void StopTargetingThis()
    {
        targeting = false;
    }
}
