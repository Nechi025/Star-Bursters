using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public string Message;
    private PhotonView pv;

    public int playersAlive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

        /*if (playersAlive <= 0)
       {
           TriggerDefeat();
       }
        */
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
       //UIManager.Instance.ShowDefeatScreen();
       //StartCoroutine(GoToMenuAfterDelay(3f));
   }

   
    }

