using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackVisual : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    private Vector3 targetPosition;
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 50f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);
        //checks if we have overshot
        if(distanceBeforeMoving < distanceAfterMoving) {
            transform.position = targetPosition;
            //we have hit so we destroy ourselves
            trailRenderer.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
