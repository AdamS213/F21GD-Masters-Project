using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAi : MonoBehaviour
{

    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer = 1f;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if(TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state) 
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if(TryTakeEnemyAiAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }else
                    {
                        //no more enemies that can take actions
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }

        
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 1f;
        }
        
    }

    private bool TryTakeEnemyAiAction(Action onEnemyAiActionComplete)
    {
        List<Unit> enemies = UnitManager.Instance.GetEnemyUnitList();
        
        foreach (Unit enemyUnit in enemies)
        {
            if (TryTakeEnemyAiAction(enemyUnit, onEnemyAiActionComplete))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAiAction(Unit enemyUnit, Action onEnemyAiActionComplete)
    {
        EnemyAiAction bestEnemyAiAction = null;
        BaseAction bestBaseAction = null;
        foreach(BaseAction baseAction in enemyUnit.GetActions())
        {
            //if we don't have enough action points to perform this action check the next
            if(!enemyUnit.CanSpendPointsToTakeAction(baseAction))
            {
                continue;
            }
            //if the action has no viable AiActions for us we check the next
            if(baseAction.GetBestEnemyAiAction() == null)
            {
                continue;
            }
            //if the actions best AiAction has a value of 0 we check the next action
            if(baseAction.GetBestEnemyAiAction().actionValue == 0)
            {
                continue;
            }
            if(bestEnemyAiAction == null)
            {
                bestEnemyAiAction = baseAction.GetBestEnemyAiAction();
                bestBaseAction = baseAction;
            }else
            {
                EnemyAiAction testEnemyAiAction = baseAction.GetBestEnemyAiAction();
                if(testEnemyAiAction != null && testEnemyAiAction.actionValue > bestEnemyAiAction.actionValue)
                {
                    bestEnemyAiAction = testEnemyAiAction;
                    bestBaseAction = baseAction;
                }
            }
         
        }
        //if we have a viable Ai action and we can spend the action points to perform it, we do
        if(bestEnemyAiAction != null && enemyUnit.TrySpendPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAiAction.gridPosition, onEnemyAiActionComplete);
            return true;
        /// otherwise we tell the system that this unit has no more viable actions to take
        }else
        {
            return false;
        }
    }
}
