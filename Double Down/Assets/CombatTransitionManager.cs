using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CombatTransitionManager : Singleton<CombatTransitionManager>
    {
        public Transform playerChars;
        public Transform nonCombatPlayerChars;
        public Transform enemyChars;
        public Transform nonCombatEnemyChars;

        private int combatInsts = -1;
        public ScreenTransitionCanvas transitionUI;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CreateNewCombatInstance(GameObject player, List<GameObject> enemies)
        {
            // Increments the number of combat instances appropriately
            combatInsts++;

            // Temporarily removes all player characters from the combat list
            int temp = playerChars.childCount;
            for (int i = 0; i < temp; ++i)
                playerChars.GetChild(0).parent = nonCombatPlayerChars;

            // Adds the player characters to the combat list
            player.transform.parent = playerChars;
            player.GetComponent<CharData>().isInCombat = true;
            player.GetComponent<CharData>().combatInst = combatInsts;

            // Adds the enemies to the combat list
            for (int i = 0; i < enemies.Count; ++i)
            {
                enemies[i].transform.parent = enemyChars;
                enemies[i].GetComponent<CharData>().isInCombat = true;
                enemies[i].GetComponent<CharData>().combatInst = combatInsts;
            }

            SetCharacterHubPositions();
            transitionUI.ExitScene("CombatScene");
        }

        public void DestroyCombatInstance(List<GameObject> players)
        {
            // Increments the number of combat instances appropriately
            combatInsts--;

            // Temporarily removes all player characters from the combat list
            int temp = nonCombatPlayerChars.childCount;
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
                nonCombatPlayerChars.GetChild(0).parent = playerChars;

            // Adds the player characters to the combat list
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].GetComponent<CharData>().isInCombat = false;
                players[i].GetComponent<CharData>().combatInst = -1;
            }

            for (int i = 0; i < enemyChars.childCount; ++i)
                enemyChars.transform.GetChild(i).parent = nonCombatEnemyChars;
            
            transitionUI.ExitScene("Hub");
        }

        private void SetCharacterHubPositions()
        {
            for (int i = 0; i < playerChars.childCount; ++i)
                playerChars.GetChild(i).GetComponent<CharData>().hubPosition = playerChars.GetChild(i).localPosition;
            for (int i = 0; i < enemyChars.childCount; ++i)
                enemyChars.GetChild(i).GetComponent<CharData>().hubPosition = enemyChars.GetChild(i).localPosition;
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
                nonCombatPlayerChars.GetChild(i).GetComponent<CharData>().hubPosition = nonCombatPlayerChars.GetChild(i).localPosition;
            for (int i = 0; i < nonCombatEnemyChars.childCount; ++i)
                nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().hubPosition = nonCombatEnemyChars.GetChild(i).localPosition;
        }

        public void RetrieveCharacterHubPositions()
        {
            for (int i = 0; i < playerChars.childCount; ++i)
                playerChars.GetChild(i).localPosition = playerChars.GetChild(i).GetComponent<CharData>().hubPosition;
            for (int i = 0; i < enemyChars.childCount; ++i)
                enemyChars.GetChild(i).localPosition = enemyChars.GetChild(i).GetComponent<CharData>().hubPosition;
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
                nonCombatPlayerChars.GetChild(i).localPosition = nonCombatPlayerChars.GetChild(i).GetComponent<CharData>().hubPosition;
            for (int i = 0; i < nonCombatEnemyChars.childCount; ++i)
                nonCombatEnemyChars.GetChild(i).localPosition = nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().hubPosition;
        }

        public void RetrieveCharacterCombatPositions()
        {
            for (int i = 0; i < playerChars.childCount; ++i)
                playerChars.GetChild(i).localPosition = playerChars.GetChild(i).GetComponent<CharData>().combatPosition;
            for (int i = 0; i < enemyChars.childCount; ++i)
                enemyChars.GetChild(i).localPosition = enemyChars.GetChild(i).GetComponent<CharData>().combatPosition;
        }
    }
}