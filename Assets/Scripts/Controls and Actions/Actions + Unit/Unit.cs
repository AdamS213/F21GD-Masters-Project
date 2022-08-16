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
    [SerializeField] private bool isLootable;

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

    public bool IsLootable()
    {
        return isLootable;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
       
    }

    public bool checkIfOpposedUnitInSight()
    {
        if(isEnemy)
        {
            foreach (Unit unit in UnitManager.Instance.GetFriendlyUnitList())
            {
                if (unit.IsLootable())
                {
                    continue;
                }
                else
                {
                    return checkIfOpposedUnitInSight(unit);
                }
            }
        }
        //code may be relevant in future work but isn't in current build
        else
        {
            foreach (Unit unit in UnitManager.Instance.GetEnemyUnitList())
            {
                //may want to change if idea for stealing from enemies for points is implemented
                if (unit.IsLootable())
                {
                    continue;
                }
                else
                {
                    return checkIfOpposedUnitInSight(unit);
                }
            }
        }
        return false;

    }

    public bool checkIfOpposedUnitInSight(Unit targetUnit)
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Obstacle");
        Vector3 direction = targetUnit.GetWorldPosition() - transform.position;
        float distanceToNode = Vector3.Distance(transform.position, targetUnit.GetWorldPosition());
        if (Physics.Raycast(transform.position, direction, out hit, distanceToNode, mask))
        {
            float distanceToHit = Vector3.Distance(transform.position, hit.point);

            if ((distanceToNode - distanceToHit) < 1.0f)
            {
                return true;
            }
        }
        return false;
    }
}
