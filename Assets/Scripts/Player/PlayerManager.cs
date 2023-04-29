using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    Transform spawnPoint;

    GameObject playerController;
    PhotonView pView;

    private void Awake()
    {
        pView = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
            spawnPoint = spawnPoints[0];
        else
            spawnPoint = spawnPoints[1];

        if (!pView.IsMine)
            return;

        CreatePlayer();
    }

    private void CreatePlayer()
    {
        playerController = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position,
            Quaternion.identity, 0, new object[] { playerPrefab.GetComponent<PhotonView>().ViewID });
    }

    public void DestroyPlayer()
    {
        PhotonNetwork.Destroy(playerController);
        CreatePlayer();
    }
}
