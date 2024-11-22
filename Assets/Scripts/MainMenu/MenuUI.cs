using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMPro.TMP_InputField createInput;
    [SerializeField] private TMPro.TMP_InputField joinInput;
    [SerializeField] private TMPro.TMP_Text feedbackText; //Texto para mostrar mensajes de error o feedback


    private int MaxRoomNameLength = 8;
    private int MinRoomNameLength = 1;

    private void Awake()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
    }

    private void CreateRoom()
    {
        string roomName = createInput.text;

        //Verificar longitud del nombre de la sala
        if (roomName.Length < MinRoomNameLength || roomName.Length > MaxRoomNameLength)
        {
            feedbackText.text = $"El nombre de la sala debe tener entre {MinRoomNameLength} a {MaxRoomNameLength} caracteres.";
            return;
        }

        RoomOptions roomConfiguration = new RoomOptions();
        roomConfiguration.MaxPlayers = 3;
        PhotonNetwork.CreateRoom(createInput.text, roomConfiguration);
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Gameplay");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        feedbackText.text = $"Error al unirse: {message}.";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameIdAlreadyExists)
        {
            feedbackText.text = "Ya existe una sala con este nombre. Elige otro.";
        }
        else
        {
            feedbackText.text = $"Error al crear la sala: {message}";
        }
    }

}