using System.Collections;
using UnityEngine;

public class ObjectBobbing : MonoBehaviour
{
    private float amplitude = 0.1f;
    private float frequency = 2f;

    private Vector3 startPos;
    private bool _isBobbing;

    void Awake()
    {
        startPos = transform.position;
        _isBobbing = false;
    }

    void Update()
    {
        if (_isBobbing)
        {
            float tempVal = startPos.y + amplitude * Mathf.Sin(Time.time * frequency);
            transform.position = new Vector3(startPos.x, tempVal, startPos.z);
        }
    }

    public void ToggleAnimation(bool isBobbing)
    {
        _isBobbing = isBobbing;
        if (!_isBobbing)
        {
            StartCoroutine(ReturnToStart());
        }
    }

    private IEnumerator ReturnToStart()
    {
        while (transform.position != startPos)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime);
            yield return null;
        }
    }

    public bool IsAnimationRunning()
    {
        return _isBobbing;
    }
}