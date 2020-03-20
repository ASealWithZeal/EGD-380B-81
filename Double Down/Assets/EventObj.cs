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
    private bool combatActive = false;
    private bool contactActive = true;
    private bool init = false;
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
        {
            check = true;
        }

        if (!check)
        {
            for (int i = 0; i < enemies.Count; ++i)
                enemies[0].SetActive(false);

            box.DisableBox();
            gameObject.SetActive(false);
        }
        else if (check && enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; ++i)
                if (enemies[i].GetComponent<CharData>().isInCombat)
                    combatActive = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (contactActive && other.gameObject == Managers.TurnManager.Instance.t1[0] && !other.gameObject.GetComponent<CharData>().hasActed 
            && !other.gameObject.GetComponent<CharData>().isInCombat && other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            box.PassEventIn(text, gameObject);
        }

        else if (other.gameObject == Managers.TurnManager.Instance.t1[0] && !other.gameObject.GetComponent<CharData>().hasActed 
            && other.gameObject.GetComponent<CharData>().isInCombat && other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            PassExistingCombat();
        }
    }

    public void PassResponse(bool @bool)
    {
        if (type == HubEvents.Pass && @bool)
            Managers.TurnManager.Instance.EndRound();
        else if (type == HubEvents.Battle && @bool && !combatActive)
            Managers.CombatTransitionManager.Instance.CreateNewCombatInstance(eventNum, player, enemies);
        else if (type == HubEvents.Battle && @bool && combatActive)
            PassExistingCombat();
        else if (!@bool)
            Managers.MovementManager.Instance.canMoveChars = true;
    }

    public void PassExistingCombat()
    {
        Debug.Log("HERE");
        Managers.TurnManager.Instance.t1[0].GetComponent<CharacterController>().enabled = false;
        Managers.CombatTransitionManager.Instance.EnterExistingCombatInstance(eventNum, player, enemies);
    }
}
