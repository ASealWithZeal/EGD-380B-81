using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationTarget
{
    One = 0,
    Enemies,
    Allies
}

public class AbilityEffectsGenerator : MonoBehaviour
{
    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAnimation(GameObject animationObj, GameObject target, int damage, int tpDamage, bool end, int damageType)
    {
        if (animationObj != null)
        {
            // Get the position on the canvas
            Vector2 uiOffset = new Vector2(canvas.GetComponent<RectTransform>().sizeDelta.x / 2f, canvas.GetComponent<RectTransform>().sizeDelta.y / 2f);

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.transform.position);
            Vector2 proportionalPosition = new Vector2(ViewportPosition.x * canvas.GetComponent<RectTransform>().sizeDelta.x, ViewportPosition.y * canvas.GetComponent<RectTransform>().sizeDelta.y);
            Vector2 actualPosition = proportionalPosition - uiOffset;

            // Set the position and remove the screen offset
            GameObject g = Instantiate(animationObj, transform);

            if (target.tag == "Player")
                g.transform.localScale = new Vector3(-1, 1, 1);

            g.transform.localPosition = actualPosition;

            if (damageType == 0)
                g.GetComponent<AbilityEffects>().InitDamageValues(target, damage, end);
            else if (damageType == 1)
                g.GetComponent<AbilityEffects>().InitTPDamageValues(target, tpDamage, end);
            else if (damageType == 2)
                g.GetComponent<AbilityEffects>().InitHybridDamageValues(target, damage, tpDamage, end);
        }
        else
        {
            if (damageType == 0)
                Managers.CombatManager.Instance.InflictDamageOnTarget(damage, target, end);
            else if (damageType == 1)
                Managers.CombatManager.Instance.InflictTPDamageOnTarget(damage, target, end);
            else if (damageType == 2)
                Managers.CombatManager.Instance.InflictHybridDamageOnTarget(damage, tpDamage, target, end);
        }
    }

    public void CreateAnimation(GameObject animationObj, GameObject target, List<int> type, List<float> mod, List<int> duration, bool end, float textDuration)
    {
        if (animationObj != null)
        {
            // Get the position on the canvas
            Vector2 uiOffset = new Vector2(canvas.GetComponent<RectTransform>().sizeDelta.x / 2f, canvas.GetComponent<RectTransform>().sizeDelta.y / 2f);

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.transform.position);
            Vector2 proportionalPosition = new Vector2(ViewportPosition.x * canvas.GetComponent<RectTransform>().sizeDelta.x, ViewportPosition.y * canvas.GetComponent<RectTransform>().sizeDelta.y);
            Vector2 actualPosition = proportionalPosition - uiOffset;

            // Set the position and remove the screen offset
            GameObject g = Instantiate(animationObj, transform);

            if (target.tag == "Player")
                g.transform.localScale = new Vector3(-1, 1, 1);

            g.transform.localPosition = actualPosition;
            g.GetComponent<AbilityEffects>().InitBuffValues(target, type, mod, duration, end, textDuration);
        }
        else
            Managers.CombatManager.Instance.InflictStatusOnTarget(target, type, mod, duration, end, textDuration);
    }
}
