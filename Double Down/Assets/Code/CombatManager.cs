using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CombatManager : Singleton<CombatManager>
    {
        public GameObject combatMenu;
        public Transform moveTarget;
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
                Act();
            }
        }

        // Performs an action
        public void Act()
        {
            endingTurn = true;
            if (TurnManager.Instance.t1[0].tag == "Player")
            {
                combatMenu.GetComponent<PlayerCombatMenuManager>().MakeButtonVisible(false);
                StartCoroutine(MovePlayerCharacter(origPos));
            }

            else
            {
                TurnManager.Instance.EndRound();
            }
        }

        private void DealDamage()
        {
            //target.ReduceHP(DamageFormula(chara, target, 1));
            //Debug.Log(target.HP());
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