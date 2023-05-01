using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private GameObject crossHair;

    public void SwitchCrossHair(bool value)
    {
        crossHair.SetActive(value);
    }
}
