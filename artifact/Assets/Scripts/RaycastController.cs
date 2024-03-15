using UnityEngine;
using UnityEngine.UI;

public class RaycastController : MonoBehaviour
{
    public Image reticule;
    public Color defaultColor = Color.white;
    public Color targetColor = Color.green;
    public Color disabledColor = Color.red;    
    public float rayLength = 100f;
    private string _targetTag;
    private LayerMask _targetLayer;

    void Awake()
    {
        reticule = GetComponentInChildren<Image>();
    }

    public void SetTarget(string targetTag, int targetLayer){
        _targetTag = targetTag;
        _targetLayer = 1 << targetLayer;
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength, _targetLayer))
        {
            GameObject hitGameObject = hit.collider.gameObject;
            if (hitGameObject.CompareTag(_targetTag))
            {
                ButtonController buttonController = hitGameObject.GetComponent<ButtonController>();
                if (buttonController.ButtonEnabled)
                {
                    reticule.color = targetColor;
                }
                else
                {
                    reticule.color = disabledColor;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    hitGameObject.SendMessage("OnSelected", SendMessageOptions.DontRequireReceiver);
                }
            }
        }        
        else
        {
            reticule.color = defaultColor;
        }
    }
}