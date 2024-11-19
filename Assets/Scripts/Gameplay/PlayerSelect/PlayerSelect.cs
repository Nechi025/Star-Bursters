using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelect : MonoBehaviour
{
    private Dictionary<characters, int> selectedCharacters = new Dictionary<characters, int>();
    private PhotonView pv;
    [SerializeField] GameObject playerSelectCanvas;
    [SerializeField] EnemySpawner spawner;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Ghost()
    {
        SelectCharacter(characters.ghost);
    }
    public void Healer()
    {
        SelectCharacter(characters.healer);
    }
    public void Tank()
    {
        SelectCharacter(characters.tank);
    }
    public void Recon()
    {
        SelectCharacter(characters.recon);
    }
    public void LeaveRoom()
    {

    }

    public void StartGame()
    {
        // Solo el host puede verificar e iniciar el juego
        if (PhotonNetwork.IsMasterClient)
        {
            // Verifica si todos los jugadores seleccionaron un personaje
            if (AllPlayersSelected())
            {
                Debug.Log("Todos los jugadores han seleccionado. Iniciando el juego...");
                pv.RPC("BeginGame", RpcTarget.All);
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
        // Compara el número de selecciones con el número de jugadores en la sala
        return selectedCharacters.Count == PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void SelectCharacter(characters character)
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;

        // Solo permite seleccionar si el personaje no está tomado
        if (!selectedCharacters.ContainsKey(character))
        {
            // Sincroniza la selección con todos los jugadores
            pv.RPC("ConfirmSelection", RpcTarget.AllBuffered, character, playerID);
        }
        else
        {
            Debug.Log($"El personaje {character} ya está seleccionado por otro jugador.");
        }
    }


    [PunRPC]
    private void ConfirmSelection(characters character, int playerID)
    {
        // Verifica si el personaje ya está tomado
        if (!selectedCharacters.ContainsKey(character))
        {
            selectedCharacters[character] = playerID;

            // Si este es el jugador local, llama a SpawnCharacter
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
            {
                PlayerSpawn playerSpawn = FindObjectOfType<PlayerSpawn>();
                playerSpawn.SpawnCharacter(character);
            }
        }
    }

    [PunRPC]
    private void BeginGame()
    {
        playerSelectCanvas.SetActive(false);
    }
}
