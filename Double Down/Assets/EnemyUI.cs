using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    public Image healthBar = null;
    public List<Color> colors = null;
    public Canvas parentCanvas = null;
    public TextMeshProUGUI name = null;

    // Start is called before the first frame update
    void Start()
    {
        //healthBar.color = colors[0];
    }

    public void ChangeHealth(float newHealthPercent)
    {
        StartCoroutine(AlterHealthBar(newHealthPercent));
    }

    public void CreateUI(string name, Vector3 objPosition, GameObject obj, float healthPercent)
    {
        this.name.SetText(name);
        healthBar.fillAmount = healthPercent;
        healthBar.color = HealthPercentColor();
        parentCanvas = GameObject.Find("_CombatCanvas").GetComponent<Canvas>();

        // Get the position on the canvas
        Vector2 uiOffset = new Vector2(parentCanvas.GetComponent<RectTransform>().sizeDelta.x / 2f, parentCanvas.GetComponent<RectTransform>().sizeDelta.y / 2f);

        Vector2 sprite_size = obj.GetComponent<SpriteRenderer>().sprite.rect.size;

        //convert to screen space size
        Vector2 size = new Vector2(0, (sprite_size.y / 2.0f) + 15);

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objPosition);
        Vector2 proportionalPosition = new Vector2(ViewportPosition.x * parentCanvas.GetComponent<RectTransform>().sizeDelta.x, ViewportPosition.y * parentCanvas.GetComponent<RectTransform>().sizeDelta.y);
        Vector2 actualPosition = proportionalPosition - uiOffset;
        //actualPosition.y = (actualPosition.y / objTransform.localScale.y) - ((0.5f + (objTransform.localScale.y / 10)) * objTransform.localScale.y) - (2.0f - objTransform.position.z);

        // Set the position and remove the screen offset
        gameObject.transform.localPosition = actualPosition - size;
    }

    IEnumerator AlterHealthBar(float newHealthPercent)
    {
        bool decrease = true;
        float multiplier = 1;
        if (healthBar.fillAmount < newHealthPercent)
        {
            decrease = false;
            multiplier = -1;
        }

        while ((decrease && healthBar.fillAmount > newHealthPercent)
            || (!decrease && healthBar.fillAmount < newHealthPercent))
        {
            healthBar.fillAmount -= 0.01f * multiplier;
            healthBar.color = HealthPercentColor();

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        healthBar.fillAmount = newHealthPercent;

        yield return null;
    }

    private Color HealthPercentColor()
    {
        Color c = new Color();

        if (healthBar.fillAmount > 0.5f)
            c = colors[0];
        else if (healthBar.fillAmount <= 0.5f && healthBar.fillAmount > 0.25f)
            c = colors[1];
        else if (healthBar.fillAmount <= 0.25f)
            c = colors[2];

        return c;
    }
}
