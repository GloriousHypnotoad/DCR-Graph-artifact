using UnityEngine;
using System.Collections;

public class ObjectShake : MonoBehaviour
{
    private float _shakeDuration = 0.5f;
    private float _shakeIntensity = 0.5f;

    private Vector3 originalPosition;
    private bool isShaking = false;
    public void StartShake()
    {
        if (!isShaking)
        {
            originalPosition = transform.position;
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        isShaking = true;
        float endTime = Time.time + _shakeDuration;

        while (Time.time < endTime)
        {
            float x = Random.Range(-_shakeIntensity, _shakeIntensity) + originalPosition.x;
            float y = Random.Range(-_shakeIntensity, _shakeIntensity) + originalPosition.y;
            float z = Random.Range(-_shakeIntensity, _shakeIntensity) + originalPosition.z;

            transform.position = new Vector3(x, y, z);
            yield return null;
        }

        transform.position = originalPosition;
        isShaking = false;
    }
}