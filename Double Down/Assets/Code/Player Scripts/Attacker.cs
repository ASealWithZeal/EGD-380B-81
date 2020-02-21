using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    public Stats charStats = null;

    [Header("Attack")]
    public string attackName = "Attack";
    public float attackMod = 1.0f;
    public Targeting attackTarget;

    [Header("Defend")]
    public string defendName = "Defend";
    public float defendMod = 0.0f;
    public Targeting defendTarget;

    [Header("Ability0")]
    public string ability0Name = "Heavy Slash";
    public float ability0Mod = 0.0f;
    public int ability0Delay = 1;
    public int ability0Cost = 6;
    public Targeting ability0Target;

    [Header("Ability0 Upgrade")]
    public string ability0UpgradeName = "Guard Overhead";
    public float ability0UpgradeMod = 3.0f;
    public float ability0UpgradeDefMod = 1.25f;
    public int ability0UpgradeCost = 8;
    public Targeting ability0UpgradeTarget;

    [Header("Ability1")]
    public string ability1Name = "Provoke";
    public int ability1Cost = 4;
    public float ability1Effect = 1.0f;
    public int ability1Duration = 3;
    public Targeting ability1Target;

    // Perform a simple attack
    public void SetAttack()
    {
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        Managers.CombatManager.Instance.SetUpAction(0);
    }
    public void PerformAttack()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(attackName);
        Managers.CombatManager.Instance.DealDamage(attackMod);
    }

    // Guards for a turn, raising defense
    public void SetDefend()
    {
        Managers.CombatManager.Instance.SetTarget((int)defendTarget);
        Managers.CombatManager.Instance.SetUpAction(1);
    }
    public void PerformDefend()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(defendName);
        Managers.CombatManager.Instance.UseStatusAbility(1, 2.0f, 1, true, 0.5f);
    }

    // Uses Heavy Slash
    public void SetAbility0()
    {
        Managers.CombatManager.Instance.SetTarget((int)ability0Target);
        Managers.CombatManager.Instance.SetUpAction(2);
    }
    public void PerformAbility0()
    {
        // Animation
        charStats.currentTP -= ability0Cost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        Managers.CombatManager.Instance.DisplayAbilityName(ability0Name);
        Managers.CombatManager.Instance.UseDelayedAbility(ability0Name, ability0Mod, 1, false, 0.5f);

        // Adds the self-debuff
        Managers.CombatManager.Instance.SetTarget(6);
        Managers.CombatManager.Instance.UseStatusAbility(2, 0.01f, 1, true, 0.5f);
    }
    public void PerformAbility0Upgrade()
    {
        // Animation
        charStats.currentTP -= ability0UpgradeCost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        Managers.CombatManager.Instance.DisplayAbilityName(ability0UpgradeName);
        Managers.CombatManager.Instance.UseDelayedAbility(ability0UpgradeName, ability0UpgradeMod, 1, false, 0.5f);

        // Adds the self-debuff
        Managers.CombatManager.Instance.SetTarget(6);
        Managers.CombatManager.Instance.UseStatusAbility(2, 0.01f, 1, false, 0.5f);
        Managers.CombatManager.Instance.UseStatusAbility(1, ability0UpgradeDefMod, 1, true, 0.5f);
    }

    // Uses Provoke
    public void SetAbility1()
    {
        Managers.CombatManager.Instance.SetTarget((int)ability1Target);
        Managers.CombatManager.Instance.SetUpAction(3);
    }
    public void PerformAbility1()
    {
        // Animation
        charStats.currentTP -= ability1Cost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        Managers.CombatManager.Instance.DisplayAbilityName(ability1Name);
        Managers.CombatManager.Instance.UseStatusAbility(3, ability1Effect, ability1Duration, true, 0.5f);
    }
}
