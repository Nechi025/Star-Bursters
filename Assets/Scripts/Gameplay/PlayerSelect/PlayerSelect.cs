using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerSelect : MonoBehaviour
{
    private Dictionary<int, characters> selectedCharacters = new Dictionary<int, characters>(); //ID del jugador y su selección
    private PhotonView pv;
    [SerializeField] GameObject playerSelectCanvas;
    [SerializeField] EnemySpawner spawner;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Ghost()
    {
        ChangeCharacterSelection(characters.ghost);
    }

    public void Healer()
    {
        ChangeCharacterSelection(characters.healer);
    }

    public void Tank()
    {
        ChangeCharacterSelection(characters.tank);
    }

    public void Recon()
    {
        ChangeCharacterSelection(characters.recon);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (AllPlayersSelected())
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;

                Debug.Log("Todos los jugadores han seleccionado. Iniciando el juego...");
                pv.RPC("BeginGame", RpcTarget.AllBuffered);
                spawner.gameStart = true;
            }
            else
            {
                Debug.Log("No todos los jugadores han seleccionado un personaje.");
            }
        }
    }

    private bool AllPlayersSelected()
    {
        return selectedCharacters.Count == PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void ChangeCharacterSelection(characters newCharacter)
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;

        // Verifica si el personaje ya está seleccionado por otro jugador
        if (!selectedCharacters.ContainsValue(newCharacter))
        {
            // Notifica a todos el cambio de selección
            pv.RPC("UpdateCharacterSelection", RpcTarget.AllBuffered, newCharacter, playerID);
        }
        else
        {
            Debug.Log($"El personaje {newCharacter} ya está seleccionado por otro jugador.");
        }
    }

    [PunRPC]
    private void UpdateCharacterSelection(characters newCharacter, int playerID)
    {
        // Libera el personaje previamente seleccionado (si existe)
        if (selectedCharacters.ContainsKey(playerID))
        {
            Debug.Log($"Jugador {playerID} cambió de {selectedCharacters[playerID]} a {newCharacter}.");
            selectedCharacters.Remove(playerID);
        }

        // Asigna el nuevo personaje
        selectedCharacters[playerID] = newCharacter;
    }

    [PunRPC]
    private void BeginGame()
    {
        // Hace el spawn de los personajes seleccionados
        if (selectedCharacters.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber, out characters selectedCharacter))
        {
            PlayerSpawn playerSpawn = FindObjectOfType<PlayerSpawn>();
            playerSpawn.SpawnCharacter(selectedCharacter);
            GameManager.Instance.InitializeGame();
        }

        playerSelectCanvas.SetActive(false);
    }
}