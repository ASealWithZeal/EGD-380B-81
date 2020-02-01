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

    public void ChangeHP()
    {
        if (gameObject.tag == "Player")
            charUI.GetComponent<PlayerStatusUI>().ChangeHealth(charStats.HPPercent());
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
