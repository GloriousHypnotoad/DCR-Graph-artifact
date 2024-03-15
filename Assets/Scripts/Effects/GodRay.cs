using UnityEngine;
using System.Collections;

public class GodRay : MonoBehaviour
{
    public bool GetIsActive()
    {
        return gameObject.activeSelf;
    }
    public void toggleActive(bool isActive)
    {
        if (isActive)
        {
            gameObject.SetActive(true);
        }
        else
        {
            StartCoroutine(ToggleChildrenAndDeactivateSelf());
        }
    }

    private IEnumerator ToggleChildrenAndDeactivateSelf()
    {
        for (int i = 0; i < 10; i++)
        {
            SetChildrenActive(i % 2 == 0);
            yield return new WaitForSeconds(0.1f);
        }

        SetChildrenActive(true);
        gameObject.SetActive(false);
    }

    private void SetChildrenActive(bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }
}
