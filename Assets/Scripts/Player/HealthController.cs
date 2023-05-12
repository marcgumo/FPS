using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviourPun, IPunObservable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject hpBar;

    int currentHealth;
    Image hpBarImage;
    PhotonView pView;

    private void Awake()
    {
        pView = GetComponent<PhotonView>();
        hpBarImage = hpBar.GetComponentInChildren<Image>();
        currentHealth = maxHealth;

        HPBarUpdate();
    }

    void Start()
    {
        GameController.Instance.UIControllerInstance.HPBarUpdate(currentHealth, maxHealth);
    }

    public void HPBarUpdate()
    {
        hpBarImage.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    public void TakeDamage(int damage)
    {
        pView.RPC(nameof(RPC_TakeDamage), pView.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        if (pView.IsMine)
        {
            currentHealth -= damage;

            HPBarUpdate();
            GameController.Instance.UIControllerInstance.HPBarUpdate(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                GameController.Instance.PlayerManagerInstance.DestroyPlayer();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hpBarImage.fillAmount);
            stream.SendNext(currentHealth);
        }
        else
        {
            hpBarImage.fillAmount = (float)stream.ReceiveNext();
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}
