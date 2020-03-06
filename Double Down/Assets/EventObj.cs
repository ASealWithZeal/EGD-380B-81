using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HubEvents
{
    Pass,
    Battle
}

public class EventObj : MonoBehaviour
{
    public HubEvents type;
    public string text;
    public EventTextBox box = null;
    public List<GameObject> enemies = null;
    public GameObject player;
    public int eventNum = 0;

    private void Start()
    {
        Transform enemyCharsContainer = GameObject.Find("NonCombatEnemies").transform;
        List<GameObject> enemyChars = new List<GameObject>();
        for (int i = 0; i < enemyCharsContainer.childCount; ++i)
        {
            if (enemyCharsContainer.GetChild(i).GetComponent<CharData>().attachedEventNum == eventNum)
                enemyChars.Add(enemyCharsContainer.GetChild(i).gameObject);
        }

        enemies.Clear();
        for (int i = 0; i < enemyChars.Count; ++i)
            enemies.Add(enemyChars[i]);

        bool check = false;

        for (int i = 0; i < enemies.Count; ++i)
        {
            if (type == HubEvents.Battle && !enemies[i].GetComponent<CharData>().dead)
                check = true;
        }

        if (type == HubEvents.Pass)
            check = true;

        if (!check)
        {
            for (int i = 0; i < enemies.Count; ++i)
            {
                //Destroy(enemies[0]);
                //enemies.Remove(enemies[0]);
                enemies[0].SetActive(false);
            }

            box.DisableBox();
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Managers.TurnManager.Instance.t1[0] && other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            box.PassEventIn(text, gameObject);
        }
    }

    public void PassResponse(bool @bool)
    {
        if (type == HubEvents.Pass && @bool)
        {
            Managers.TurnManager.Instance.EndRound();
        }
        else if (type == HubEvents.Battle && @bool)
        {
            Managers.CombatTransitionManager.Instance.CreateNewCombatInstance(player, enemies);
        }
        else if (!@bool)
        {
            Managers.MovementManager.Instance.canMoveChars = true;
        }
    }
}
