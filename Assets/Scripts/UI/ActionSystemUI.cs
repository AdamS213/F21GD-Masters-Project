using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ActionSystemUI : MonoBehaviour
{
    Unit player;
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIs;

    private void Awake()
    {
        actionButtonUIs = new List<ActionButtonUI>();
        
    }
    private void Start()
    {
        createActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();

        GameManager.Instance.OnSelectedActionChange += ActionSystem_OnSelectedActionChanged;
        GameManager.Instance.OnActionStarted += ActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
    }
    private void createActionButtons()
    {
        player = GameManager.Instance.GetPlayer();
        foreach(BaseAction baseAction in player.GetActions())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUIs.Add(actionButtonUI);
            actionButtonUI.SetBaseAction(baseAction);
        }
    }

    private void ActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void ActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
    private void UpdateSelectedVisual()
    {
        foreach(ActionButtonUI actionButtonUI in actionButtonUIs)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {

        actionPointsText.text = " Action Points: " + GameManager.Instance.GetPlayer().GetActionPoints();
    }
}
