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

    // Perform a simple attack
    public void Attack()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(attackName);
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        Managers.CombatManager.Instance.DealDamage(attackMod);
    }

    // Guards for a turn, raising defense
    public void Defend()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(defendName);
        Managers.CombatManager.Instance.SetTarget((int)defendTarget);
        Managers.CombatManager.Instance.UseStatusAbility(1, 2.0f, 1, true);
    }

    // Uses Heavy Slash
    public void Ability0()
    {
        // Animation
        charStats.SetSpdMod(0.01f, 1);
        charStats.currentTP -= ability0Cost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        Managers.CombatManager.Instance.DisplayAbilityName(ability0Name);

        // Adds the self-debuff
        Managers.CombatManager.Instance.SetTarget(6);
        Managers.CombatManager.Instance.UseStatusAbility(2, 0.01f, 1, false);

        // Sets up the next turn's action
        Managers.CombatManager.Instance.SetTarget((int)ability0Target);
        Managers.CombatManager.Instance.UseDelayedAbility(ability0Name, ability0Mod, 1, true);
    }
}
