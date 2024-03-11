using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;
    private DamageWorldUI damageWorldUI;
    [SerializeField] private Transform DamagePopUpPrefab;
    [SerializeField] private float health = 120f;
    private float healthMAX;
    public bool isDead = false;

    // grab damage value from passthrough Damage method
    private float currentDamage; 

    public void Awake()
    {
        this.OnDamaged += HealthSystem_OnDamaged;
        healthMAX = health;
    }

    public void Damage(float totalDamageAmount)
    {
        currentDamage = totalDamageAmount;
        health -= totalDamageAmount;

        if(health < 0)
        {
            health = 0;
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (health == 0)
        {
            isDead = true;
            Die();
        }
    }
    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }
    public float GetHealthNormalized()
    {
        return health / healthMAX;
    }
    public float GetCurrentDamage()
    {
        return currentDamage;
    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        //make offset pos to put prefab popup about target unit's head
        Vector3 offsetPos = transform.position;
        offsetPos.y = transform.position.y + 2;
        //make damagepopUP
        Transform damagePopUpTransform = Instantiate(DamagePopUpPrefab, offsetPos, Quaternion.identity);
        //get its script
        damageWorldUI = damagePopUpTransform.GetComponent<DamageWorldUI>();
        //pump damage value into script
        damageWorldUI.Setup(currentDamage);
    }
}
