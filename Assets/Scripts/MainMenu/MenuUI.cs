using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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





    [System.Serializable]
    public class MenuButton
    {
        public Button button;
        public Animator animator;
        public string animationTrigger = "Pressed";
    }

    [Header("Generic Buttons")]
    [SerializeField] private List<MenuButton> genericButtons;

    [Header("Special Buttons")]
    [SerializeField] private Button exitButton;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            string message = GameManager.Instance.GetMessage();
            if (!string.IsNullOrEmpty(message))
            {
                feedbackText.text = message;
            }
        }
            


        foreach (var menuButton in genericButtons)
        {
            if (menuButton.button != null && menuButton.animator != null)
            {
                menuButton.button.onClick.AddListener(() => HandleGenericButtonPress(menuButton));
            }
            else
            {
                Debug.LogWarning("Button or Animator is null in generic buttons list!");
            }
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(HandleExitButtonPress);
        }
        else
        {
            Debug.LogWarning("Exit button is not assigned!");
        }
    }

    private void HandleGenericButtonPress(MenuButton menuButton)
    {
        menuButton.animator.SetTrigger(menuButton.animationTrigger);
        Debug.Log($"Button pressed: {menuButton.button.name}");
    }

    private void HandleExitButtonPress()
    {

        Debug.Log("Closing Game (In Editor)");

        Application.Quit();

    }

    public void AddGenericButton(Button button, Animator animator, string animationTrigger = "Pressed")
    {
        genericButtons.Add(new MenuButton
        {
            button = button,
            animator = animator,
            animationTrigger = animationTrigger
        });

        button.onClick.AddListener(() => HandleGenericButtonPress(genericButtons[genericButtons.Count - 1]));
    }

    public void RemoveGenericButton(Button button)
    {
        genericButtons.RemoveAll(mb => mb.button == button);
    }
}
