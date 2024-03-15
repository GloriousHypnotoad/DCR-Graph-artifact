using System;
using UnityEngine;

public class ProximityDetector : MonoBehaviour
{
    public float detectionRadius = 9f;
    private LayerMask _targetLayer;
    private bool isTargetNearby = false;
    private event Action<bool> _onIsTargetNearby;

    public void SetTargetLayer(int targetLayer){
        _targetLayer = 1 << targetLayer;
    }

    private void FixedUpdate()
    {
        bool targetInRange = Physics.CheckSphere(transform.position, detectionRadius, _targetLayer);
        if (targetInRange && !isTargetNearby)
        {
            isTargetNearby = true;
            _onIsTargetNearby?.Invoke(isTargetNearby);
        }
        else if (!targetInRange && isTargetNearby)
        {
            isTargetNearby = false;
            _onIsTargetNearby?.Invoke(isTargetNearby);
        }
    }

    public void SubscribeToIsTargetNearby(Action<bool> subscriber){
        _onIsTargetNearby += subscriber;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
