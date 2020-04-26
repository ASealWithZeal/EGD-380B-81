using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3 cameraHubPos;
    public Vector3 cameraCombatPos;

    public bool lerping = false;
    private GameObject lerpTarget = null;
    private Vector3 lerpOffset;

    private bool shaking = false;

    public void SetCameraPos(int i)
    {
        if (i == 0)
        {
            transform.position = cameraHubPos;
            transform.localEulerAngles = new Vector3(15, 0, 0);
        }
        else if (i == 1)
        {
            transform.position = cameraCombatPos;
            transform.localEulerAngles = new Vector3(10, 5, 0);
        }
    }

    private void FixedUpdate()
    {
        Lerp();
    }

    private void Lerp()
    {
        if (lerping)
        {
            transform.position = Vector3.Lerp(transform.position, lerpTarget.transform.position + lerpOffset, Time.deltaTime * 10);

            if ((transform.position.x + 0.01f > lerpTarget.transform.position.x && transform.position.x - 0.01f < lerpTarget.transform.position.x)
            || (transform.position.z + 0.01f > lerpTarget.transform.position.z && transform.position.z - 0.01f < lerpTarget.transform.position.z))
            {
                transform.position = lerpTarget.transform.position + lerpOffset;
                lerping = false;
                lerpTarget = null;
            }
        }
    }

    public void StoreCameraHubPos()
    {
        cameraHubPos = transform.position;
    }
    
    public void StoreCameraCombatPos()
    {
        cameraCombatPos = transform.position;
    }

    public void LerpToTarget(GameObject target, Vector3 offset)
    {
        lerping = true;
        lerpTarget = target;
        lerpOffset = offset;
    }

    public void ShakeScreen(float intensity)
    {
        if (!shaking)
        {
            shaking = true;
            StartCoroutine(ShakeScreenCoroutine(intensity));
        }
    }

    IEnumerator ShakeScreenCoroutine(float intensity)
    {
        float shakeTime = 0.1f;
        Vector3 position = transform.localPosition;
        while (shakeTime > 0)
        {
            transform.localPosition = position;
            Vector3 move = new Vector3(Random.insideUnitSphere.x * intensity, Random.insideUnitSphere.y * intensity, 0);
            transform.position += move;
            shakeTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        shaking = false;
        transform.localPosition = position;
        yield return null;
    }
}