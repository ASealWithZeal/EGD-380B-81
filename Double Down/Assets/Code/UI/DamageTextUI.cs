﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageTextUI : MonoBehaviour
{
    public GameObject textObject;
    public GameObject targetPos;

    public Canvas parentCanvas;
    private Vector2 uiOffset;

    public List<Color> textColors;

    public float timeBetweenButtons = 0.05f;

    /// <summary>
    /// Initiate
    /// </summary>
    void Start()
    {
        // Calculate the screen offset
        uiOffset = new Vector2(parentCanvas.GetComponent<RectTransform>().sizeDelta.x / 2f, parentCanvas.GetComponent<RectTransform>().sizeDelta.y / 2f);
    }

    public void DamageNumbers(int damage, Transform target, bool canEnd)
    {
        StartCoroutine(SpawnDamageNumbers(damage, target, canEnd));
    }

    public IEnumerator SpawnDamageNumbers(int damage, Transform target, bool canEnd)
    {
        Color currentColor;

        if (damage < 0)
        {
            damage = Mathf.Abs(damage);
            currentColor = textColors[1];
        }

        else
            currentColor = textColors[0];

        string dam = damage.ToString();
        float textOffset = (dam.Length - 1) * 0.5f;
        float offsetMod = 6.3f - (dam.Length * 0.25f);
        float offset = (dam.Length / (offsetMod * textOffset)) * 35.0f;

        List<GameObject> buttons = new List<GameObject>();

        for (int i = 0; i < dam.Length; ++i)
        {
            GameObject newText = Instantiate(textObject, GetComponent<RectTransform>());

            newText.GetComponent<TextMeshProUGUI>().fontSize = 35.0f;
            newText.GetComponent<TextMeshProUGUI>().color = currentColor;
            newText.GetComponent<TextMeshProUGUI>().SetText(dam[i].ToString());

            // Get the position on the canvas
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.position);
            Vector2 proportionalPosition = new Vector2(ViewportPosition.x * parentCanvas.GetComponent<RectTransform>().sizeDelta.x, ViewportPosition.y * parentCanvas.GetComponent<RectTransform>().sizeDelta.y);

            // Set the position and remove the screen offset
            newText.GetComponent<TextMeshProUGUI>().rectTransform.localPosition = proportionalPosition - uiOffset;
            if (dam.Length > 1)
                newText.GetComponent<TextMeshProUGUI>().rectTransform.localPosition += new Vector3(offset * (i - textOffset), 0, 0);

            buttons.Add(newText);

            yield return new WaitForSeconds(timeBetweenButtons);
        }

        while (!buttons[buttons.Count - 1].GetComponent<DamageTextSpawnUI>().landed)
        {
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.2f);

        int buttonCount = buttons.Count;
        for (int i = 0; i < buttonCount; ++i)
        {
            Destroy(buttons[0]);
            buttons.RemoveAt(0);
        }

        buttons.Clear();

        if (canEnd)
            Managers.CombatManager.Instance.FollowUpAction();

        yield return null;
    }
}
