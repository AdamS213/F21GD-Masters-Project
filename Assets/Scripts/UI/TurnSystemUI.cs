using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnCounter;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UpdateEndTurnButtonVisibility();
        UpdateTurnText();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEndTurnButtonVisibility();
    }
    private void UpdateTurnText()
    {
        turnCounter.text = "Turn " + TurnSystem.Instance.GetTurnNumber();
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
