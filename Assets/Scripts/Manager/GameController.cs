using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : Singleton<GameController>
{
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private UIController UIController;

    public UIController UIControllerInstance { get { return UIController; } }

    void Start()
    {
        
    }

    private void OnEnable()
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(playerManager.name, transform.position, Quaternion.identity);
        }
    }
}
