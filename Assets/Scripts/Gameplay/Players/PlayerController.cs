using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private float cdAbilitie = 0f;
    [SerializeField] float abilitieCooldown = 0;
    [SerializeField] float abilitieDuration = 0;
    [SerializeField] Slider healthBar;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject reviveZone;
    [SerializeField] private Slider reviveProgressSlider;


    //Recon
    public bool isRecon = false;
    public ReconLaser laser;

    public bool isDead = false;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        maxHealth = Health;

        if (pv.IsMine)
        {

            healthBar.maxValue = maxHealth;
            healthBar.value = Health;
        }
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (isDead == false)
            {
                HandleInput();
                UpdateHealthBar();
            }
        }
    }

    private void HandleInput()
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
        if (!isRecon)
        {
            if (Input.GetKey(KeyCode.Space) && Time.time >= timeNextShoot)
            {
                Shoot();
                timeNextShoot = Time.time + attackCooldown;
            }
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            Laser();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            laser.TurnOffLaser();
        }

        if (Input.GetKeyDown(KeyCode.E) && cdAbilitie <= 0f)
        {
            pv.RPC("UseAbilitie", RpcTarget.All);
            cdAbilitie = abilitieCooldown;
            animator.SetTrigger("AbilityUse");
        }

        if (cdAbilitie > 0f)
        {
            cdAbilitie -= Time.deltaTime;
        }
    }


    
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = Health;
        }
    }

    private void LateUpdate()
    {

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
            PhotonNetwork.Instantiate(bullet.name, point.position, point.rotation);
        }
    }

    public void Laser()
    {
        laser.ShootLaser(transform);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet != null && bullet.CompareTag("EnemyShot") && isDead == false)
        {
            TakeDamage(bullet.damage);
            Destroy(bullet.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController reviver = collision.GetComponent<PlayerController>();

        if (reviver != null && reviver.isDead == false && isDead == true)
        {
            StartCoroutine(ReviveProcess(reviver));
        }
    }

    public void TakeDamage(float damage)
    {
        if (pv.IsMine)
        {
            Health -= damage;
            pv.RPC("SyncHealth", RpcTarget.All, Health);
        }

        if (Health <= 0)
        {
            isDead = true;
            GameManager.Instance.PlayerDied();
        }
    }

    [PunRPC]
    private void SyncHealth(float updatedHealth)
    {
        Health = updatedHealth;
        UpdateHealthBar();
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
            obj.SetActive(false);
        }
    }




    private IEnumerator ReviveProcess(PlayerController reviver)
    {
        // Debug Revivir
        if (reviveZone == null || reviveProgressSlider == null)
        {
            Debug.LogError("Sin zona o Slider");
            yield break;
        }

        // Activar zona de revivir
        reviveZone.transform.position = this.transform.position;
        reviveZone.SetActive(true);

        // Reseteo de progreso
        reviveProgressSlider.value = 0f;

        float timer = 0f;
        Debug.Log("Reviviendo...");

        while (timer < 3f)
        {
            // Jugador cercano para revivir
            if (!reviveZone.GetComponent<Collider2D>().bounds.Contains(reviver.transform.position))
            {
                Debug.Log("Revivir Interrumpido.");
                yield return null;
                continue; 
            }

            // Actualiza Slider
            reviveProgressSlider.value = timer / 3f;

            timer += Time.deltaTime;
            yield return null;
        }

        // Revivido
        if (pv.IsMine)
        {
            pv.RPC("RevivePlayer", RpcTarget.All);
        }

        Debug.Log("Revivido.");
        reviveZone.SetActive(false);
        GameManager.Instance.PlayerRevived();
    }



    [PunRPC]
    private void RevivePlayer()
    {
        Health = maxHealth / 2; // 50% de vida segun jugador
        isDead = false;

        UpdateHealthBar();
        Debug.Log("Revived player health: " + Health);
    }




    public void Heal(int amount)
    {
        if (pv.IsMine)
        {
            Health += amount;
            Health = Mathf.Min(Health, maxHealth);
            pv.RPC("SyncHealth", RpcTarget.All, Health);
        }
    }
}


