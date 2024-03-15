using System.Collections;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    public float rotationSpeed = 25.0f;
    private float originalSpeed;
    public float quickRotationSpeed = 900.0f;
    public int numberOfQuickRotations = 3;
    private bool _isRotating = false;

    void Start()
    {
        originalSpeed = rotationSpeed;
    }

    void Update()
    {
        if (_isRotating)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    public void ToggleAnimation(bool isRotating)
    {
        _isRotating = isRotating;
    }

    public void PerformQuickRotation()
    {
        StartCoroutine(QuickRotationCoroutine());
    }

    public float getQuickRotationDuration(){
        return numberOfQuickRotations * 360 / quickRotationSpeed;
    }

    private IEnumerator QuickRotationCoroutine()
    {
        rotationSpeed = quickRotationSpeed;
        yield return new WaitForSeconds(getQuickRotationDuration());
        rotationSpeed = originalSpeed;
    }

    public bool IsAnimationRunning()
    {
        return _isRotating;
    }
}
