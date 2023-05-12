using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField createInput;
    [SerializeField] private TMP_InputField joinInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorText;

    void Start()
    {
        errorText.text = "";
    }

    public void CreateRoom()
    {
        //if name input is empty, dont create room
        if (nameInput.text == "")
        {
            errorText.text = "Please enter a name";
            return;
        }

        errorText.text = "";
        PhotonNetwork.NickName = nameInput.text;

        //if create input is empty, dont join room
        if (createInput.text == "")
        {
            errorText.text = "Please enter a room name";
            return;
        }

        errorText.text = "";
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        //if name input is empty, dont join room
        if (nameInput.text == "")
        {
            errorText.text = "Please enter a name";
            return;
        }

        errorText.text = "";
        PhotonNetwork.NickName = nameInput.text;

        //if join input is empty, dont join room
        if (joinInput.text == "")
        {
            errorText.text = "Please enter a room name";
            return;
        }

        errorText.text = "";
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

}
