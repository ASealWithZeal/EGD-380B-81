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

            SceneChangeManager.Instance.ChangeScene("CombatScene");
        }
    }
}