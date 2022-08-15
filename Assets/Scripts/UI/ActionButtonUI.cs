using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedVisual;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshPro.text = this.baseAction.GetActionName().ToUpper();
        button.onClick.AddListener(() =>
        {
            GameManager.Instance.SetSelectedAction(this.baseAction);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = GameManager.Instance.GetSelectedAction();
        selectedVisual.SetActive(selectedBaseAction == baseAction);
    }
}
