using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandAdjuster : MonoBehaviour
{
    [Header("Hand Settings")]
    [SerializeField] private Transform leftHandHoldPosition;
    [SerializeField] private Transform leftHandBone;

    private void LateUpdate()
    {
        if (!GetComponentInParent<PhotonView>().IsMine)
        {
            return;
        }

        leftHandBone.transform.position = leftHandHoldPosition.position;
    }
}
