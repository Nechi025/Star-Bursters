using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ButtonSelected : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button characterButton;
    [SerializeField] private Animator buttonAnimator;

    private static ButtonSelected localPlayerSelectedButton;

    private bool isSelected = false;

    private void Start()
    {
        characterButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (isSelected)
        {
            photonView.RPC("DeselectButton", RpcTarget.AllBuffered);
        }
        else
        {
            if (localPlayerSelectedButton != null)
            {
                localPlayerSelectedButton.photonView.RPC("DeselectButton", RpcTarget.AllBuffered);
            }

            photonView.RPC("SelectButton", RpcTarget.AllBuffered);
            localPlayerSelectedButton = this;
        }
    }

    [PunRPC]
    private void SelectButton()
    {
        isSelected = true;
        characterButton.interactable = false;
        buttonAnimator.SetTrigger("Select");
    }

    [PunRPC]
    private void DeselectButton()
    {
        isSelected = false;
        characterButton.interactable = true;
        buttonAnimator.SetTrigger("Deselect");
    }
}

