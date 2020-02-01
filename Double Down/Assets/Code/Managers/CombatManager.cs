using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Targeting
{
    One_Enemy = 0,
    All_Enemies = 1,
    One_Ally = 2,
    All_Allies = 3,
    User = 4
}

namespace Managers
{
    public class CombatManager : Singleton<CombatManager>
    {
        public GameObject combatMenu;
        public Transform moveTarget;
        public DamageTextUI dText = null;

        private List<GameObject> moveTargets = new List<GameObject>();

        private Vector3 origPos;
        private bool endingTurn = false;

        // Starts each character's round of combat
        public void StartRound()
        {
            endingTurn = false;
            if (TurnManager.Instance.t1[0].tag == "Player")
            {
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
            if (TurnManager.Instance.t1[0].tag == "Player")
            {
                combatMenu.GetComponent<PlayerCombatMenuManager>().MakeButtonVisible(false);
                SetTarget(0);
                TurnManager.Instance.t1[0].GetComponent<PlayerActions>().CheckAction(action);
            }

            else
            {
                for (int i = 0; i < moveTargets.Count; ++i)
                    moveTargets.Remove(moveTargets[i]);
                moveTargets.Add(TurnManager.Instance.playerCharsList[Random.Range(0, TurnManager.Instance.playerCharsList.Count)]);

                DealDamage(1.0f);
            }
        }

        public void FollowUpAction()
        {
            for (int i = 0; i < moveTargets.Count; ++i)
                if (moveTargets[i].GetComponent<Stats>().HP() <= 0)
                {
                    if (moveTargets[i].tag == "Player")
                        TurnManager.Instance.playerCharsList.Remove(moveTargets[i]);
                    else
                        TurnManager.Instance.enemyCharsList.Remove(moveTargets[i]);

                    TurnManager.Instance.combatChars.Remove(moveTargets[i]);
                    Destroy(moveTargets[i]);
                }

            for (int i = 0; i < moveTargets.Count; ++i)
                moveTargets.Remove(moveTargets[i]);

            endingTurn = true;
            if (TurnManager.Instance.t1[0].tag == "Player")
            {
                StartCoroutine(MovePlayerCharacter(origPos));
            }

            else
            {
                TurnManager.Instance.EndRound();
            }
        }

        public void SetTarget(int targetType)
        {
            for (int i = 0; i < moveTargets.Count; ++i)
                moveTargets.Remove(moveTargets[i]);

            switch (targetType)
            {
                case (int)Targeting.One_Enemy:
                    moveTargets.Add(TurnManager.Instance.enemyCharsList[0]);
                    break;
                case (int)Targeting.All_Enemies:
                    // target all enemies
                    break;
                case (int)Targeting.One_Ally:
                    // target an ally
                    break;
                case (int)Targeting.All_Allies:
                    // target all allies
                    break;
                case (int)Targeting.User:
                    // target the user
                    break;
            }
        }

        private void AddEnemyTargets(Targeting t)
        {
            if (t == Targeting.One_Enemy)
                moveTargets.Add(TurnManager.Instance.enemyCharsList[0]);
            else if (t == Targeting.All_Enemies)
                for (int i = 0; i < TurnManager.Instance.enemyCharsList.Count; ++i)
                    moveTargets.Add(TurnManager.Instance.enemyCharsList[i]);
        }

        // Deals damage to the current target(s) based on a shared modifier
        public void DealDamage(float modifier)
        {
            for (int i = 0; i < moveTargets.Count; ++i)
            {
                int damage = DamageFormula(TurnManager.Instance.t1[0].GetComponent<Stats>(), moveTargets[i].GetComponent<Stats>(), modifier);

                moveTargets[i].GetComponent<Stats>().ReduceHP(damage);
                moveTargets[i].GetComponent<CharData>().ChangeHP();

                dText.DamageNumbers(damage, moveTargets[i].transform);
            }
        }

        private int DamageFormula(Stats a, Stats t, float mod)
        {
            int i = (int)(((a.Attack() * a.Attack()) / t.Defense()) * mod * Random.Range(0.8f, 1.0f));
            return i;
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