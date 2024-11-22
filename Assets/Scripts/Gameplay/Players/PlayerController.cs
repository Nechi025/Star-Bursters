using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IHealable
{
    private PhotonView pv;
    private float maxHealth;
    public float Health;
    public Vector2 Hrange = Vector2.zero;
    public Vector2 Vrange = Vector2.zero;
    public Bullet bullet;
    public float attackCooldown;
    float timeNextShoot = 0;
    public List<Transform> FiringPoints;
    [SerializeField] GameObject abilitie;
    private float cdAbilitie = 0f; // Tiempo restante para que la habilidad se pueda usar
    [SerializeField] float abilitieCooldown = 0; // Duración total del cooldown
    [SerializeField] float abilitieDuration = 0;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        maxHealth = Health;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.up * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += -Vector3.up * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += -Vector3.right * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.Space) && Time.time >= timeNextShoot)
            {
                Shoot();
                timeNextShoot = Time.time + attackCooldown;
            }
            if (Input.GetKeyDown(KeyCode.E) && cdAbilitie <= 0f)
            {
                pv.RPC("UseAbilitie", RpcTarget.All);
                cdAbilitie = abilitieCooldown;
            }

            // Actualiza el cooldown
            if (cdAbilitie > 0f)
            {
                cdAbilitie -= Time.deltaTime; // Reduce el tiempo restante del cooldown
            }
        }


        if (Health <= 0)
        {
            Destroy(this.gameObject);
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void LateUpdate()
    {
        //Limites del mapa
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, Vrange.x, Vrange.y),
            Mathf.Clamp(transform.position.y, Hrange.x, Hrange.y),
            transform.position.z
            );
    }

    public void Shoot()
    {
        foreach (var point in FiringPoints)
        {
            GameObject go = PhotonNetwork.Instantiate(bullet.name, point.position, point.rotation);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet != null && bullet.CompareTag("EnemyShot"))
        {
            Health -= bullet.damage;
            Destroy(bullet.gameObject);
        }
    }

    [PunRPC]
    private void UseAbilitie()
    {
        abilitie.SetActive(true);

        StartCoroutine(DestroyAfterTime(abilitie, abilitieDuration));
    }

    private IEnumerator DestroyAfterTime(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (obj != null)
        {
            obj.SetActive(false); // Destruir el objeto de la habilidad
        }
    }

    public void Heal(int amount)
    {
        Health += amount;
        Health = Mathf.Min(Health, maxHealth); // Limitar a la salud máxima
        Debug.Log($"Jugador curado. Salud actual: {Health}");
    }
}
