using System.Collections;
using UnityEngine;

public class ObjectRotationContinuous : MonoBehaviour
{
    public float rotationSpeed = 25.0f;
    private float originalSpeed;
    public float quickRotationSpeed = 900.0f;
    public int numberOfQuickRotations = 3;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    public void PerformQuickRotation()
    {
        StartCoroutine(QuickRotationCoroutine());
    }

    private IEnumerator QuickRotationCoroutine()
    {
        float previousSpeed = rotationSpeed;
        rotationSpeed = quickRotationSpeed;

        float quickRotationDuration = numberOfQuickRotations * 360 / quickRotationSpeed;
        yield return new WaitForSeconds(quickRotationDuration);

        rotationSpeed = previousSpeed;
    }
}
