using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : Singleton<GameController>
{
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private UIController UIController;

    GameObject pManager;

    public UIController UIControllerInstance { get { return UIController; } }
    public PlayerManager PlayerManagerInstance { get { return pManager.GetComponent<PlayerManager>(); } }

    void Start()
    {
        
    }

    private void OnEnable()
    {
        if(PhotonNetwork.IsConnected)
        {
            pManager = PhotonNetwork.Instantiate(playerManager.name, transform.position, Quaternion.identity);
        }
    }
}
