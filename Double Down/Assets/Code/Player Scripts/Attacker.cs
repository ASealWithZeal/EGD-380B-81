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
        Managers.CombatManager.Instance.UseDelayedAbility(ability0Name, ability0Mod, 1, true, 0.5f);

        // Adds the self-debuff
        Managers.CombatManager.Instance.SetTarget(6);
        Managers.CombatManager.Instance.UseStatusAbility(2, 0.01f, 1, false, 0.5f);
    }
}
