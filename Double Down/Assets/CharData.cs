using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharData : MonoBehaviour
{
    public string name;

    public bool targeting;
    public int t1Pos = 0;
    public int t2Pos = 0;
    private Vector3 initialPos = new Vector3();
    public Vector3 hubPosition = new Vector3();
    public Vector3 combatPosition = new Vector3();
    public int charNum = -1;

    [Header("Event Info")]
    public bool dead = false;
    [HideInInspector] public int deathTurns = 3;
    public int attachedEventNum = 0;

    [Header("Combat Info")]
    public bool isInCombat = false;
    public int combatInst = -1;

    [Header("Abilities")]
    public List<bool> learnedAbilities = new List<bool>();

    [Header("Delayed Attack Info")]
    public string delayedAbilityName;
    public bool delayedAttack;
    public int delayTimer;
    public float storedModifier;
    public List<GameObject> target;

    [Header("Sprites")]
    public bool facingDir;
    public Color[] colors;
    public bool hasFinishedActionAnimation = false;

    public GameObject charUI;
    public WinCanvasScript charWinUI;
    public Stats charStats;
    public CharAnimator charAnimator;
    public bool hasActed = false;

    private void Start()
    {
        facingDir = GetComponent<SpriteRenderer>().flipX;
        initialPos = gameObject.transform.localPosition;
        if (gameObject.tag == "Player")
            SetCharUI();
    }

    public void SetCharUI()
    {
        if (gameObject.tag == "Player")
        {
            charUI.GetComponent<PlayerStatusUI>().SetNewHP(name, charStats.level, charStats.currentHP, charStats.MaxHP(), charStats.currentTP, charStats.MaxTP());
            charWinUI?.Init();
        }
        else if (gameObject.tag == "Enemy")
        {
            Vector3 objPosition = new Vector3(transform.position.x, GetComponent<EnemyAnimator>().defaultY, transform.position.z);
            charUI.GetComponent<EnemyUI>().CreateUI(name, objPosition, gameObject, charStats.HPPercent());
        }
    }

    public void ChangeCharUIDisplay()
    {
        charUI.GetComponent<PlayerStatusUI>().SetNewHP(name, charStats.level, charStats.currentHP, charStats.MaxHP(), charStats.currentTP, charStats.MaxTP());
    }

    public void ChangeColor()
    {
        if (!dead)
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        else
            gameObject.GetComponent<SpriteRenderer>().color = colors[1];
    }

    public Color GetColor()
    {
        if (!dead)
            return colors[0];
        else
            return colors[0];
    }

    public void FullRestore()
    {
        charStats.currentHP = charStats.MaxHP();
        charStats.currentTP = charStats.MaxTP();

        if (gameObject.tag == "Player")
        {
            ChangeHP();
            ChangeTP();
        }
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

    // Juts out a character's UI to show they are currently acting
    public void MoveCharUI(bool forward)
    {
        charUI.GetComponent<PlayerStatusUI>().MoveUI(forward);
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

    // Recalculates each character's bonus stats at the beginning of each turn
    public void SetBonusStats()
    {
        charStats.ResetBonusStats();
        if (gameObject.tag == "Player")
        {
            for (int i = 0; i < learnedAbilities.Count; ++i)
                if (learnedAbilities[i])
                    GetComponent<PlayerActions>().CalculatePassiveBonus(i);
        }
    }

    public void Targeted()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
    }

    public void DisplayActionAnimation()
    {
        hasFinishedActionAnimation = false;
        StartCoroutine(ActionCoroutine());
    }

    IEnumerator ActionCoroutine()
    {
        if (tag != "Player")
        {
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
            yield return new WaitForSeconds(0.05f);
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
            yield return new WaitForSeconds(0.05f);
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
            yield return new WaitForSeconds(0.05f);
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
            yield return new WaitForSeconds(0.05f);
        }

        hasFinishedActionAnimation = true;
        yield return null;
    }



    public void KillChar()
    {
        StartCoroutine(DeathAnim());
    }

    // Counts down the turns until a character revives
    public void CountDownDeathTurns()
    {
        deathTurns--;

        // When all turns have passed, revive the character at their starting position
        if (deathTurns == 0)
        {
            deathTurns = 3;
            dead = false;
            ChangeColor();
            
            charStats.currentHP = charStats.MaxHP();
            charStats.currentTP = charStats.MaxTP();
            ChangeHP();
            ChangeTP();

            //gameObject.transform.localPosition = initialPos;
            gameObject.SetActive(true);
        }
    }

    // After transitioning into the hub, check all player characters to see if anyone's dead
    public void CheckDeath()
    {
        if (dead)
        {
            GetComponent<SpriteRenderer>().color = colors[1];
            transform.position = initialPos;
        }
    }

    private void ResetOnDeath()
    {
        isInCombat = false;
        combatInst = -1;
        deathTurns = 3;

        delayedAttack = false;
    }

    IEnumerator DeathAnim()
    {
        dead = true;
        ResetOnDeath();
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

        Color storedColor = sr.color;
        while (sr.color.a > 0)
        {
            sr.color -= new Color(0.07f, 0.07f, 0.07f, 0.1f);
            yield return new WaitForSeconds(0.025f);
        }
        sr.color = new Color(storedColor.r, storedColor.g, storedColor.b, 0);

        if (gameObject.tag != "Player")
        {
            GetComponent<EnemyAnimator>().HideShadow();
            charUI.SetActive(false);
        }
        else
            transform.position = new Vector3(-9, -0.5f, -2);
        yield return null;
    }

    public void CopyData(CharData c)
    {
        int temp = Mathf.Abs(learnedAbilities.Count - c.learnedAbilities.Count);
        if (temp > 0)
            for (int i = 0; i < temp; ++i)
                learnedAbilities.Add(false);

        for (int i = 0; i < c.learnedAbilities.Count; ++i)
            learnedAbilities[i] = c.learnedAbilities[i];

        isInCombat = c.isInCombat;
        t1Pos = c.t1Pos;
        t2Pos = c.t2Pos;
        charNum = c.charNum;
    }
}
