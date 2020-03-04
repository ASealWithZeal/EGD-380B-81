using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class TurnManager : Singleton<TurnManager>
    {
        private bool inCombat = false;

        public GameObject playerChars;
        public GameObject enemyChars;
        public TurnTracker tracker;
        [HideInInspector] public List<GameObject> playerCharsList = null;
        [HideInInspector] public List<GameObject> enemyCharsList = null;
        [HideInInspector] public List<GameObject> combatChars = null;
        [HideInInspector] public List<GameObject> t1 = null;
        [HideInInspector] public List<GameObject> t2 = null;

        private void Start()
        {
            for (int i = 0; i < playerChars.transform.childCount; ++i)
            {
                playerCharsList.Add(playerChars.transform.GetChild(i).gameObject);
                combatChars.Add(playerChars.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < enemyChars.transform.childCount; ++i)
            {
                enemyCharsList.Add(enemyChars.transform.GetChild(i).gameObject);
                combatChars.Add(enemyChars.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < combatChars.Count; ++i)
            {
                t1.Add(combatChars[i]);
                t2.Add(combatChars[i]);
            }

            SetTurnOrder(true);
        }

        // Resets the turn order to account for dead enemies
        public void ResetTurns()
        {
            for (int i = 0; i < t1.Count; ++i)
            {
                if (t1[i] == null)
                    t1.Remove(t1[i]);
            }

            for (int i = 0; i < t2.Count; ++i)
            {
                if (t2[i] == null)
                    t2.Remove(t2[i]);
            }

            tracker.ResetTrackers();
        }

        public void StartGlobalTurn()
        {
            for (int i = 0; i < combatChars.Count; ++i)
                combatChars[i].GetComponent<CharData>().hasActed = false;
            tracker.SetUpTrackers(t2, 2, false);
        }

        // Starts a turn of gameplay
        public void StartRound()
        {
            SetTurnOrder(false);
            t1[0].GetComponent<CharData>().hasActed = true;
            tracker.MoveTracker(0);
        }

        public void CheckForRoundType()
        {
            // If either player is engaged in combat, start them in it
            //  CHANGE TO ONLY AFFECT RELEVANT CHARACTER IN THE FUTURE
            if (inCombat)
                CombatManager.Instance.StartRound();
            else
                MovementManager.Instance.StartRound();

            //else
            //    EndRound();
        }

        // Ends the current round of combat
        public void EndRound()
        {
            t1[0].GetComponent<CharData>().t1Pos--;
            if (combatChars.Count != t2.Count)
                ResetTurns();

            else
            {
                t1.Remove(t1[0]);

                if (t1.Count > 0)
                    StartRound();

                else
                    EndGlobalTurn();
            }
        }

        // Ends the overall global turn of combat
        public void EndGlobalTurn()
        {
            for (int i = 0; i < t2.Count; ++i)
                t1.Add(t2[i]);

            t2.Clear();

            for (int i = 0; i < combatChars.Count; ++i)
                t2.Add(combatChars[i]);

            tracker.MoveTracker(1);
        }

        // Sorts the character order for each turn
        // Can be used to re-sort the order mid-turn
        private void SetTurnOrder(bool init)
        {
            StartCoroutine(WaitForTurnSetup(init));
        }

        IEnumerator WaitForTurnSetup(bool init)
        {
            bool sorted = false;
            bool sorting = true;
            GameObject temp = null;

            yield return new WaitForSeconds(0.1f);

            while (sorting)
            {
                for (int i = 0; i < combatChars.Count; ++i)
                {
                    if (t1.Count > i && t1.Count > 0 && i != 0 && t1[i].GetComponent<Stats>().Speed() > t1[i - 1].GetComponent<Stats>().Speed() && !t1[i - 1].GetComponent<CharData>().hasActed)
                    {
                        sorted = false;

                        temp = t1[i];
                        t1[i] = t1[i - 1];
                        t1[i - 1] = temp;
                    }

                    if (t2.Count > 0 && i != 0 && t2[i].GetComponent<Stats>().NextSpeed(t2[i].GetComponent<CharData>().hasActed) > t2[i - 1].GetComponent<Stats>().NextSpeed(t2[i - 1].GetComponent<CharData>().hasActed))
                    {
                        sorted = false;

                        temp = t2[i];
                        t2[i] = t2[i - 1];
                        t2[i - 1] = temp;
                    }
                }

                if (!sorted)
                    sorted = true;

                else
                    sorting = false;
            }

            for (int i = 0; i < t1.Count; ++i)
            {
                t1[i].GetComponent<CharData>().t1Pos = i;
            }

            for (int i = 0; i < t2.Count; ++i)
            {
                t2[i].GetComponent<CharData>().t2Pos = i;
            }

            if (init)
            {
                tracker.SetUpTrackers(t1, 1, true);
                tracker.SetUpTrackers(t2, 2, false);
            }
            else
                tracker.ReorderTrackers();

            yield return null;
        }
    }
}