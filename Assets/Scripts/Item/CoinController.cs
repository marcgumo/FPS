using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour, iTakeItem
{
    void iTakeItem.TakeItem()
    {
        //GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>().UpdateTotalCoins();
        Destroy(gameObject);
    }
}
