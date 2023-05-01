using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float timeToDestroy = 2.0f;
    
    void Start()
    {
        Invoke(nameof(DestroyObject), timeToDestroy);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
