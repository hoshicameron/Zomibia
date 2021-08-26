using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public EventHandler<OnGetDamageEventArgs> OnGetDamage;
    public event EventHandler OnDied;

    public class OnGetDamageEventArgs : EventArgs
    {
        public float healthAmount;
    }

    [SerializeField] private int maxHealth=100;
    private int health=50;

    private void Start()
    {
        health = maxHealth;

    }

    public void AddHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        OnGetDamage?.Invoke(this,new OnGetDamageEventArgs
        {
            healthAmount = health
        });
    }
    public int GetHealthAmount()
    {
        return health;
    }
    public bool CanAddHealth()
    {
        return health != maxHealth;
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        OnGetDamage?.Invoke(this,new OnGetDamageEventArgs
        {
            healthAmount = health
        });
        if (health == 0)
        {
            OnDied?.Invoke(this,EventArgs.Empty);

        }
    }


}
