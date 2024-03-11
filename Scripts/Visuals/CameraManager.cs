using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideActionCamera();
    }
    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    } //Action camera here is more like shooter camera. active at start of action
    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    } //Action camera here is more like shooter camera. active at end of action
    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch(sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                Vector3 shootDir = (targetUnit.GetWorldPos() - shooterUnit.GetWorldPos()).normalized;

                float shoulderOffsetAmount = 0.5f;

                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

                Vector3 actionCameraPos =
                    shooterUnit.GetWorldPos() + 
                    cameraCharacterHeight + 
                    shoulderOffset + 
                    (shootDir * -1);

                actionCameraGameObject.transform.position = actionCameraPos;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPos() + cameraCharacterHeight);
                ShowActionCamera();
                
                break;
        }
    }
    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }
}
