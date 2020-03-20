using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class TurnManager : Singleton<TurnManager>
    {
        private bool inCombat = false;

        public GameObject playerChars;
        public GameObject nonCombatPlayer;
        public GameObject enemyChars;
        public GameObject nonCombatEnemies;
        [HideInInspector] public GameObject firstT1Char = null;
        public TurnTracker tracker;
        [HideInInspector] public List<GameObject> playerCharsList = null;
        [HideInInspector] public List<GameObject> enemyCharsList = null;
        [HideInInspector] public List<GameObject> combatChars = null;
        //[HideInInspector]
        public List<GameObject> t1 = null;
        //[HideInInspector]
        public List<GameObject> t2 = null;

        public void Init()
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

            SetTurnOrder(0);
        }

        private void RemoveTurns()
        {
            for (int i = 0; i < t1.Count; ++i)
            {
                if (t1[i].GetComponent<CharData>().dead)
                    t1.Remove(t1[i]);
            }

            for (int i = 0; i < t2.Count; ++i)
            {
                if (t2[i].GetComponent<CharData>().dead)
                    t2.Remove(t2[i]);
            }
        }

        private void RemoveEnemyTurns()
        {
            for (int i = 0; i < t1.Count; ++i)
            {
                if (t1[i].tag != "Player")
                    t1.Remove(t1[i]);
            }

            for (int i = 0; i < t2.Count; ++i)
            {
                if (t2[i].tag != "Player")
                    t2.Remove(t2[i]);
            }
        }

        // Resets the turn order to account for dead enemies
        public void ResetTurns()
        {
            RemoveTurns();
            tracker.ResetTrackers();
        }

        // Resets the turn order to account for new enemies
        public void PrepCombatTurns()
        {
            RemoveTurns();
            tracker.DestroyNonCombatTrackers(false);

            FillCombatTurns();
            SetTurnOrder(1);
        }

        // Fills the combat list with enemies
        // In the future, ALSO ADD THE NEW PLAYER CHAR, IF APPLICABLE
        public void FillCombatTurns()
        {
            GameObject player = firstT1Char;

            // Clears all lists to ensure everything is refilled properly
            playerCharsList.Clear();
            enemyCharsList.Clear();
            combatChars.Clear();
            t1.Clear();
            t2.Clear();

            // Refills lists with relevant information
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

            t1.Add(firstT1Char);
            for (int i = 0; i < combatChars.Count; ++i)
            {
                if (!combatChars[i].GetComponent<CharData>().hasActed && combatChars[i] != firstT1Char)
                    t1.Add(combatChars[i]);
                t2.Add(combatChars[i]);
            }

            StartCoroutine(SortBetweenScenes(player));
            //tracker.AddTurnTrackers(t1, t2, player);
        }

        // Resets the turn order to account for new enemies
        public void PrepNonCombatTurns()
        {
            RemoveTurns();
            tracker.DestroyNonPlayerCombatTrackers(false);

            FillNonCombatTurns();
            SetTurnOrder(1);
        }
        // Resets the turn order upon returning to the hub
        public void FillNonCombatTurns()
        {
            GameObject player = firstT1Char;

            // Clears all lists to ensure everything is refilled properly
            playerCharsList.Clear();
            enemyCharsList.Clear();
            combatChars.Clear();
            t1.Clear();
            t2.Clear();

            // Refills lists with relevant information
            for (int i = 0; i < playerChars.transform.childCount; ++i)
            {
                playerCharsList.Add(playerChars.transform.GetChild(i).gameObject);
                combatChars.Add(playerChars.transform.GetChild(i).gameObject);
            }

            t1.Add(firstT1Char);
            for (int i = 0; i < combatChars.Count; ++i)
            {
                if (!combatChars[i].GetComponent<CharData>().hasActed && combatChars[i] != firstT1Char)
                    t1.Add(combatChars[i]);
                t2.Add(combatChars[i]);
            }

            StartCoroutine(SortBetweenScenes(player));
            //tracker.AddTurnTrackers(t1, t2, player);
        }

        public void EnterSceneResetTurns()
        {
            // Reset turns completely
            playerCharsList.Clear();
            combatChars.Clear();
            t1.Clear();
            t2.Clear();

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

            SetTurnOrder(1);
        }

        public void StartGlobalTurn()
        {
            ResetCharActions();
            tracker.SetUpTrackers(t2, 2, false);
        }

        private void ResetCharActions()
        {
            for (int i = 0; i < playerChars.transform.childCount; ++i)
                playerChars.transform.GetChild(i).GetComponent<CharData>().hasActed = false;
            for (int i = 0; i < nonCombatPlayer.transform.childCount; ++i)
                nonCombatPlayer.transform.GetChild(i).GetComponent<CharData>().hasActed = false;
            for (int i = 0; i < enemyChars.transform.childCount; ++i)
                enemyChars.transform.GetChild(i).GetComponent<CharData>().hasActed = false;
            for (int i = 0; i < nonCombatEnemies.transform.childCount; ++i)
                nonCombatEnemies.transform.GetChild(i).GetComponent<CharData>().hasActed = false;
        }

        // Starts a turn of gameplay
        public void StartRound()
        {
            SetTurnOrder(1);
            tracker.MoveTracker(0);
        }

        public void CheckForRoundType()
        {
            // If either player is engaged in combat, start them in it
            //  CHANGE TO ONLY AFFECT RELEVANT CHARACTER IN THE FUTURE
            firstT1Char = t1[0];
            if (t1[0].tag == "Player" && t1[0].GetComponent<CharData>().dead)
                t1[0].GetComponent<CharData>().CountDownDeathTurns();

            if (!t1[0].GetComponent<CharData>().dead)
            {
                if (t1[0].GetComponent<CharData>().isInCombat && CombatManager.Instance != null)
                    CombatManager.Instance.StartRound();
                else
                    MovementManager.Instance.StartRound();
            }
            // If a character is dead, end the turn
            else
                EndRound();
        }

        // Ends the current round of combat
        public void EndRound()
        {
            t1[0].GetComponent<CharData>().hasActed = true;
            t1[0].GetComponent<CharData>().t1Pos--;

            //Debug.Log(combatChars.Count);
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
            // CHECK IF ALL CHARACTERS ARE CURRENTLY IN THE SAME BATTLE; 
            //  IF SO, IMMEDIATELY END THE GLOBAL TURN AND CONTINUE COMBAT
            //  IF NOT, END THE COMBAT INSTANCE
            
            if (nonCombatPlayer.transform.childCount == 0)
                SetNextGlobalTurnData();
            else
                CombatTransitionManager.Instance.ExitExistingCombatInstance(playerCharsList);
        }

        public void SetNextGlobalTurnData()
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
        public void SetTurnOrder(int initVal)
        {
            StartCoroutine(WaitForTurnSetup(initVal));
        }

        IEnumerator WaitForTurnSetup(int initVal)
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

            if (initVal == 0)
            {
                tracker.SetUpTrackers(t1, 1, true);
                tracker.SetUpTrackers(t2, 2, false);
            }
            else if (initVal == 1)
            {
                yield return new WaitForSeconds(0.2f);
                tracker.ReorderTrackers(false);
            }
            else if (initVal == 2)
            {
                yield return new WaitForSeconds(0.2f);
                tracker.ReorderTrackers(true);
            }

            yield return null;
        }

        IEnumerator SortBetweenScenes(GameObject player)
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

            yield return new WaitForSeconds(0.2f);
            tracker.AddTurnTrackers(t1, t2, player);

            yield return null;
        }
    }
}