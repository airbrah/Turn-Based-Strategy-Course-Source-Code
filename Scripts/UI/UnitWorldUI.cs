using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image healthBackgroundImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        UpdateActionPointsText();
        UpdateHealthBar();
    }
    
    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }
    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void HideActionPointAndHealthBar()
    {
        if (!healthSystem.isDead)
        {
            actionPointsText.canvasRenderer.SetAlpha(0f);
            healthBarImage.canvasRenderer.SetAlpha(0f);
            healthBackgroundImage.canvasRenderer.SetAlpha(0f);
        }
    }
    private void ShowActionPointAndHealthBar()
    {
        if(!healthSystem.isDead)
        {
            actionPointsText.canvasRenderer.SetAlpha(1f);
            healthBarImage.canvasRenderer.SetAlpha(1f);
            healthBackgroundImage.canvasRenderer.SetAlpha(1f);
        }
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionPointAndHealthBar();
                break;
        }
    }
    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                ShowActionPointAndHealthBar();
                break;
        }
    }
}
