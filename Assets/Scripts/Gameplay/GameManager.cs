using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public string Message;
    private PhotonView pv;

    [SerializeField] private GameObject ScoreScreen;
    public int playersAlive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pv = GetComponent<PhotonView>();
    }

    public void SetMessage(string message)
    {
        Message = message;
    }

    public string GetMessage()
    {
        return Message;
    }
    public void InitializeGame()
    {
        playersAlive = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"Jugadores vivos inicializados: {playersAlive}");
    }
   
    public void PlayerDied()
    {
        //if (!PhotonNetwork.IsMasterClient) return;

        playersAlive--;
        Debug.Log(playersAlive);

        if (playersAlive <= 0)
       {
           TriggerDefeat();
       }
        
    }

    public void PlayerRevived()
    {
        playersAlive++;
        Debug.Log(playersAlive);
    }

    // Sincronizar la derrota con todos los jugadores
    [PunRPC]
   private void TriggerDefeat()
   {
       pv.RPC("HandleDefeat", RpcTarget.All);
   }

   [PunRPC]
   private void HandleDefeat()
   {
       PhotonNetwork.LeaveRoom();
       SceneManager.LoadScene("MainMenu");
        ScoreScreen.SetActive(true);
        //UIManager.Instance.ShowDefeatScreen();
        //StartCoroutine(GoToMenuAfterDelay(3f));
    }

   
    }

