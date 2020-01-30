using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                FollowUpAction();
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
                //
            }
        }

        private void FollowUpAction()
        {
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
            switch (targetType)
            {
                case 0:
                    moveTargets.Add(TurnManager.Instance.enemyCharsList[0]);
                    break;
                case 1:
                    // target all enemies
                    break;
                case 2:
                    // target an ally
                    break;
                case 3:
                    // target all allies
                    break;
            }
        }

        // Deals damage to the current target(s) based on a shared modifier
        public void DealDamage(float modifier)
        {
            for (int i = 0; i < moveTargets.Count; ++i)
            {
                int damage = DamageFormula(TurnManager.Instance.t1[0].GetComponent<Stats>(), moveTargets[i].GetComponent<Stats>(), modifier);

                moveTargets[i].GetComponent<Stats>().ReduceHP(damage);
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