using System;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{

    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    //Audio shouldn't be here; TEMP
    [SerializeField] private AudioSource audioSource;

    private GridPos gridPosition;
    private bool isGreen;
    private Action onInteractionComplete;
    private bool isActive;
    private float timer;

    private Loader.Scene _scene;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPos(gridPosition, this);

        SetColorGreen();
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isActive = false;
            onInteractionComplete();
        }
    }

    private void SetColorGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }
    private void SetColorRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;
    }
    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = .5f;

        //Audio shouldn't be here; TEMP
        audioSource.Play();

        if (isGreen)
        {
            SetColorRed();
            
        }
        else
        {
            SetColorGreen();
        }

        if(UnitManager.Instance.GetEnemyUnitList().Count == 0 && !isGreen)
        {
            EndGame();
        }
    }
    public string Name()
    {
        return "InteractSphere";
    }
    public bool IsActive()
    {
        return isActive;
    }
    public void EndGame()
    {
        _scene = Loader.Scene.EndGameScene;
        Loader.Load(_scene);
    }
}
