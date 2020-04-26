using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public Stats charStats = null;

    [Header("Normal Attack")]
    public string attackName = "Attack";
    public float attackMod = 1.0f;
    public float attackPercent = 50.0f;
    public Targeting attackTarget;
    public GameObject attackAnim;

    [Header("Special 0")]
    public string special0Name = "Beak Flurry";
    public float special0Mod = 0.6f;
    public float special0Percent = 30.0f;
    public Targeting special0Target;
    public GameObject special0Anim;

    [Header("Special 1")]
    public string special1Name = "Pyre";
    public float special1Mod = 1.2f;
    public float special1Percent = 20.0f;
    public Targeting special1Target;
    public GameObject special1Anim;

    public void Act()
    {
        float chance = Random.Range(0, 99.9f);

        if (chance >= 0 && chance < attackPercent)
            Attack();
        else if (chance >= attackPercent && chance < attackPercent + special0Percent)
            UseBeakStab();
        else if (chance >= attackPercent + special0Percent && chance < 100)
            UsePyre();
    }

    // Perform a simple attack
    public void Attack()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(attackName);
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        Managers.CombatManager.Instance.DealDamage(attackAnim, attackMod);
    }

    public void UseBeakStab()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(special0Name);
        Managers.CombatManager.Instance.SetTarget((int)special0Target);
        Managers.CombatManager.Instance.DealDamage(special0Anim, special0Mod);
    }

    public void UsePyre()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(special1Name);
        Managers.CombatManager.Instance.SetTarget((int)special1Target);
        Managers.CombatManager.Instance.DealDamage(special1Anim, special1Mod);
    }
}
