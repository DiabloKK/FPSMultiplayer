﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    private void Start()
    {
        // PlayerPrefs là một cơ chế lưu trữ đơn giản trong Unity Engine
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = usernameInput.text;
        } else
        {
            usernameInput.text = "Player " + Random.Range(0, 10000).ToString("00000");
            OnUserNameInputValueChanged();
        }
    }

    public void OnUserNameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.text;
        PlayerPrefs.SetString("username", usernameInput.text);
    }
}
