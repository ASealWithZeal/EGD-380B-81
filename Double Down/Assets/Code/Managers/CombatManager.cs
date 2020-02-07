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

        private List<GameObject> moveTargets = new List<GameObject>();

        public int DAMAGE_MULT = 10;
        private bool oneTarget = false;
        private bool doneAttacking = false;
        private int newTarget = 0;
        private float storedMod = 0.0f;

        private Vector3 origPos;
        private bool endingTurn = false;

        // Starts each character's round of combat
        public void StartRound()
        {
            endingTurn = false;
            if (TurnManager.Instance.t1[0].tag == "Player")
            {
                // Ticks down the characters modifier changes
                TurnManager.Instance.t1[0].GetComponent<Stats>().TickModifierChanges();
                origPos = TurnManager.Instance.t1[0].transform.position;
                StartCoroutine(MovePlayerCharacter(moveTarget.position));
            }

            else
            {
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
                //for (int i = 0; i < moveTargets.Count; ++i)
                //    moveTargets.Remove(moveTargets[i]);
                //moveTargets.Add(TurnManager.Instance.playerCharsList[Random.Range(0, TurnManager.Instance.playerCharsList.Count)]);
                TurnManager.Instance.t1[0].GetComponent<EnemyActions>().PerformAction();
            }
        }

        public void FollowUpAction()
        {
            if (doneAttacking)
            {
                aDisplay.ChangeDisplayOpacity(false);

                endingTurn = true;
                bool canMoveOn = true;
                for (int i = 0; i < moveTargets.Count; ++i)
                    if (moveTargets[i].GetComponent<Stats>().HP() <= 0)
                    {
                        if (moveTargets[i].tag == "Player")
                            TurnManager.Instance.playerCharsList.Remove(moveTargets[i]);
                        else
                            TurnManager.Instance.enemyCharsList.Remove(moveTargets[i]);

                        TurnManager.Instance.combatChars.Remove(moveTargets[i]);
                        Destroy(moveTargets[i]);

                        //endingTurn = false;
                    }

                if (TurnManager.Instance.enemyCharsList.Count == 0)
                {
                    canMoveOn = false;
                    for (int i = 0; i < TurnManager.Instance.playerCharsList.Count; ++i)
                        TurnManager.Instance.playerCharsList[i].GetComponent<Stats>().exp += 50;
                    winCanvas.ShowWinCanvas();
                }
                else if (TurnManager.Instance.playerCharsList.Count == 0)
                {
                    canMoveOn = false;
                    SceneChangeManager.Instance.ChangeScene("LoseScene");
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
                        moveTargets.Add(TurnManager.Instance.playerCharsList[Random.Range(0, TurnManager.Instance.playerCharsList.Count)]);

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
                            moveTargets.Add(TurnManager.Instance.playerCharsList[Random.Range(0, TurnManager.Instance.playerCharsList.Count)]);

                    oneTarget = true;
                    break;

                // Target three random enemies
                case (int)Targeting.ThreeRandomEnemies:
                    if (TurnManager.Instance.t1[0].tag == "Player")
                        for (int i = 0; i < 3; ++i)
                            moveTargets.Add(TurnManager.Instance.enemyCharsList[Random.Range(0, TurnManager.Instance.enemyCharsList.Count)]);
                    else
                        for (int i = 0; i < 3; ++i)
                            moveTargets.Add(TurnManager.Instance.playerCharsList[Random.Range(0, TurnManager.Instance.playerCharsList.Count)]);

                    oneTarget = true;
                    break;

                // Target an ally
                case (int)Targeting.OneAlly:
                    break;

                // Target all allies
                case (int)Targeting.AllAllies:
                    break;

                // Target the user
                case (int)Targeting.User:
                    break;
            }
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

        private int DamageFormula(Stats a, Stats t, float mod)
        {
            //int i = (int)(((a.Attack() * a.Attack()) / t.Defense()) * mod * Random.Range(0.8f, 1.0f));
            float i = (int)((a.Attack() * (a.Attack() / 2)) * (1 + (1 - (0.3f * a.level))) * DAMAGE_MULT) / t.Defense();
            i *= mod;
            i *= Random.Range(0.8f, 1.0f);
            return (int)i;
        }

        IEnumerator MovePlayerCharacter(Vector3 targetPos)
        {
            while (TurnManager.Instance.t1[0].transform.position != targetPos)
            {
                TurnManager.Instance.t1[0].transform.position = Vector3.MoveTowards(TurnManager.Instance.t1[0].transform.position, targetPos, 0.125f);
                yield return new WaitForSeconds(0.01f);
            }

            // IN FUTURE CHANGE
            if (endingTurn)
            {
                endingTurn = false;
                TurnManager.Instance.EndRound();
            }

            else
            {
                combatMenu.GetComponent<PlayerCombatMenuManager>().MakeButtonVisible(true);
            }

            yield return null;
        }
    }
}