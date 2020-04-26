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
    OneDifferentAlly,
    User
}

namespace Managers
{
    public class CombatManager : Singleton<CombatManager>
    {
        public GameObject combatMenu;
        public GameObject animDrawer;
        private GameObject anim;
        public Transform moveTarget;
        public DamageTextUI dText = null;
        public AbilityNameDisplay aDisplay = null;
        public OverallWinCanvasScript winCanvas = null;
        public ExpContainer container = null;
        public List<Vector3> positions = null;

        [HideInInspector] public List<GameObject> moveTargets = new List<GameObject>();

        public int DAMAGE_MULT = 10;
        private bool oneTarget = false;
        private bool otherTarget = false;
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
            container = GameObject.Find("_ExpContainer").GetComponent<ExpContainer>();
            winCanvas = GameObject.Find("WinCanvas").GetComponent<OverallWinCanvasScript>();

            GameObject.Find("EnemyData").GetComponent<EnemyUIHolder>().CreateUI();
        }

        public void StartCombat()
        {
            if (TurnManager.Instance.playerCharsList.Count == 1)
                StartCoroutine(MovePlayerCharacterAtStart(positions[0], 0, true));
            else
            {
                StartCoroutine(MovePlayerCharacterAtStart(positions[1], 0, true));
                StartCoroutine(MovePlayerCharacterAtStart(positions[2], 1, false));
            }
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
                // Ticks down the character's modifier changes
                if (TurnManager.Instance.t1[0].GetComponent<Stats>().defending == true)
                {
                    TurnManager.Instance.t1[0].GetComponent<Stats>().defending = false;
                    TurnManager.Instance.t1[0].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.CombatIdle);
                }

