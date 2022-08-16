using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    private BaseAction[] actions;
    private HealthSystem healthSystem;
    public Animator unitAnimator;
    [SerializeField] private int actionPointsMax;
    [SerializeField] private bool isEnemy;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int maxMoveDistance;
    private int actionPoints;
    private GridPosition gridPosition; 

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        actions = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = GameManager.Instance.levelGrid.GetGridPosition(transform.position);
        GameManager.Instance.levelGrid.GetGridObject(gridPosition).SetUnit(this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;
        actionPoints = actionPointsMax;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = GameManager.Instance.levelGrid.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            GameManager.Instance.levelGrid.GetGridObject(gridPosition).ClearUnit();
            GameManager.Instance.levelGrid.GetGridObject(newGridPosition).SetUnit(this);
            gridPosition = newGridPosition;
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((isEnemy && !TurnSystem.Instance.IsPlayerTurn()) || (!isEnemy && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = actionPointsMax;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        GameManager.Instance.levelGrid.GetGridObject(gridPosition).ClearUnit();
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public T GetAction<T>() where T: BaseAction
    {
        foreach (BaseAction baseAction in actions)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }
    
    public BaseAction[] GetActions()
    {
        return actions;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public int GetMaxMoveDistance()
    {
        return maxMoveDistance;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    public bool TrySpendPointsToTakeAction(BaseAction baseAction)
    {
        if(CanSpendPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        return false;
    }
    public bool CanSpendPointsToTakeAction(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointsCost();
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints() {
        return actionPoints;    
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
       
    }
}
