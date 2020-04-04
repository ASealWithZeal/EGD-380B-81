using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CombatTransitionManager : Singleton<CombatTransitionManager>
    {
        public CameraManager cam;
        public ExpContainer container;
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

        public void CreateNewCombatInstance(HubEvents type, int combatInst, GameObject player, List<GameObject> enemies)
        {
            container.SetCombatInst(combatInst);
            container.battleType = type;

            // Temporarily removes all player characters from the combat list
            int temp = playerChars.childCount;
            for (int i = 0; i < temp; ++i)
                playerChars.GetChild(0).parent = nonCombatPlayerChars;

            // Adds the participating player character to the combat list
            player.transform.parent = playerChars;
            player.GetComponent<CharData>().isInCombat = true;
            player.GetComponent<CharData>().combatInst = combatInst;

            // Adds the enemies to the combat list
            for (int i = 0; i < enemies.Count; ++i)
            {
                enemies[i].transform.parent = enemyChars;
                enemies[i].GetComponent<CharData>().isInCombat = true;
                enemies[i].GetComponent<CharData>().combatInst = combatInst;
            }

            SetCharacterHubPositions();
            cam.StoreCameraHubPos();
            transitionUI.ExitScene("CombatScene");
        }

        public void EnterExistingCombatInstance(HubEvents type, int combatInst, GameObject player, List<GameObject> enemies)
        {
            container.SetCombatInst(combatInst);
            container.battleType = type;

            // Temporarily removes all player characters from the combat list
            int temp = playerChars.childCount;
            for (int i = 0; i < temp; ++i)
                playerChars.GetChild(0).parent = nonCombatPlayerChars;

            // Adds the participating player character to the combat list
            player.transform.parent = playerChars;
            player.GetComponent<CharData>().isInCombat = true;
            player.GetComponent<CharData>().combatInst = combatInst;

            Debug.Log(nonCombatPlayerChars.childCount);
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
            {
                if (nonCombatPlayerChars.GetChild(i).GetComponent<CharData>().combatInst == combatInst)
                    nonCombatPlayerChars.GetChild(i).parent = playerChars;
            }

            // Adds the enemies to the combat list
            for (int i = 0; i < enemies.Count; ++i)
            {
                if (!enemies[i].GetComponent<CharData>().dead)
                {
                    enemies[i].transform.parent = enemyChars;
                    enemies[i].GetComponent<CharData>().isInCombat = true;
                    enemies[i].GetComponent<CharData>().combatInst = combatInst;
                }
            }

            SetCharacterHubPositions();
            cam.StoreCameraHubPos();
            transitionUI.ExitScene("CombatScene");
        }

        public void ExitExistingCombatInstance(List<GameObject> players)
        {
            SetCharacterCombatPositions();
            cam.StoreCameraCombatPos();

            // Adds all player characters back to the combat list
            int temp = nonCombatPlayerChars.childCount;
            for (int i = 0; i < temp; ++i)
                nonCombatPlayerChars.GetChild(0).parent = playerChars;

            // Removes all enemies from the combat list
            temp = enemyChars.childCount;
            for (int i = 0; i < temp; ++i)
                enemyChars.transform.GetChild(0).parent = nonCombatEnemyChars;
            
            transitionUI.ExitScene("Hub");
        }

        public void DestroyCombatInstance(List<GameObject> players)
        {
            container.FlushWinExp();

            // Sets all involved characters as not currently being involved in combat
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].GetComponent<CharData>().isInCombat = false;
                players[i].GetComponent<CharData>().combatInst = -1;
                players[i].GetComponent<CharData>().ResetInfo();
                players[i].GetComponent<Stats>().DestroyMods();
                players[i].GetComponent<CharData>().DestroyBuffUI();

                if (!players[i].GetComponent<CharData>().dead)
                {
                    players[i].GetComponent<CharData>().ChangeCharUIDisplay();
                    players[i].GetComponent<CharData>().FullRestore();
                }
            }

            // Adds all "missing" characters to the combat list
            int temp = nonCombatPlayerChars.childCount;
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
                nonCombatPlayerChars.GetChild(0).parent = playerChars;

            // Removes all enemies, IF ANY EXIST, from the combat list
            temp = enemyChars.childCount;
            for (int i = 0; i < temp; ++i)
            {
                enemyChars.GetChild(0).GetComponent<CharData>().isInCombat = false;
                enemyChars.GetChild(0).GetComponent<CharData>().combatInst = -1;
                enemyChars.GetChild(0).GetComponent<CharData>().FullRestore();
                enemyChars.GetChild(0).parent = nonCombatEnemyChars;
            }

            ResetCharacterCombatPositions();
            //cam.StoreCameraCombatPos();
            transitionUI.ExitScene("Hub");
        }

        // Resets a combat instance when all player characters die
        // "Revives" all enemies
        public void ResetCombatInstance(List<GameObject> players)
        {
            container.FlushWinExp();

            // Sets all involved characters as not currently being involved in combat
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].GetComponent<CharData>().isInCombat = false;
                players[i].GetComponent<CharData>().combatInst = -1;
                players[i].GetComponent<CharData>().ResetInfo();
                players[i].GetComponent<Stats>().DestroyMods();
                players[i].GetComponent<CharData>().DestroyBuffUI();
            }

            // Adds all "missing" characters to the combat list
            int temp = nonCombatPlayerChars.childCount;
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
                nonCombatPlayerChars.GetChild(0).parent = playerChars;

            // Removes all enemies, IF ANY EXIST, from the combat list
            temp = nonCombatEnemyChars.childCount;
            for (int i = 0; i < temp; ++i)
            {
                if (nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().attachedEventNum == enemyChars.GetChild(0).GetComponent<CharData>().attachedEventNum)
                {
                    nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().dead = false;
                    nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().FullRestore();
                    nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().ChangeColor();
                }
            }

            // Removes all enemies, IF ANY EXIST, from the combat list
            temp = enemyChars.childCount;
            for (int i = 0; i < temp; ++i)
            {
                enemyChars.GetChild(0).GetComponent<CharData>().isInCombat = false;
                enemyChars.GetChild(0).GetComponent<CharData>().dead = false;
                enemyChars.GetChild(0).GetComponent<CharData>().combatInst = -1;
                enemyChars.GetChild(0).GetComponent<CharData>().FullRestore();
                enemyChars.GetChild(0).GetComponent<CharData>().ChangeColor();

                enemyChars.GetChild(0).parent = nonCombatEnemyChars;
            }

            ResetCharacterCombatPositions();
            //cam.StoreCameraCombatPos();
            transitionUI.ExitScene("Hub");
        }

        private void SetCharacterHubPositions()
        {
            for (int i = 0; i < playerChars.childCount; ++i)
            {
                playerChars.GetChild(i).GetComponent<CharData>().facingDir = playerChars.GetChild(i).GetComponent<SpriteRenderer>().flipX;
                playerChars.GetChild(i).GetComponent<CharData>().hubPosition = playerChars.GetChild(i).localPosition;
            }
            for (int i = 0; i < enemyChars.childCount; ++i)
            {
                enemyChars.GetChild(i).GetComponent<CharData>().facingDir = enemyChars.GetChild(i).GetComponent<SpriteRenderer>().flipX;
                enemyChars.GetChild(i).GetComponent<CharData>().hubPosition = enemyChars.GetChild(i).localPosition;
            }
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
                nonCombatPlayerChars.GetChild(i).GetComponent<CharData>().hubPosition = nonCombatPlayerChars.GetChild(i).localPosition;
            for (int i = 0; i < nonCombatEnemyChars.childCount; ++i)
                nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().hubPosition = nonCombatEnemyChars.GetChild(i).localPosition;
        }

        private void SetCharacterCombatPositions()
        {
            for (int i = 0; i < playerChars.childCount; ++i)
                playerChars.GetChild(i).GetComponent<CharData>().combatPosition = playerChars.GetChild(i).localPosition;
            for (int i = 0; i < enemyChars.childCount; ++i)
                enemyChars.GetChild(i).GetComponent<CharData>().combatPosition = enemyChars.GetChild(i).localPosition;
        }

        private void ResetCharacterCombatPositions()
        {
            for (int i = 0; i < playerChars.childCount; ++i)
                playerChars.GetChild(i).GetComponent<CharData>().combatPosition = new Vector3(-9f, -0.5f, -2f);
        }

        public void RetrieveCharacterHubPositions()
        {
            for (int i = 0; i < playerChars.childCount; ++i)
            {
                playerChars.GetChild(i).GetComponent<SpriteRenderer>().flipX = playerChars.GetChild(i).GetComponent<CharData>().facingDir;
                playerChars.GetChild(i).localPosition = playerChars.GetChild(i).GetComponent<CharData>().hubPosition;
            }
            for (int i = 0; i < enemyChars.childCount; ++i)
            {
                enemyChars.GetChild(i).GetComponent<SpriteRenderer>().flipX = enemyChars.GetChild(i).GetComponent<CharData>().facingDir;
                enemyChars.GetChild(i).localPosition = enemyChars.GetChild(i).GetComponent<CharData>().hubPosition;
                enemyChars.GetChild(i).GetComponent<EnemyAnimator>().UpdateAnimations();
                enemyChars.GetChild(i).GetComponent<EnemyAnimator>().HideShadow();
            }
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
                nonCombatPlayerChars.GetChild(i).localPosition = nonCombatPlayerChars.GetChild(i).GetComponent<CharData>().hubPosition;
            for (int i = 0; i < nonCombatEnemyChars.childCount; ++i)
            {
                nonCombatEnemyChars.GetChild(i).GetComponent<SpriteRenderer>().flipX = nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().facingDir;
                nonCombatEnemyChars.GetChild(i).localPosition = nonCombatEnemyChars.GetChild(i).GetComponent<CharData>().hubPosition;
                nonCombatEnemyChars.GetChild(i).GetComponent<EnemyAnimator>().UpdateAnimations();
                nonCombatEnemyChars.GetChild(i).GetComponent<EnemyAnimator>().HideShadow();
            }

            cam.SetCameraPos(0);
        }

        public void RetrieveCharacterCombatPositions()
        {
            for (int i = 0; i < playerChars.childCount; ++i)
            {
                playerChars.GetChild(i).GetComponent<SpriteRenderer>().flipX = true;
                playerChars.GetChild(i).localPosition = playerChars.GetChild(i).GetComponent<CharData>().combatPosition;
            }
            for (int i = 0; i < enemyChars.childCount; ++i)
            {
                enemyChars.GetChild(i).GetComponent<SpriteRenderer>().flipX = false;
                enemyChars.GetChild(i).localPosition = enemyChars.GetChild(i).GetComponent<CharData>().combatPosition;
                enemyChars.GetChild(i).GetComponent<EnemyAnimator>().UpdateAnimations();
            }
            for (int i = 0; i < nonCombatPlayerChars.childCount; ++i)
                nonCombatPlayerChars.GetChild(i).localPosition = new Vector3(-2000f, -0.5f, -2f);
            for (int i = 0; i < nonCombatEnemyChars.childCount; ++i)
                nonCombatEnemyChars.GetChild(i).localPosition = new Vector3(-2000f, -0.5f, -2f);

            cam.SetCameraPos(1);
        }
    }
}