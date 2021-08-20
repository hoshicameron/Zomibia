using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
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
        if(health==0)
            AudioManager.PlayDeathAudio();
        print(health);
    }


}
