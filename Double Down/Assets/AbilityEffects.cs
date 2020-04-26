using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    Damage = 0,
    DamageTP,
    HybridDamage,
    Buff
}

public class AbilityEffects : MonoBehaviour
{
    public AbilityType type = AbilityType.Damage;
    private List<int> buffTypes = new List<int>();
    private List<float> buffEffects = new List<float>();
    private List<int> buffDurations = new List<int>();

    int damage = 0;
    int tpDamage = 0;

    GameObject target = null;
    bool endTurn = false;
    float textDuration = 0.0f;

    [Header("Color Changes")]
    public Color[] colors;
    public float[] colorIntensity;

    [Header("Sound Effects")]
    public AudioClip[] soundClips;
    public float[] volume;

    // Used to initialize certain values, such as damage modifier
    public void InitDamageValues(GameObject targetObj, int displayDamage, bool end)
    {
        type = AbilityType.Damage;
        damage = displayDamage;

        target = targetObj;
        endTurn = end;
        
        GetComponent<Animation>().Play();
    }

    // Used to initialize certain values, such as damage modifier
    public void InitTPDamageValues(GameObject targetObj, int displayTPDamage, bool end)
    {
        type = AbilityType.DamageTP;
        tpDamage = displayTPDamage;

        target = targetObj;
        endTurn = end;

        GetComponent<Animation>().Play();
    }

    public void InitHybridDamageValues(GameObject targetObj, int displayDamage, int displayTPDamage, bool end)
    {
        type = AbilityType.HybridDamage;
        damage = displayDamage;
        tpDamage = displayTPDamage;

        target = targetObj;
        endTurn = end;

        GetComponent<Animation>().Play();
    }

    public void InitBuffValues(GameObject targetObj, List<int> buffs, List<float> buffVals, List<int> buffDuration, bool end, float textDuration)
    {
        type = AbilityType.Buff;

        for (int i = 0; i < buffs.Count; ++i)
            buffTypes.Add(buffs[i]);
        for (int i = 0; i < buffVals.Count; ++i)
            buffEffects.Add(buffVals[i]);
        for (int i = 0; i < buffDuration.Count; ++i)
            buffDurations.Add(buffDuration[i]);

        target = targetObj;
        endTurn = end;
        this.textDuration = textDuration;


        GetComponent<Animation>().Play();
    }

    public void PerformEffect()
    {
        switch (type)
        {
            case AbilityType.Damage:
                DealDamage();
                break;
            case AbilityType.DamageTP:
                DealTPDamage();
                break;
            case AbilityType.HybridDamage:
                DealHybridDamage();
                break;
            case AbilityType.Buff:
                PerformBuffEffect();
                break;
        }
    }

    public void DealDamage()
    {
        Managers.CombatManager.Instance.InflictDamageOnTarget(damage, target, endTurn);
    }

    public void DealTPDamage()
    {
        Managers.CombatManager.Instance.InflictTPDamageOnTarget(tpDamage, target, endTurn);
    }

    public void DealHybridDamage()
    {
        Managers.CombatManager.Instance.InflictHybridDamageOnTarget(damage, tpDamage, target, endTurn);
    }

    public void PerformBuffEffect()
    {
        Managers.CombatManager.Instance.InflictStatusOnTarget(target, buffTypes, buffEffects, buffDurations, endTurn, textDuration);
    }

    public void FlashTarget(int index)
    {
        Color flashCol = colors[index];
        float intensity = colorIntensity[index];
        StartCoroutine(FlashTargetCoroutine(target.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>(), flashCol, intensity));
    }

    IEnumerator FlashTargetCoroutine(SpriteRenderer flashObj, Color flashCol, float intensity)
    {
        flashObj.color = flashCol;
        flashObj.gameObject.SetActive(true);

        while (flashObj.color.a < intensity)
        {
            flashObj.color += new Color(0, 0, 0, (intensity / 5));
            yield return new WaitForSeconds(0.02f);
        }

        while (flashObj.color.a > 0.0f)
        {
            flashObj.color -= new Color(0, 0, 0, (intensity / 5));
            yield return new WaitForSeconds(0.02f);
        }

        flashObj.gameObject.SetActive(false);
        flashObj.color = new Color(1, 1, 1, 0.2941177f);

        yield return null;
    }

    public void PlaySoundEffect(int index)
    {
        Managers.SoundEffectManager.Instance.PlaySoundClip(soundClips[index], volume[index]);
    }

    public void SlowdownTime(float timeChange)
    {
        Time.timeScale = timeChange;
    }

    public void ShakeScreen(float intensity)
    {
        Camera.main.GetComponent<CameraManager>().ShakeScreen(intensity);
    }

    public void DestroyAnim()
    {
        target.transform.GetChild(0).gameObject.SetActive(false);
        target.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2941177f);
        Destroy(gameObject);
    }
}
