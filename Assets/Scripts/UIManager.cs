using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI magazineText;
    [SerializeField] private GameObject bloodUIParent;
    [SerializeField] private GameObject bloodUI;

    private float canvasHeight, canvasWidth;

    private void Start()
    {
        GameObject player=GameManager.Instance.Player;
        player.GetComponent<Health>().OnGetDamage+=Health_OnGetDamage;
        player.GetComponent<Ammo>().OnMagazineReloaded_NotFull+=Ammo_OnMagazineReloadedNotFull;
        player.GetComponent<Ammo>().OnAmmoSpent+=Ammo_OnAmmoSpent;
        player.GetComponent<Ammo>().OnAmmoAdded+=Ammo_OnAmmoAdded;

        healthBar.value = player.GetComponent<Health>().GetHealthAmount();
        ammoText.SetText(player.GetComponent<Ammo>().GetAmmoAmount().ToString());
        magazineText.SetText(player.GetComponent<Ammo>().GetMagazineAmount().ToString());

        canvasHeight = GetComponent<RectTransform>().rect.height;
        canvasWidth= GetComponent<RectTransform>().rect.width;
    }

    private void Ammo_OnAmmoAdded(object sender, Ammo.OnReloadEventArgs e)
    {
        ammoText.SetText(e.ammoCount.ToString());
        magazineText.SetText(e.magazineCount.ToString());
    }

    private void Ammo_OnAmmoSpent(object sender, Ammo.OnAmmoSpentEventArgs e)
    {
        magazineText.SetText(e.magazineCount.ToString());
    }

    private void Ammo_OnMagazineReloadedNotFull(object sender, Ammo.OnReloadEventArgs e)
    {
        ammoText.SetText(e.ammoCount.ToString());
        magazineText.SetText(e.magazineCount.ToString());
    }

    private void Health_OnGetDamage(object sender, Health.OnGetDamageEventArgs e)
    {
        healthBar.value = e.healthAmount;
        var bUI=Instantiate(bloodUI, bloodUIParent.transform);
        bUI.transform.position=new Vector3(Random.Range(0,canvasWidth),Random.Range(0,canvasHeight),0);

    }
}
