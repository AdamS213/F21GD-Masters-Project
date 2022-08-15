using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackVisualPrefab;
    [SerializeField] private Transform spellPointTransform;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.onStartMoving += MoveAction_OnStartMoving;
            moveAction.onStopMoving += MoveAction_OnStopMoving;
        }
        if (TryGetComponent<AttackAction>(out AttackAction attackAction))
        {
            attackAction.onAttack += AttackAction_OnAttack;
            
        }
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void AttackAction_OnAttack(object sender, AttackAction.OnAttackEventArgs e)
    {
        animator.SetTrigger("Attack");

        
        Transform attackVisualTransform = Instantiate(attackVisualPrefab, spellPointTransform.position, Quaternion.identity);
        AttackVisual attackVisual = attackVisualTransform.GetComponent<AttackVisual>();

        attackVisual.Setup(e.targetUnit.GetWorldPosition());
    }
}
