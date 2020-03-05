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

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
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
