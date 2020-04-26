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
    public GameObject attackAnim;

    [Header("Defend")]
    public string defendName = "Defend";
    public float defendMod = 0.0f;
    public Targeting defendTarget;

    [Header("Ability0")]
    public string ability0Name = "Heavy Slash";
    public bool ability0Active = true;
    public string ability0Description = "Gain -99% SPD for 1 turn. During your next turn, deal 2.5x damage to an enemy.";
    public float ability0Mod = 0.0f;
    public int ability0Delay = 1;
    public int ability0Cost = 6;
    public Targeting ability0Target;
    public GameObject ability0SetupAnim;
    public GameObject ability0ActionAnim;

    [Header("Ability0 Upgrade")]
    public string ability0UpgradeName = "Guard Overhead";
    public string ability0UpgradeDescription = "Gain -99% SPD and +50% DEF for 1 turn. During your next turn, deal 3.0x damage to an enemy.";
    public float ability0UpgradeMod = 3.0f;
    public float ability0UpgradeDefMod = 1.25f;
    public int ability0UpgradeCost = 8;
    public Targeting ability0UpgradeTarget;
    public GameObject ability0UpgradeSetupAnim;
    public GameObject ability0UpgradeActionAnim;

    [Header("Ability1")]
    public string ability1Name = "Provoke";
    public bool ability1Active = true;
    public string ability1Description = "All enemies will target the user for 3 turns.";
    public int ability1Cost = 4;
    public float ability1Effect = 1.0f;
    public int ability1Duration = 3;
    public Targeting ability1Target;
    public GameObject ability1Anim;

    [Header("PassiveAbility1")]
    public string ability2Name = "Patience";
    public bool ability2Active = false;
    public string ability2Description = "If the user ends a turn without dealing damage, gain +25% ATK for 1 turn.";
    public bool ability2Trigger = false;
    public GameObject ability2Anim;

    [Header("PassiveAbility2")]
    public string ability3Name = "Wall of Blades";
    public bool ability3Active = false;
    public MainStats ability3Recipient = MainStats.DEF;
    public int ability3RecipientGains = 1;
    public MainStats ability3Giver = MainStats.ATK;
    public int ability3GiverGives = 3;
    public string ability3Description = "User gains +1 DEF for every 3 points of ATK.";

    // Perform a simple attack
    public void SetAttack()
    {
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        Managers.CombatManager.Instance.SetUpAction(0);
    }
    public void PerformAttack()
    {
        if (GetComponent<CharData>().learnedAbilities[2])
            ability2Trigger = false;

        // Animation
        GetComponent<CharData>().DisplayActionAnimation();
        Managers.CombatManager.Instance.DisplayAbilityName(attackName);
        Managers.CombatManager.Instance.DealDamage(attackAnim, attackMod);
        //GameObject g = Instantiate(attackAnim, GameObject.Find("_AbilityDrawCanvas").transform);
        //GameObject.Find("_AbilityDrawCanvas").GetComponent<AbilityEffectsGenerator>().CreateAnimation(attackAnim, Managers.CombatManager.Instance.moveTargets[0].transform.position);
    }

    // Guards for a turn, raising defense
    public void SetDefend()
    {
        Managers.CombatManager.Instance.SetTarget((int)defendTarget);
        Managers.CombatManager.Instance.SetUpAction(1);
    }
    public void PerformDefend()
    {
        if (GetComponent<CharData>().learnedAbilities[2])
            ability2Trigger = true;

        // Animation
        List<int> type = new List<int>();
        type.Add(4);
        List<float> mod = new List<float>();
        mod.Add(0);
        List<int> length = new List<int>();
        length.Add(0);

        GetComponent<CharData>().DisplayActionAnimation();
        Managers.CombatManager.Instance.DisplayAbilityName(defendName);
        Managers.CombatManager.Instance.UseStatusAbility(null, type, mod, length, true, 0.5f);
    }

    // Uses Heavy Slash
    public void SetAbility0()
    {
        Managers.CombatManager.Instance.SetTarget((int)ability0Target);
        Managers.CombatManager.Instance.SetUpAction(2);
    }
    public void PerformAbility0()
    {
        if (GetComponent<CharData>().learnedAbilities[2])
            ability2Trigger = true;

        // Animation
        charStats.currentTP -= ability0Cost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        Managers.CombatManager.Instance.DisplayAbilityName(ability0Name);
        GetComponent<CharData>().DisplayActionAnimation();
        Managers.CombatManager.Instance.UseDelayedAbility(ability0ActionAnim, ability0Target, ability0Name, ability0Mod, 1, false, 0.5f, ability0SetupAnim.GetComponent<Animation>().clip.length);

        // Adds the self-debuff
        List<int> type = new List<int>();
        type.Add(2);
        List<float> mod = new List<float>();
        mod.Add(-0.99f);
        List<int> length = new List<int>();
        length.Add(1);

        Managers.CombatManager.Instance.SetTarget((int)Targeting.User);
        Managers.CombatManager.Instance.UseStatusAbility(ability0SetupAnim, type, mod, length, true, 0.5f);
    }
    public void PerformAbility0Upgrade()
    {
        if (GetComponent<CharData>().learnedAbilities[2])
            ability2Trigger = true;

        // Animation
        charStats.currentTP -= ability0UpgradeCost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();

        GetComponent<CharData>().DisplayActionAnimation();
        Managers.CombatManager.Instance.DisplayAbilityName(ability0UpgradeName);
        Managers.CombatManager.Instance.UseDelayedAbility(ability0UpgradeActionAnim, ability0UpgradeTarget, ability0UpgradeName, ability0UpgradeMod, 1, false, 0.5f, ability0UpgradeSetupAnim.GetComponent<Animation>().clip.length);

        // Adds the self-debuff
        List<int> type = new List<int>();
        type.Add(2);        type.Add(1);
        List<float> mod = new List<float>();
        mod.Add(-0.99f);    mod.Add(ability0UpgradeMod);
        List<int> length = new List<int>();
        length.Add(1);      length.Add(1);

        Managers.CombatManager.Instance.SetTarget((int)Targeting.User);
        Managers.CombatManager.Instance.UseStatusAbility(ability0UpgradeSetupAnim, type, mod, length, true, 0.5f);
    }

    // Uses Provoke
    public void SetAbility1()
    {
        Managers.CombatManager.Instance.SetTarget((int)ability1Target);
        Managers.CombatManager.Instance.SetUpAction(3);
    }
    public void PerformAbility1()
    {
        if (GetComponent<CharData>().learnedAbilities[2])
            ability2Trigger = true;

        // Animation
        charStats.currentTP -= ability1Cost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();

        GetComponent<CharData>().DisplayActionAnimation();
        Managers.CombatManager.Instance.DisplayAbilityName(ability1Name);

        // Adds the self-debuff
        List<int> type = new List<int>();
        type.Add(3);
        List<float> mod = new List<float>();
        mod.Add(ability1Effect);
        List<int> length = new List<int>();
        length.Add(ability1Duration);

        Managers.CombatManager.Instance.UseStatusAbility(ability1Anim, type, mod, length, true, 0.5f);
    }

    public void GetPassiveAbility1()
    {
        //charStats.GetPassiveAbilityStats(MainStats.HP, 0.25f);
    }

    public void UsePassiveAbility1()
    {
        if (ability2Trigger)
        {
            GetComponent<CharData>().DisplayActionAnimation();
            Managers.CombatManager.Instance.DisplayAbilityName(ability2Name);

            List<int> type = new List<int>();
            type.Add(0);
            List<float> mod = new List<float>();
            mod.Add(0.25f);
            List<int> length = new List<int>();
            length.Add(1);

            Managers.CombatManager.Instance.SetTarget((int)Targeting.User);
            Managers.CombatManager.Instance.UseStatusAbility(ability2Anim, type, mod, length, true, 0.5f);

            ability2Trigger = false;
        }
    }

    public void GetPassiveAbility2()
    {
        charStats.GetPassiveAbilityStats(ability3Recipient, ability3Giver, ability3RecipientGains, ability3GiverGives);
    }
}
