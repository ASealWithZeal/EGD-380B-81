using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Targeting
{
    OneEnemy = 0,
    AllEnemies,
    TwoRandomEnemies,
    ThreeRandomEnemies,
    OneAlly,
    AllAllies,
    User
}

namespace Managers
{
    public class CombatManager : Singleton<CombatManager>
    {
        public GameObject combatMenu;
        public Transform moveTarget;
        public DamageTextUI dText = null;
        public AbilityNameDisplay aDisplay = null;
        public OverallWinCanvasScript winCanvas = null;
        public List<Vector3> positions = null;

        private List<GameObject> moveTargets = new List<GameObject>();

        public int DAMAGE_MULT = 10;
        private bool oneTarget = false;
        private bool doneAttacking = false;
        private int newTarget = 0;
        private float storedMod = 0.0f;
        private int winEXP = 0;
        private int storedAbility = -1;

        private Vector3 origPos;
        private bool endingTurn = false;
        private bool settingTarget = false;

        private void Start()
        {
            // Starts combat! - Temp
            // StartRound();
            winCanvas = GameObject.Find("WinCanvas").GetComponent<OverallWinCanvasScript>();
        }

        public void StartCombat()
        {
            StartCoroutine(MovePlayerCharacterAtStart(positions[TurnManager.Instance.playerCharsList.Count - 1]));
        }

        private void Update()
        {
            SelectTargets();
        }

        // Starts each character's round of combat
        public void StartRound()
        {
            endingTurn = false;
            if (TurnManager.Instance.t1[0].tag == "Player")
            {
                // Ticks down the characters modifier changes
                TurnManager.Instance.t1[0].GetComponent<Stats>().TickModifierChanges();
                TurnManager.Instance.t1[0].GetComponent<CharData>().delayTimer--;
                origPos = TurnManager.Instance.t1[0].transform.position;
                StartCoroutine(MovePlayerCharacter(moveTarget.position));
            }
        
            else
            {
                // Ticks down the characters modifier changes
                TurnManager.Instance.t1[0].GetComponent<Stats>().TickModifierChanges();
                TurnManager.Instance.t1[0].GetComponent<CharData>().delayTimer--;
                Act(0);
            }
        }

        // Performs an action
        public void Act(int action)
        {
            doneAttacking = false;
            newTarget = 0;
            if (TurnManager.Instance.t1[0].tag == "Player")
            {
                combatMenu.GetComponent<PlayerCombatMenuManager>().MakeButtonVisible(false);
                SetTarget(0);
                if (action == 1)
                    doneAttacking = true;
                TurnManager.Instance.t1[0].GetComponent<PlayerActions>().CheckAction(action);
            }

            else
            {
                CharData c = TurnManager.Instance.t1[0].GetComponent<CharData>();
                if (!c.delayedAttack || c.delayTimer > 0)
                    TurnManager.Instance.t1[0].GetComponent<EnemyActions>().PerformAction();
                else
                    PerformDelayedAbility(c);
            }
        }

