using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private GameObject crossHair;

    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI bulletsText;

    public void SwitchCrossHair(bool value)
    {
        crossHair.SetActive(value);
    }

    public void HPBarUpdate(float currentHealth, float maxHealth)
    {
        hpBar.fillAmount = currentHealth / maxHealth;
    }

    public void BulletTextUpdate(int bulletsInRound, int totalBullets)
    {
        bulletsText.text = bulletsInRound.ToString() + " / " + totalBullets.ToString();
    }
}
