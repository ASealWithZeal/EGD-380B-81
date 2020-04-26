using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlashEffect : MonoBehaviour
{
    public Image thisImage;

    public void Flash(Color flashColor, float intensity, float length)
    {
        StartCoroutine(FlashCoroutine(flashColor, intensity, length));
    }

    IEnumerator FlashCoroutine(Color flashColor, float intensity, float length)
    {
        thisImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
        float quarterLength = length / 4.0f;

        while (thisImage.color.a < intensity)
        {
            thisImage.color += new Color(0, 0, 0, (1.0f / (quarterLength)) * 0.05f);
            yield return new WaitForSeconds(0.02f);
        }

        while (thisImage.color.a > 0)
        {
            thisImage.color -= new Color(0, 0, 0, (1.0f / (quarterLength * 3)) * 0.05f);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