        // Performs a follow-up action or instantly ends the turn
        public void FollowUpAction()
        {
            if (doneAttacking)
            {
                aDisplay.ChangeDisplayOpacity(false);

                endingTurn = true;
                bool canMoveOn = true;
                for (int i = 0; i < TurnManager.Instance.combatChars.Count; ++i)
                {
                    if (TurnManager.Instance.combatChars[i].GetComponent<Stats>().HP() <= 0)
                    {
                        if (TurnManager.Instance.combatChars[i].tag == "Player")
                        {
                            TurnManager.Instance.playerCharsList.Remove(TurnManager.Instance.combatChars[i]);
                            TurnManager.Instance.combatChars[i].transform.parent = TurnManager.Instance.nonCombatPlayer.transform;
                        }
                        else
                        {
                            // Increases the battle's EXP point gain
                            winEXP += TurnManager.Instance.combatChars[i].GetComponent<Stats>().exp;
                            TurnManager.Instance.enemyCharsList.Remove(TurnManager.Instance.combatChars[i]);
                            TurnManager.Instance.combatChars[i].transform.parent = TurnManager.Instance.nonCombatEnemies.transform;
                        }

                        TurnManager.Instance.combatChars[i].GetComponent<CharData>().KillChar();
                        TurnManager.Instance.combatChars.Remove(TurnManager.Instance.combatChars[i]);

                        i = -1;
                    }
                }

                if (TurnManager.Instance.enemyCharsList.Count == 0)
                {
                    canMoveOn = false;
                    for (int i = 0; i < TurnManager.Instance.playerCharsList.Count; ++i)
                        TurnManager.Instance.playerCharsList[i].GetComponent<Stats>().GainEXP(winEXP / TurnManager.Instance.playerCharsList.Count);
                    winCanvas.ShowWinCanvas(winEXP / TurnManager.Instance.playerCharsList.Count, TurnManager.Instance.playerCharsList[0].GetComponent<CharData>().combatInst);
                }
                else if (TurnManager.Instance.playerCharsList.Count == 0)
                {
                    canMoveOn = false;
                    Invoke("EndCombatLoss", 0.75f);
                }

                if (canMoveOn)
                {
                    for (int i = 0; i < moveTargets.Count; ++i)
                        moveTargets.Remove(moveTargets[i]);
                    newTarget = 0;

                    if (TurnManager.Instance.t1[0].tag == "Player")
                    {
                        StartCoroutine(MovePlayerCharacter(origPos));
                    }

                    else
                    {
                        TurnManager.Instance.EndRound();
                    }
                }
            }

            else
                DealDamage(storedMod);
        }

        private void EndCombatLoss()
        {
            SceneChangeManager.Instance.LoseCombat();
        }

        // Sets up an action before targeting the relevant character(s)
        public void SetUpAction(int type)
        {
            storedAbility = type;
            Invoke("SetTarget", 0.0125f);
            for (int i = 0; i < moveTargets.Count; ++i)
            {
                moveTargets[i].GetComponent<CharData>().Targeted();
                TurnManager.Instance.tracker.HighlightSelectedTrackers(moveTargets[i]);
            }
        }

        private void SetTarget()
        {
            settingTarget = true;
        }

        private void SelectTargets()
        {
            if (settingTarget)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                    PerformAction();

                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    for (int i = 0; i < moveTargets.Count; ++i)
                    {
                        moveTargets[i].GetComponent<CharData>().Targeted();
                        TurnManager.Instance.tracker.HighlightSelectedTrackers(moveTargets[i]);
                    }

                    settingTarget = false;
                    combatMenu.GetComponent<PlayerCombatMenuManager>().MakeButtonVisible(true);
                }

