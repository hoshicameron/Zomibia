using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public EventHandler<OnReloadEventArgs> OnMagazineReloaded_NotFull;
    public EventHandler<OnReloadEventArgs> OnMagazineReload_Full;
    public EventHandler<OnReloadEventArgs> OnAmmoAdded;


    public class OnReloadEventArgs : EventArgs
    {
        public int ammoCount;
        public int magazineCount;
    }

    public EventHandler<OnAmmoSpentEventArgs> OnAmmoSpent;
    public class OnAmmoSpentEventArgs:EventArgs
    {
        public int magazineCount;
    }

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
        OnAmmoAdded?.Invoke(this,new OnReloadEventArgs
        {
            ammoCount = ammo,
            magazineCount = magazine
        });
    }

    public int GetAmmoAmount()
    {
        return ammo;
    }

    public int GetMagazineAmount()
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
        OnAmmoSpent?.Invoke(this, new OnAmmoSpentEventArgs
        {
            magazineCount = magazine
        });
        if (magazine == 0 && ammo>0)
        {
            ReFillMagazine();
        }
    }

    public void ReFillMagazine()
    {
        if (IsMagazineFull())
        {
            OnMagazineReload_Full?.Invoke(this,new OnReloadEventArgs());
            return;
        }
        if (ammo >= magazineMaxAmmoCapacity)
        {
            ammo -= (magazineMaxAmmoCapacity - magazine);
            magazine = magazineMaxAmmoCapacity;
            OnMagazineReloaded_NotFull?.Invoke(this, new OnReloadEventArgs
            {
                ammoCount = ammo,
                magazineCount = magazine
            });

        } else if (ammo<magazineMaxAmmoCapacity && ammo>0)
        {
            magazine = ammo;
            ammo = 0;
            OnMagazineReloaded_NotFull?.Invoke(this, new OnReloadEventArgs
            {
                ammoCount = ammo,
                magazineCount =magazine
            });
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
