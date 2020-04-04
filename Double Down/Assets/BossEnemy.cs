using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public Stats charStats = null;

    [Header("Normal Attack")]
    public string attackName = "Attack";
    public float attackMod = 1.0f;
    public float attackPercent = 50.0f;
    public float attackPercent2 = 50.0f;
    public Targeting attackTarget;

    [Header("Special 0")]
    public string special0Name = "Feather Storm";
    public float special0Mod = 0.5f;
    public float special0Percent = 30.0f;
    public float special0Percent2 = 30.0f;
    public Targeting special0Target;

    [Header("Special 1")]
    public string special1Name = "Pyre";
    public float special1Mod = 1.2f;
    public float special1Percent = 20.0f;
    public float special1Percent2 = 20.0f;
    public Targeting special1Target;

    [Header("Special 2")]
    public string special2DelayName = "The magpie lets out a furious caw.";
    public string special2Name = "Pyrea";
    public float special2Mod = 1.65f;
    public float special2Percent = 0.0f;
    public float special2Percent2 = 20.0f;
    public int special2Delay = 1;
    public Targeting special2Target;

    [Header("Special 3")]
    public string special3Name = "The magpie releases a burning mist.";
    public float special3Mod = 0.25f;
    public int special3Length = -10;
    public bool special3Used = false;
    public Targeting special3Target;

    [Header("Special 4")]
    public string special4Name = "The magpie's wingbeats grow faster, still!";
    public float special4Mod = 0.25f;
    public int special4Length = -10;
    public bool special4Used = false;
    public Targeting special4Target;

    public void Act()
    {
        float chance = Random.Range(0, 99.9f);

        // At >50% HP, use this "block" of skills:
        if (!special3Used && charStats.currentHP > (0.5f * charStats.MaxHP()))
        {
            if (chance >= 0 && chance < attackPercent)
                Attack();
            else if (chance >= attackPercent && chance < attackPercent + special0Percent)
                UseFeatherStorm();
            else if (chance >= attackPercent + special0Percent && chance < 100)
                UsePyre();
        }

        // After using Special3, use these skills:
        else if (special3Used && !special4Used && charStats.currentHP > (0.25f * charStats.MaxHP()))
        {
            if (chance >= 0 && chance < attackPercent2)
                Attack();
            else if (chance >= attackPercent2 && chance < attackPercent2 + special0Percent2)
                UseFeatherStorm();
            else if (chance >= attackPercent2 + special0Percent2 && chance < attackPercent2 + special0Percent2 + special1Percent2)
                UsePyre();
            else if (chance >= attackPercent2 + special0Percent2 + special1Percent2 && chance < 100)
                UsePyrea();
        }

        // After using Special4, use these skills:
        else if (special4Used)
        {
            if (chance >= 0 && chance < attackPercent2)
                Attack();
            else if (chance >= attackPercent2 && chance < attackPercent2 + special0Percent2)
                UseFeatherStorm();
            else if (chance >= attackPercent2 + special0Percent2 && chance < attackPercent2 + special0Percent2 + special1Percent2)
                UsePyre();
            else if (chance >= attackPercent2 + special0Percent2 + special1Percent2 && chance < 100)
                UsePyrea();
        }

        // At 50% HP or less, use this skill once:
        else if (!special3Used && charStats.currentHP <= (0.5f * charStats.MaxHP()) && charStats.currentHP > (0.25f * charStats.MaxHP()))
            UseBurningMist();

        // At 25% HP or less, use this skill once:
        else
            UseSpecial4();
    }

    // Perform a simple attack
    public void Attack()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(attackName);
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        Managers.CombatManager.Instance.DealDamage(attackMod);
    }

    public void UseFeatherStorm()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(special0Name);
        Managers.CombatManager.Instance.SetTarget((int)special0Target);
        Managers.CombatManager.Instance.DealDamage(special0Mod);
    }

    public void UsePyre()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(special1Name);
        Managers.CombatManager.Instance.SetTarget((int)special1Target);
        Managers.CombatManager.Instance.DealDamage(special1Mod);
    }

    public void UsePyrea()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(special2DelayName);
        Managers.CombatManager.Instance.SetTarget((int)special2Target);
        Managers.CombatManager.Instance.UseDelayedAbility(special2Name, special2Mod, special2Delay, true, 1.5f);
    }

    public void UseBurningMist()
    {
        // Animation
        special3Used = true;
        //charStats.atk += 2;
        //charStats.def -= 2;
        Managers.CombatManager.Instance.DisplayAbilityName(special3Name);
        Managers.CombatManager.Instance.SetTarget((int)special3Target);
        Managers.CombatManager.Instance.UseStatusAbility(0, special3Mod, special3Length, false, 2.0f);
        Managers.CombatManager.Instance.UseStatusAbility(2, special3Mod, special3Length, true, 2.0f);
    }

    public void UseSpecial4()
    {
        // Animation
        special4Used = true;
        //charStats.atk += 2;
        //charStats.def -= 2;
        Managers.CombatManager.Instance.DisplayAbilityName(special4Name);
        Managers.CombatManager.Instance.SetTarget((int)special4Target);
        Managers.CombatManager.Instance.UseStatusAbility(0, special4Mod, special4Length, false, 2.0f);
        Managers.CombatManager.Instance.UseStatusAbility(2, special4Mod, special4Length, true, 2.0f);
    }
}
