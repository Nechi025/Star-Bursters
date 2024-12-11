using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using ExitGames.Client.Photon;

[System.Serializable]
public class Wave
{
    public List<EnemyConfig> enemies;
    public float timeBetweenEnemies;
}

[System.Serializable]
public class EnemyConfig
{
    public StraightShooterFactory factory;
    public int amount;                    
}

public class EnemySpawner : MonoBehaviour, IInRoomCallbacks
{
    [SerializeField] List<Wave> waves;     
    [SerializeField] Wave bonusWave;         
    [SerializeField] float timeBetweenWaves;   
    [SerializeField] float timeToStartSpawning;
    [SerializeField] TextMeshProUGUI waveText;

    private PhotonView photonView;
    private float timer;
    private int waveCount = 0; 
    private bool spawningWave = false;
    public bool gameStart = false;


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        timer = 0;
        UpdateWaveText();
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 1 && PhotonNetwork.IsMasterClient && gameStart)
        {
            timer += Time.deltaTime;

            if (!spawningWave && timer >= timeToStartSpawning)
            {
                if (waveCount == 9)
                {
                
                    StartCoroutine(SpawnWave(bonusWave));
                    waveCount = 0;
                }
                else
                {
                   
                    int randomWaveIndex = Random.Range(0, waves.Count);
                    StartCoroutine(SpawnWave(waves[randomWaveIndex]));
                    waveCount++;
                }
                UpdateWaveText();
                timer = 0;
            }
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        spawningWave = true;

        foreach (var enemyConfig in wave.enemies)
        {
            for (int i = 0; i < enemyConfig.amount; i++)
            {
                enemyConfig.factory.CreateEnemy();
                yield return new WaitForSeconds(wave.timeBetweenEnemies);
            }
        }

        yield return new WaitForSeconds(timeBetweenWaves);
        spawningWave = false;
    }

    private void UpdateWaveText()
    {
        if (PhotonNetwork.IsMasterClient)
        {
           
            photonView.RPC("UpdateWaveTextRPC", RpcTarget.AllBuffered, waveCount + 1);
        }
    }

    [PunRPC]
    private void UpdateWaveTextRPC(int waveNumber)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave    {waveNumber}";
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Método que realmente necesitas
    public void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("MasterClient has switched!");

        // Verifica si el nuevo MasterClient es null (puede ocurrir en desconexiones)
        if (newMasterClient == null || PhotonNetwork.PlayerList.Length <= 1)
        {
            NotifyPlayersAndCloseRoom();
        }
    }

    private void NotifyPlayersAndCloseRoom()
    {
        GameManager.Instance.SetMessage("La sala se cerró debido a la desconexión del MasterClient.");

        // Regresar al menú principal
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }
    public void OnPlayerEnteredRoom(Player newPlayer) { }

    public void OnPlayerLeftRoom(Player otherPlayer) { }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) { }
}