                TurnManager.Instance.t1[0].GetComponent<CharData>().delayTimer--;
                origPos = TurnManager.Instance.t1[0].transform.position;
                StartCoroutine(MovePlayerCharacter(moveTarget.position));
            }
        
            else
            {
                // Ticks down the character's modifier changes
                if (TurnManager.Instance.t1[0].GetComponent<Stats>().defending == true)
                {
                    TurnManager.Instance.t1[0].GetComponent<Stats>().defending = false;
                }

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
                {
                    c.DisplayActionAnimation();
                    TurnManager.Instance.t1[0].GetComponent<EnemyActions>().PerformAction();
                }
                else
                    PerformDelayedAbility(c);
            }
        }

        // Performs a follow-up action or instantly ends the turn
        public void FollowUpAction()
        {
            if (doneAttacking)
                EndTurn();

            else
                DealDamage(anim, storedMod);
        }

        // Ends the current turn
        private void EndTurn()
        {
            aDisplay.ChangeDisplayOpacity(false);
            endingTurn = true;
            bool canMoveOn = true;

            CheckForDeath();

            // If every enemy is dead, players win the battle
            if (TurnManager.Instance.enemyCharsList.Count == 0)
            {
                canMoveOn = false;
                if (!container.CheckForBossEvent())
                    Invoke("EndCombatWin", 0.5f);
                else
                    Invoke("EndCombatWin", 8f);
            }

            // If every player character is dead, they lose the battle
            else if (TurnManager.Instance.playerCharsList.Count == 0)
            {
                canMoveOn = false;
                Invoke("EndCombatLoss", 0.75f);
            }

            // If a player character can perform an "end-of-turn" effect, do so
            else if (TurnManager.Instance.t1[0].tag == "Player" && TurnManager.Instance.t1[0].GetComponent<PlayerActions>().CheckEndTurnEffects())
            {
                canMoveOn = false;
                Invoke("CallEndOfTurnEffect", 0.5f);
            }

            // If none of the above is true, just end the turn here
            if (canMoveOn)
            {
                int num = moveTargets.Count;
                for (int i = 0; i < num; ++i)
                    moveTargets.Remove(moveTargets[0]);
                newTarget = 0;

                // Ticks player moidifiers and moves them to their original position
                if (TurnManager.Instance.t1[0].tag == "Player")
                {
                    TurnManager.Instance.t1[0].GetComponent<Stats>().TickModifierChanges();
                    TurnManager.Instance.t1[0].GetComponent<CharData>().ChangeCharUIBuffDisplay();
                    StartCoroutine(MovePlayerCharacter(origPos));
                }

                // Ticks enemy modifiers
                else
                {
                    TurnManager.Instance.t1[0].GetComponent<Stats>().TickModifierChanges();
                    TurnManager.Instance.t1[0].GetComponent<CharData>().ChangeCharUIBuffDisplay();
                    TurnManager.Instance.EndRound();
                }
            }
        }

        // Checks each character at the end of a turn to see if they've died
        private void CheckForDeath()
        {
            for (int i = 0; i < TurnManager.Instance.combatChars.Count; ++i)
            {
                if (TurnManager.Instance.combatChars[i].GetComponent<Stats>().HP() <= 0)
                {
                    if (TurnManager.Instance.combatChars[i].tag == "Player")
                    {
                        TurnManager.Instance.playerCharsList.Remove(TurnManager.Instance.combatChars[i]);
                        //TurnManager.Instance.combatChars[i].transform.parent = TurnManager.Instance.nonCombatPlayer.transform;
                    }
                    else
                    {
                        // Increases the battle's EXP point gain
                        container.AddWinExp(TurnManager.Instance.combatChars[i].GetComponent<Stats>().exp);
                        TurnManager.Instance.enemyCharsList.Remove(TurnManager.Instance.combatChars[i]);
                        TurnManager.Instance.combatChars[i].transform.parent = TurnManager.Instance.nonCombatEnemies.transform;
                    }

                    TurnManager.Instance.combatChars[i].GetComponent<CharData>().KillChar();
                    TurnManager.Instance.combatChars.Remove(TurnManager.Instance.combatChars[i]);

                    i = -1;
                }
            }
        }

        private void CallEndOfTurnEffect()
        {
            TurnManager.Instance.t1[0].GetComponent<PlayerActions>().UseEndOfTurnEffect();
        }

        private void EndCombatWin()
        {
            for (int i = 0; i < TurnManager.Instance.playerCharsList.Count; ++i)
            {
                TurnManager.Instance.playerCharsList[i].GetComponent<Stats>().DestroyMods();
                TurnManager.Instance.playerCharsList[i].GetComponent<Stats>().GainEXP(container.GetWinExp() / TurnManager.Instance.playerCharsList.Count);
            }
            winCanvas.ShowWinCanvas(container.GetWinExp() / TurnManager.Instance.playerCharsList.Count, TurnManager.Instance.playerCharsList[0].GetComponent<CharData>().combatInst, container.CheckForBossEvent());
        }

        private void EndCombatLoss()
        {
            bool allDead = true;
            // Checks if all player characters are dead before making a decision:
            for (int i = 0; i < TurnManager.Instance.nonCombatPlayer.transform.childCount; ++i)
                if (!TurnManager.Instance.nonCombatPlayer.transform.GetChild(i).GetComponent<CharData>().dead)
                    allDead = false;

            if (allDead)
                SceneChangeManager.Instance.LoseCombat();
            else
                CombatTransitionManager.Instance.ResetCombatInstance(TurnManager.Instance.playerCharsList);
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
                {
                    SoundEffectManager.Instance.PlaySoundClip(SFX.CursorSelect, 0.15f);
                    PerformAction();
                }

                else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.RightControl))
                {
                    SoundEffectManager.Instance.PlaySoundClip(SFX.CursorExit, 0.25f);

                    for (int i = 0; i < moveTargets.Count; ++i)
                    {
                        moveTargets[i].GetComponent<CharData>().Targeted();
                        TurnManager.Instance.tracker.HighlightSelectedTrackers(moveTargets[i]);
                    }

                    settingTarget = false;
                    combatMenu.GetComponent<PlayerCombatMenuManager>().MakeButtonVisible(true);
                }

                else if (oneTarget && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S)))
                {
                    SoundEffectManager.Instance.PlaySoundClip(SFX.CursorMove, 0.25f);

                    if (!otherTarget && moveTargets[0].tag == "Player" && TurnManager.Instance.playerCharsList.Count > 1)
                        TargetDown(TurnManager.Instance.playerCharsList);

                    else if (!otherTarget && moveTargets[0].tag != "Player" && TurnManager.Instance.enemyCharsList.Count > 1)
                        TargetDown(TurnManager.Instance.enemyCharsList);
                }

                else if (oneTarget && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W)))
                {
                    SoundEffectManager.Instance.PlaySoundClip(SFX.CursorMove, 0.25f);

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

            if (otherTarget && l[num] == TurnManager.Instance.t1[0])
            {
                if (num > 0)
                    num -= 1;
                else if (num == 0)
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

            if (otherTarget && l[num] == TurnManager.Instance.t1[0])
            {
                if (num < l.Count - 1)
                    num += 1;
                else if (num == l.Count - 1)
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
            int num = moveTargets.Count;
            for (int i = 0; i < num; ++i)
                moveTargets.Remove(moveTargets[0]);

            switch (targetType)
            {
                // Target an enemy
                case (int)Targeting.OneEnemy:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        moveTargets.Add(TurnManager.Instance.enemyCharsList[0]);
                    else
                        EnemySetPlayerTarget();

                    otherTarget = false;
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

                    otherTarget = false;
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

                    otherTarget = false;
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

                    otherTarget = false;
                    oneTarget = true;
                    break;

                // Target an ally
                case (int)Targeting.OneAlly:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        moveTargets.Add(TurnManager.Instance.playerCharsList[0]);
                    else
                        moveTargets.Add(TurnManager.Instance.enemyCharsList[Random.Range(0, TurnManager.Instance.enemyCharsList.Count)]);

                    otherTarget = false;
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

                    otherTarget = false;
                    oneTarget = false;
                    break;

                // Target the user
                case (int)Targeting.OneDifferentAlly:
                    if (TurnManager.Instance.t1[0] != TurnManager.Instance.playerCharsList[0])
                        moveTargets.Add(TurnManager.Instance.playerCharsList[0]);
                    else
                        moveTargets.Add(TurnManager.Instance.playerCharsList[1]);

                    otherTarget = true;
                    oneTarget = false;
                    break;

                // Target the user
                case (int)Targeting.User:
                    moveTargets.Add(TurnManager.Instance.t1[0]);

                    otherTarget = false;
                    oneTarget = false;
                    break;
            }
        }

        private void SetTarget(GameObject newTarget)
        {
            int num = moveTargets.Count;
            for (int i = 0; i < num; ++i)
                moveTargets.Remove(moveTargets[0]);

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
                if (TurnManager.Instance.playerCharsList[i].GetComponent<Stats>().aggro > 1)
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

        IEnumerator DisplayAbilityNameAfterAnimation(string newString)
        {
            CharData c = TurnManager.Instance.t1[0].GetComponent<CharData>();

            c.DisplayActionAnimation();
            while (!c.hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            yield return null;
        }

        public void EndTurnImmediately()
        {

        }

        // Deals damage to the current target(s) based on a shared modifier
        public void DealDamage(GameObject animObj, float modifier)
        {
            StartCoroutine(DealDamageCoroutine(animObj, modifier));
        }

        IEnumerator DealDamageCoroutine(GameObject animObj, float modifier)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            int targ = newTarget + 1;
            storedMod = modifier;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = newTarget; i < targ; ++i)
            {
                int damage = DamageFormula(TurnManager.Instance.t1[0].GetComponent<Stats>(), moveTargets[i].GetComponent<Stats>(), modifier);

                //moveTargets[i].GetComponent<Stats>().ReduceHP(damage);
                //moveTargets[i].GetComponent<CharData>().ChangeHP();

                bool numEnd = true;
                if (!oneTarget && i != targ - 1)
                    numEnd = false;

                //dText.DamageNumbers(damage, true, moveTargets[i].transform.position, numEnd);

                animDrawer.GetComponent<AbilityEffectsGenerator>().CreateAnimation(animObj, moveTargets[i], damage, 0, numEnd, 0);
            }

            if (!oneTarget || newTarget == moveTargets.Count - 1)
                doneAttacking = true;
            else
            {
                anim = animObj;
                newTarget++;
            }

            yield return null;
        }

        // Deals damage to the current target(s) based on a shared modifier
        public void DealDamage(GameObject animObj, int damage, bool end)
        {
            StartCoroutine(DealDamageCoroutine(animObj, damage, end));
        }

        IEnumerator DealDamageCoroutine(GameObject animObj, int damage, bool end)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            int targ = newTarget + 1;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = newTarget; i < targ; ++i)
            {
                bool numEnd = false;
                if (end)
                {
                    numEnd = true;
                    if (!oneTarget && i != targ - 1)
                        numEnd = false;
                }

                animDrawer.GetComponent<AbilityEffectsGenerator>().CreateAnimation(animObj, moveTargets[i], damage, 0, numEnd, 0);
            }

            if (!oneTarget || newTarget == moveTargets.Count - 1)
                doneAttacking = true;
            else
            {
                anim = animObj;
                newTarget++;
            }

            yield return null;
        }

        public void InflictDamageOnTarget(int damage, GameObject target, bool end)
        {
            target.GetComponent<Stats>().ReduceHP(damage);
            target.GetComponent<CharData>().ChangeHP();
            dText.DamageNumbers(damage, true, target.transform.position, end);
        }

        // Deals damage to the current target(s) based on a shared modifier
        public void DealTPDamage(GameObject animObj, int damage, bool end)
        {
            StartCoroutine(DealTPDamageCoroutine(animObj, damage, end));
        }

        IEnumerator DealTPDamageCoroutine(GameObject animObj, int damage, bool end)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            int targ = newTarget + 1;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = newTarget; i < targ; ++i)
            {
                bool numEnd = false;
                if (end)
                {
                    numEnd = true;
                    if (!oneTarget && i != targ - 1)
                        numEnd = false;
                }
                
                animDrawer.GetComponent<AbilityEffectsGenerator>().CreateAnimation(animObj, moveTargets[i], 0, damage, numEnd, 1);
            }

            if (!oneTarget || newTarget == moveTargets.Count - 1)
                doneAttacking = true;
            else
            {
                anim = animObj;
                newTarget++;
            }

            yield return null;
        }

        public void InflictTPDamageOnTarget(int damage, GameObject target, bool end)
        {
            target.GetComponent<Stats>().ReduceTP(damage);
            target.GetComponent<CharData>().ChangeTP();
            dText.DamageNumbers(damage, false, target.transform.position, end);
        }

        // Deals damage to the current target(s) based on a shared modifier
        public void DealHybridDamage(GameObject animObj, GameObject target, int hpDamage, int tpDamage, bool end)
        {
            StartCoroutine(DealHybridDamageCoroutine(animObj, target, hpDamage, tpDamage, end));
        }

        IEnumerator DealHybridDamageCoroutine(GameObject animObj, GameObject target, int hpDamage, int tpDamage, bool end)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            if (target == null)
            {
                int targ = newTarget + 1;
                if (!oneTarget)
                    targ = moveTargets.Count;

                for (int i = newTarget; i < targ; ++i)
                {
                    bool numEnd = false;
                    if (end)
                    {
                        numEnd = true;
                        if (!oneTarget && i != targ - 1)
                            numEnd = false;
                    }

                    animDrawer.GetComponent<AbilityEffectsGenerator>().CreateAnimation(animObj, moveTargets[i], hpDamage, tpDamage, numEnd, 2);
                }
            }
            else
                animDrawer.GetComponent<AbilityEffectsGenerator>().CreateAnimation(animObj, target, hpDamage, tpDamage, end, 2);

            if (!oneTarget || newTarget == moveTargets.Count - 1)
                doneAttacking = true;
            else
            {
                anim = animObj;
                newTarget++;
            }

            yield return null;
        }

        public void InflictHybridDamageOnTarget(int damage, int tpDamage, GameObject target, bool end)
        {
            target.GetComponent<Stats>().ReduceHP(damage);
            target.GetComponent<CharData>().ChangeHP();
            dText.DamageNumbers(damage, true, target.transform.position + new Vector3(-0.2f, 0.33f, 0), false);

            target.GetComponent<Stats>().ReduceTP(tpDamage);
            target.GetComponent<CharData>().ChangeTP();
            dText.DamageNumbers(tpDamage, false, target.transform.position + new Vector3(0.2f, -0.33f, 0), end);
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

                dText.DamageNumbers(damage, true, moveTargets[i].transform.position, false);
            }
            
            StartCoroutine(EndNonDamageTextDisplay(textDuration, false));
        }

        // Deals damage to the current target(s) based on a shared modifier
        public int DealDamageWithAbsorb(GameObject animObj, float modifier, bool hp, Vector3 offset)
        {
            int damage = 0;
            int targ = 1;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = 0; i < targ; ++i)
            {
                damage = DamageFormula(TurnManager.Instance.t1[0].GetComponent<Stats>(), moveTargets[i].GetComponent<Stats>(), modifier);
            }

            StartCoroutine(DealDamageWithAbsorbCoroutine(animObj, modifier, damage, hp, offset));
            return damage;
        }

        IEnumerator DealDamageWithAbsorbCoroutine(GameObject animObj, float modifier, int damage, bool hp, Vector3 offset)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);
            
            int targ = 1;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = 0; i < targ; ++i)
            {
                if (hp)
                {
                    //moveTargets[i].GetComponent<Stats>().ReduceHP(damage);
                    //moveTargets[i].GetComponent<CharData>().ChangeHP();

                    DealDamage(animObj, damage, false);
                }
                else
                {
                    //moveTargets[i].GetComponent<Stats>().ReduceTP(damage);
                    //moveTargets[i].GetComponent<CharData>().ChangeTP();

                    
                }

                //dText.DamageNumbers(damage, true, moveTargets[i].transform.position + offset, false);
            }

            yield return null;
        }

        // Deals damage to the current target(s) based on a shared modifier
        public int DamageUserAsAbsorb(GameObject animObj, int num, bool hp, Vector3 offset)
        {
            int damage = num;
            StartCoroutine(DamageUserAsAbsorbCoroutine(animObj, num, hp, offset));
            return damage;
        }

        IEnumerator DamageUserAsAbsorbCoroutine(GameObject animObj, int damage, bool hp, Vector3 offset)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            if (hp)
            {
                //TurnManager.Instance.t1[0].GetComponent<Stats>().ReduceHP(damage);
                //TurnManager.Instance.t1[0].GetComponent<CharData>().ChangeHP();

                DealDamage(animObj, damage, false);
            }
            else
            {
                TurnManager.Instance.t1[0].GetComponent<Stats>().ReduceTP(damage);
                TurnManager.Instance.t1[0].GetComponent<CharData>().ChangeTP();
            }

            //dText.DamageNumbers(damage, true, TurnManager.Instance.t1[0].transform.position + offset, false);

            yield return null;
        }

        // Deals damage to the current target(s) based on a shared modifier
        public Vector2Int DamageUserAsAbsorb(GameObject animObj, int hpNum, int tpNum)
        {
            StartCoroutine(DamageUserAsAbsorbCoroutine(animObj, hpNum, tpNum));
            return new Vector2Int(hpNum, tpNum);
        }

        IEnumerator DamageUserAsAbsorbCoroutine(GameObject animObj, int hpDamage, int tpDamage)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            DealHybridDamage(animObj, TurnManager.Instance.t1[0], hpDamage, tpDamage, false);
            yield return null;
        }

        // Restores health to all specified targets
        public void RestoreHealth(GameObject animObj, int amount, bool done, Vector3 offset)
        {
            StartCoroutine(RestoreHealthCoroutine(animObj, amount, done, offset));
        }

        IEnumerator RestoreHealthCoroutine(GameObject animObj, int amount, bool done, Vector3 offset)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            int targ = 1;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = 0; i < targ; ++i)
            {
                //moveTargets[i].GetComponent<Stats>().ReduceHP(-amount);
                //moveTargets[i].GetComponent<CharData>().ChangeHP();

                bool canEnd = true;
                if (!oneTarget && i != targ - 1)
                    canEnd = false;

                //dText.DamageNumbers(-amount, true, moveTargets[i].transform.position + offset, canEnd);

                DealDamage(animObj, -amount, canEnd);
            }

            doneAttacking = true;
            yield return null;
        }

        // Restores health to all specified targets
        public void RestoreTechPoints(GameObject animObj, int amount, Vector3 offset)
        {
            StartCoroutine(RestoreTechPointsCoroutine(animObj, amount, offset));
        }

        IEnumerator RestoreTechPointsCoroutine(GameObject animObj, int amount, Vector3 offset)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            int targ = 1;
            bool canEnd = false;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = 0; i < targ; ++i)
            {
                //moveTargets[i].GetComponent<Stats>().ReduceTP(-amount);
                //moveTargets[i].GetComponent<CharData>().ChangeTP();

                if (i == targ - 1)
                    canEnd = true;

                DealTPDamage(animObj, -amount, canEnd);
                //dText.DamageNumbers(-amount, false, moveTargets[i].transform.position + offset, canEnd);
            }

            doneAttacking = true;
            yield return null;
        }

        // Restores health to all specified targets
        public void RestoreHybrid(GameObject animObj, int hpNum, int tpNum)
        {
            StartCoroutine(RestoreHybridCoroutine(animObj, hpNum, tpNum));
        }

        IEnumerator RestoreHybridCoroutine(GameObject animObj, int hpNum, int tpNum)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            int targ = 1;
            bool canEnd = false;
            if (!oneTarget)
                targ = moveTargets.Count;

            for (int i = 0; i < targ; ++i)
            {
                if (i == targ - 1)
                    canEnd = true;

                DealHybridDamage(animObj, null, -hpNum, -tpNum, canEnd);
            }

            doneAttacking = true;
            yield return null;
        }

        // Performs an ability with a non-damaging effect
        public void UseStatusAbility(GameObject animObj, List<int> type, List<float> mod, List<int> length, bool endTurn, float textDuration)
        {
            StartCoroutine(UseStatusAbilityCoroutine(animObj, type, mod, length, endTurn, textDuration));
        }

        IEnumerator UseStatusAbilityCoroutine(GameObject animObj, List<int> type, List<float> mod, List<int> length, bool endTurn, float textDuration)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            int targ = 1;
            if (!oneTarget)
                targ = moveTargets.Count;
            
            for (int i = 0; i < targ; ++i)
            {
                //if (type == 0)
                //    moveTargets[i].GetComponent<Stats>().SetAtkMod(mod, additive);
                //if (type == 1)
                //    moveTargets[i].GetComponent<Stats>().SetDefMod(mod, additive);
                //if (type == 2)
                //    moveTargets[i].GetComponent<Stats>().SetSpdMod(mod, additive);
                //if (type == 3)
                //    moveTargets[i].GetComponent<Stats>().SetAggro(mod, additive);
                //if (type == 4)
                //{
                //    moveTargets[i].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.Defend);
                //    moveTargets[i].GetComponent<Stats>().defending = true;
                //}
                //
                //moveTargets[i].GetComponent<CharData>().ChangeCharUIBuffDisplay();
                
                animDrawer.GetComponent<AbilityEffectsGenerator>().CreateAnimation(animObj, moveTargets[i], type, mod, length, endTurn, textDuration);
            }

            yield return null;
        }

        public void InflictStatusOnTarget(GameObject target, List<int> type, List<float> mod, List<int> additive, bool endTurn, float textDuration)
        {
            for (int i = 0; i < type.Count; ++i)
            {
                if (target == TurnManager.Instance.t1[0])
                    additive[i]++;

                if (type[i] == 0)
                    target.GetComponent<Stats>().SetAtkMod(mod[i], additive[i]);
                if (type[i] == 1)
                    target.GetComponent<Stats>().SetDefMod(mod[i], additive[i]);
                if (type[i] == 2)
                    target.GetComponent<Stats>().SetSpdMod(mod[i], additive[i]);
                if (type[i] == 3)
                    target.GetComponent<Stats>().SetAggro(mod[i], additive[i]);
                if (type[i] == 4)
                {
                    target.GetComponent<CharAnimator>().PlayAnimations(AnimationClips.Defend);
                    target.GetComponent<Stats>().defending = true;
                }
            }
            
            target.GetComponent<CharData>().ChangeCharUIBuffDisplay();
            if (endTurn)
                StartCoroutine(EndNonDamageTextDisplay(textDuration, endTurn));
        }

        // Prepares to let the user perform a delayed attack
        public void UseDelayedAbility(GameObject animObj, Targeting targeting, string name, float mod, int delay, bool endTurn, float textDuration, float particleDelay)
        {
            StartCoroutine(UseDelayedAbilityCoroutine(animObj, targeting, name, mod, delay, endTurn, textDuration, particleDelay));
        }

        IEnumerator UseDelayedAbilityCoroutine(GameObject animObj, Targeting targeting, string name, float mod, int delay, bool endTurn, float textDuration, float particleDelay)
        {
            while (!TurnManager.Instance.t1[0].GetComponent<CharData>().hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            TurnManager.Instance.t1[0].GetComponent<CharData>().SetDelayedAttack(animObj, targeting, name, delay, mod, moveTargets);

            if (endTurn)
                StartCoroutine(EndNonDamageTextDisplay(textDuration, endTurn));

            yield return new WaitForSeconds(particleDelay);
            TurnManager.Instance.t1[0].GetComponent<CharData>().CreateParticles();

            yield return null;
        }

        private void PerformDelayedAbility(CharData c)
        {
            c.delayedAttack = false;
            DisplayAbilityName(c.delayedAbilityName);
            c.DisplayActionAnimation();

            StartCoroutine(PerformDelayedAbilityCoroutine(c));
        }

        IEnumerator PerformDelayedAbilityCoroutine(CharData c)
        {
            while (!c.hasFinishedActionAnimation)
                yield return new WaitForSeconds(0.02f);

            int num = moveTargets.Count;
            for (int i = 0; i < num; ++i)
                moveTargets.Remove(moveTargets[0]);

            if (TurnManager.Instance.t1[0].tag == "Player")
            {
                for (int i = 0; i < c.target.Count; ++i)
                {
                    if (c.target[i].GetComponent<CharData>().dead && c.target.Count == 1)
                        c.target[i] = TurnManager.Instance.enemyCharsList[0];

                    moveTargets.Add(c.target[i]);
                }
            }

            // Resets enemy targets, just in case new players join the encounter
            else
            {
                SetTarget((int)c.delayedTargeting);
                num = c.target.Count;
                for (int i = 0; i < num; ++i)
                    c.target.Remove(c.target[0]);

                for (int i = 0; i < moveTargets.Count; ++i)
                    c.target.Add(moveTargets[i]);
            }
            
            if (c.target.Count > 1)
                oneTarget = false;
            DealDamage(c.animationObject, c.storedModifier);

            yield return new WaitForSeconds(c.animationObject.GetComponent<Animation>().clip.length);
            TurnManager.Instance.t1[0].GetComponent<CharData>().DestroyParticles();

            yield return null;
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

            if (t.defending)
                i /= 2;
            return (int)i;
        }

        public void ChangeVisibleMenuButtons(bool change)
        {
            combatMenu.GetComponent<PlayerCombatMenuManager>().ShowAbilities(change, TurnManager.Instance.t1[0]);
        }

        IEnumerator MovePlayerCharacter(Vector3 targetPos)
        {
            CharData c = TurnManager.Instance.t1[0].GetComponent<CharData>();

            c.MoveCharUI(true);

            TurnManager.Instance.t1[0].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.Move);
            while (TurnManager.Instance.t1[0].transform.position != targetPos)
            {
                TurnManager.Instance.t1[0].transform.position = Vector3.MoveTowards(TurnManager.Instance.t1[0].transform.position, targetPos, 0.1f);
                yield return new WaitForSeconds(0.0125f);
            }

            // TEMP - Make this combat idle when I have the animations available!
            TurnManager.Instance.t1[0].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.CombatIdle);

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

        IEnumerator MovePlayerCharacterAtStart(Vector3 targetPos, int index, bool end)
        {
            TurnManager.Instance.playerCharsList[index].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.Move);

            while (TurnManager.Instance.playerCharsList[index].transform.position != targetPos)
            {
                TurnManager.Instance.playerCharsList[index].transform.position = Vector3.MoveTowards(TurnManager.Instance.playerCharsList[index].transform.position, targetPos, 0.1f);
                yield return new WaitForSeconds(0.0125f);
            }

            // TEMP - Make this combat idle when I have the animations available!
            TurnManager.Instance.playerCharsList[index].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.CombatIdle);

            yield return new WaitForSeconds(0.1f);
            if (end)
                StartRound();
        }
    }
}