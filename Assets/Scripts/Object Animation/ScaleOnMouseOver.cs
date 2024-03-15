using UnityEngine;

public class ContinuousScaleOnMouseOver : MonoBehaviour
{
    private float scaleFactor = 2f;
    private float transitionSpeed = 2f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isMouseOver = false;

    private bool scalingUp = true;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * scaleFactor;
    }

    void Update()
    {
        if (isMouseOver)
        {
            if (scalingUp)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, transitionSpeed * Time.deltaTime);
                if (transform.localScale == targetScale)
                {
                    scalingUp = false;
                }
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, originalScale, transitionSpeed * Time.deltaTime);
                if (transform.localScale == originalScale)
                {
                    scalingUp = true;
                }
            }
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, transitionSpeed * Time.deltaTime);
        }
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        scalingUp = true;
    }
}
