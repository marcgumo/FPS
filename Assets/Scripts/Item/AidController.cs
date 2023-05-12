using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AidController : MonoBehaviour, iTakeItem
{
    void iTakeItem.TakeItem()
    {
        //GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>().TakeDamage(-20, "Player");
        Destroy(gameObject);
    }
}
