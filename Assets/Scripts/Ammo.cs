using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public event EventHandler OnMagazineReloaded_NotFull;
    public event EventHandler OnMagazineReload_Full;

    [SerializeField] private int magazineMaxAmmoCapacity;
    [SerializeField] private int maxAmmo=300;

    public int ammo;
    public int magazine;
    private void Start()
    {
        ammo = maxAmmo;
        magazine = magazineMaxAmmoCapacity;
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
        ammo = Mathf.Clamp(ammo, 0, maxAmmo);
    }

    public int GetAmmoAmount()
    {
        return ammo;
    }

    public int GetMagazineAmmoAmount()
    {
        return magazine;
    }

    public bool IsMagazineFull()
    {
        return magazine == magazineMaxAmmoCapacity;
    }

    public void SpendAmmo()
    {
        magazine -= 1;
        magazine = Mathf.Clamp(magazine, 0, magazineMaxAmmoCapacity);
        if (magazine == 0 && ammo>0)
        {
            ReFillMagazine();
        }
    }

    public void ReFillMagazine()
    {
        if (IsMagazineFull())
        {
            OnMagazineReload_Full?.Invoke(this,EventArgs.Empty);
            return;
        }
        if (ammo >= magazineMaxAmmoCapacity)
        {
            ammo -= (magazineMaxAmmoCapacity - magazine);
            magazine = magazineMaxAmmoCapacity;
            OnMagazineReloaded_NotFull?.Invoke(this, EventArgs.Empty);

        } else if (ammo<magazineMaxAmmoCapacity && ammo>0)
        {
            magazine = ammo;
            ammo = 0;
            OnMagazineReloaded_NotFull?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanAddAmmo()
    {
        return ammo != maxAmmo;
    }

    public bool IsMagazineEmpty()
    {
        return magazine==0 ;
    }

    public bool IsOutOfAmmo()
    {
        return (ammo + magazine) == 0;
    }
}