                else if (oneTarget && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
                {
                    if (moveTargets[0].tag == "Player" && TurnManager.Instance.playerCharsList.Count > 1)
                        TargetDown(TurnManager.Instance.playerCharsList);

                    else if (moveTargets[0].tag != "Player" && TurnManager.Instance.enemyCharsList.Count > 1)
                        TargetDown(TurnManager.Instance.enemyCharsList);
                }

                else if (oneTarget && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow)))
                {
                    if (moveTargets[0].tag == "Player" && TurnManager.Instance.playerCharsList.Count > 1)
                        TargetUp(TurnManager.Instance.playerCharsList);

                    else if (moveTargets[0].tag != "Player" && TurnManager.Instance.enemyCharsList.Count > 1)
                        TargetUp(TurnManager.Instance.enemyCharsList);
                }
            }
        }

        // Targets one below the current target
        private void TargetDown(List<GameObject> l)
        {
            int num = 0;
            for (int i = 0; i < l.Count; ++i)
            {
                if (l[i] == moveTargets[0] && i > 0)
                    num = i - 1;
                else if (l[i] == moveTargets[0] && i == 0)
                    num = l.Count - 1;
            }
            moveTargets[0].GetComponent<CharData>().Targeted();
            TurnManager.Instance.tracker.HighlightSelectedTrackers(moveTargets[0]);
            SetTarget(l[num]);
        }

        // Targets one above the current target
        private void TargetUp(List<GameObject> l)
        {
            int num = 0;
            for (int i = 0; i < l.Count; ++i)
            {
                if (l[i] == moveTargets[0] && i < l.Count - 1)
                    num = i + 1;
                else if (l[i] == moveTargets[0] && i == l.Count - 1)
                    num = 0;
            }
            moveTargets[0].GetComponent<CharData>().Targeted();
            TurnManager.Instance.tracker.HighlightSelectedTrackers(moveTargets[0]);
            SetTarget(l[num]);
        }

        private void PerformAction()
        {
            settingTarget = false;

            // Stops targeting ALL targets
            for (int i = 0; i < moveTargets.Count; ++i)
            {
                moveTargets[i].GetComponent<CharData>().Targeted();
                TurnManager.Instance.tracker.HighlightSelectedTrackers(moveTargets[i]);
            }

            if (TurnManager.Instance.t1[0].tag == "Player")
                TurnManager.Instance.t1[0].GetComponent<PlayerActions>().PerformAction(storedAbility);
        }

        public void SetTarget(int targetType)
        {
            for (int i = 0; i < moveTargets.Count; ++i)
                moveTargets.Remove(moveTargets[i]);

            switch (targetType)
            {
                // Target an enemy
                case (int)Targeting.OneEnemy:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        moveTargets.Add(TurnManager.Instance.enemyCharsList[0]);
                    else
                        EnemySetPlayerTarget();

                    oneTarget = true;
                    break;

                // Target all enemies
                case (int)Targeting.AllEnemies:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        for (int i = 0; i < TurnManager.Instance.enemyCharsList.Count; ++i)
                            moveTargets.Add(TurnManager.Instance.enemyCharsList[i]);
                    else
                        for (int i = 0; i < TurnManager.Instance.playerCharsList.Count; ++i)
                            moveTargets.Add(TurnManager.Instance.playerCharsList[i]);

                    oneTarget = false;
                    break;

                // Target two random enemies
                case (int)Targeting.TwoRandomEnemies:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        for (int i = 0; i < 2; ++i)
                            moveTargets.Add(TurnManager.Instance.enemyCharsList[Random.Range(0, TurnManager.Instance.enemyCharsList.Count)]);
                    else
                        for (int i = 0; i < 2; ++i)
                            EnemySetPlayerTarget();

                    oneTarget = true;
                    break;

                // Target three random enemies
                case (int)Targeting.ThreeRandomEnemies:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        for (int i = 0; i < 3; ++i)
                            moveTargets.Add(TurnManager.Instance.enemyCharsList[Random.Range(0, TurnManager.Instance.enemyCharsList.Count)]);
                    else
                        for (int i = 0; i < 3; ++i)
                            EnemySetPlayerTarget();

                    oneTarget = true;
                    break;

                // Target an ally
                case (int)Targeting.OneAlly:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        moveTargets.Add(TurnManager.Instance.playerCharsList[0]);
                    else
                        moveTargets.Add(TurnManager.Instance.enemyCharsList[Random.Range(0, TurnManager.Instance.enemyCharsList.Count)]);

                    oneTarget = true;
                    break;

                // Target all allies
                case (int)Targeting.AllAllies:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        for (int i = 0; i < TurnManager.Instance.playerCharsList.Count; ++i)
                            moveTargets.Add(TurnManager.Instance.playerCharsList[i]);
                    else
                        for (int i = 0; i < TurnManager.Instance.enemyCharsList.Count; ++i)
                            moveTargets.Add(TurnManager.Instance.enemyCharsList[i]);

                    oneTarget = false;
                    break;

                // Target the user
                case (int)Targeting.User:
                    moveTargets.Add(TurnManager.Instance.t1[0]);

                    oneTarget = false;
                    break;
            }
        }

        private void SetTarget(GameObject newTarget)
        {
            for (int i = 0; i < moveTargets.Count; ++i)
                moveTargets.Remove(moveTargets[i]);

            moveTargets.Add(newTarget);
            newTarget.GetComponent<CharData>().Targeted();
            TurnManager.Instance.tracker.HighlightSelectedTrackers(newTarget);
        }

        // Adds player characters to the enemy's target list; if they have aggro, guarantee the addition
        private void EnemySetPlayerTarget()
        {
            bool set = false;
            List<GameObject> tempList = new List<GameObject>();
            for (int i = 0; i < TurnManager.Instance.playerCharsList.Count; ++i)
            {
                if (TurnManager.Instance.playerCharsList[i].GetComponent<Stats>().aggro > 0)
                {
                    tempList.Add(TurnManager.Instance.playerCharsList[i]);
                    set = true;
                }
            }

            if (!set)
                moveTargets.Add(TurnManager.Instance.playerCharsList[Random.Range(0, TurnManager.Instance.playerCharsList.Count)]);
            else
                moveTargets.Add(tempList[Random.Range(0, tempList.Count)]);
        }

        private void AddEnemyTargets(Targeting t)
        {
            if (t == Targeting.OneEnemy)
                moveTargets.Add(TurnManager.Instance.enemyCharsList[0]);
            else if (t == Targeting.AllEnemies)
                for (int i = 0; i < TurnManager.Instance.enemyCharsList.Count; ++i)
                    moveTargets.Add(TurnManager.Instance.enemyCharsList[i]);
        }

        public void DisplayAbilityName(string newString)
        {
            aDisplay.ChangeNameDisplay(newString);
            aDisplay.ChangeDisplayOpacity(true);
        }

        public void EndTurnImmediately()
        {

        }

        // Deals damage to the current target(s) based on a shared modifier
        public void DealDamage(float modifier)
        {
            int targ = newTarget + 1;
            storedMod = modifier;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = newTarget; i < targ; ++i)
            {
                int damage = DamageFormula(TurnManager.Instance.t1[0].GetComponent<Stats>(), moveTargets[i].GetComponent<Stats>(), modifier);

                moveTargets[i].GetComponent<Stats>().ReduceHP(damage);
                moveTargets[i].GetComponent<CharData>().ChangeHP();

                bool numEnd = true;
                if (!oneTarget && i != targ - 1)
                    numEnd = false;

                dText.DamageNumbers(damage, moveTargets[i].transform, numEnd);
            }

            if (!oneTarget || newTarget == moveTargets.Count - 1)
                doneAttacking = true;
            else
                newTarget++;
        }

        // Deal damage AS A START / END OF TURN EFFECT
        //  Can be used for poison, mists, other effects
        public void DealDamageTurnStart(int damage, float textDuration)
        {
            int targ = 0;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = newTarget; i < targ; ++i)
            {
                moveTargets[i].GetComponent<Stats>().ReduceHP(damage);
                moveTargets[i].GetComponent<CharData>().ChangeHP();

                dText.DamageNumbers(damage, moveTargets[i].transform, false);
            }
            
            StartCoroutine(EndNonDamageTextDisplay(textDuration, false));
        }

        // Deals damage to the current target(s) based on a shared modifier
        public int DealDamageWithAbsorb(float modifier)
        {
            int damage = 0;
            int targ = 1;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = 0; i < targ; ++i)
            {
                damage = DamageFormula(TurnManager.Instance.t1[0].GetComponent<Stats>(), moveTargets[i].GetComponent<Stats>(), modifier);

                moveTargets[i].GetComponent<Stats>().ReduceHP(damage);
                moveTargets[i].GetComponent<CharData>().ChangeHP();

                dText.DamageNumbers(damage, moveTargets[i].transform, false);
            }

            return damage;
        }

        // Restores health to all specified targets
        public void RestoreHealth(int amount)
        {
            int targ = 1;
            bool canEnd = false;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = 0; i < targ; ++i)
            {
                moveTargets[i].GetComponent<Stats>().ReduceHP(-amount);
                moveTargets[i].GetComponent<CharData>().ChangeHP();

                if (i == targ - 1)
                    canEnd = true;

                dText.DamageNumbers(-amount, moveTargets[i].transform, canEnd);
            }

            doneAttacking = true;
        }

        // Performs an ability with a non-damaging effect
        public void UseStatusAbility(int type, float mod, int length, bool endTurn, float textDuration)
        {
            int targ = 1;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = 0; i < targ; ++i)
            {
                if (type == 0)
                    moveTargets[i].GetComponent<Stats>().SetAtkMod(mod, length);
                if (type == 1)
                    moveTargets[i].GetComponent<Stats>().SetDefMod(mod, length);
                if (type == 2)
                    moveTargets[i].GetComponent<Stats>().SetSpdMod(mod, length);
                if (type == 3)
                    moveTargets[i].GetComponent<Stats>().SetAggro(mod, length);
            }

            if (endTurn)
                StartCoroutine(EndNonDamageTextDisplay(textDuration, endTurn));
        }

        // Prepares to let the user perform a delayed attack
        public void UseDelayedAbility(string name, float mod, int delay, bool endTurn, float textDuration)
        {
            TurnManager.Instance.t1[0].GetComponent<CharData>().SetDelayedAttack(name, delay, mod, moveTargets);

            if (endTurn)
                StartCoroutine(EndNonDamageTextDisplay(textDuration, endTurn));
        }

        private void PerformDelayedAbility(CharData c)
        {
            c.delayedAttack = false;
            DisplayAbilityName(c.delayedAbilityName);

            for (int i = 0; i < moveTargets.Count; ++i)
                moveTargets.Remove(moveTargets[i]);

            for (int i = 0; i < c.target.Count; ++i)
            {
                if (c.target[i] == null && c.target.Count == 1)
                    c.target[i] = TurnManager.Instance.enemyCharsList[0];

                moveTargets.Add(c.target[i]);
            }

            if (c.target.Count > 1)
                oneTarget = false;

            DealDamage(c.storedModifier);
        }

        IEnumerator EndNonDamageTextDisplay(float time, bool followUp)
        {
            yield return new WaitForSeconds(time);
            doneAttacking = true;
            aDisplay.ChangeDisplayOpacity(false);
            if (followUp)
                FollowUpAction();
            yield return null;
        }

        private int DamageFormula(Stats a, Stats t, float mod)
        {
            //int i = (int)(((a.Attack() * a.Attack()) / t.Defense()) * mod * Random.Range(0.8f, 1.0f));
            float i = ((a.Attack() * (a.Attack() / 2)) * (1 + (1 - (0.3f * a.level))) * DAMAGE_MULT) / t.Defense();
            i *= mod;
            i *= Random.Range(0.8f, 1.0f);
            return (int)i;
        }

        public void ChangeVisibleMenuButtons(bool change)
        {
            combatMenu.GetComponent<PlayerCombatMenuManager>().ShowAbilities(change, TurnManager.Instance.t1[0]);
        }

        IEnumerator MovePlayerCharacter(Vector3 targetPos)
        {
            CharData c = TurnManager.Instance.t1[0].GetComponent<CharData>();

            while (TurnManager.Instance.t1[0].transform.position != targetPos)
            {
                TurnManager.Instance.t1[0].transform.position = Vector3.MoveTowards(TurnManager.Instance.t1[0].transform.position, targetPos, 0.125f);
                yield return new WaitForSeconds(0.0125f);
            }

            // IN FUTURE CHANGE
            if (!c.delayedAttack || c.delayTimer > 0)
            {
                if (endingTurn)
                {
                    endingTurn = false;
                    TurnManager.Instance.EndRound();
                }

                else
                {
                    combatMenu.GetComponent<PlayerCombatMenuManager>().MakeButtonVisible(true);
                }
            }

            else
                PerformDelayedAbility(c);

            yield return null;
        }

        IEnumerator MovePlayerCharacterAtStart(Vector3 targetPos)
        {
            Debug.Log(targetPos);
            while (TurnManager.Instance.playerCharsList[0].transform.position != targetPos)
            {
                TurnManager.Instance.playerCharsList[0].transform.position = Vector3.MoveTowards(TurnManager.Instance.playerCharsList[0].transform.position, targetPos, 0.125f);
                yield return new WaitForSeconds(0.0125f);
            }

            yield return new WaitForSeconds(0.1f);
            StartRound();
        }
    }
}