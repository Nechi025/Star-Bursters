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

    public Animator abilityAnim; //Animacion de habilidades
    private string currentStateA;

    const string abilityStart = "Start";

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
            HandleInput();
            UpdateHealthBar();
        }

        if (Health <= 0)
        {
            Destroy(this.gameObject);
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
        if (Input.GetKey(KeyCode.Space) && Time.time >= timeNextShoot)
        {
            Shoot();
            timeNextShoot = Time.time + attackCooldown;
        }
        if (Input.GetKeyDown(KeyCode.E) && cdAbilitie <= 0f)
        {
            pv.RPC("UseAbilitie", RpcTarget.All);
            cdAbilitie = abilitieCooldown;
            ChangeAnimationState(abilityStart);
        }

        if (cdAbilitie > 0f)
        {
            cdAbilitie -= Time.deltaTime;
        }
    }


    void ChangeAnimationState(string newStateA)
    {
        if (currentStateA == newStateA) return;
        abilityAnim.Play(newStateA);
        currentStateA = newStateA;
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

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet != null && bullet.CompareTag("EnemyShot"))
        {
            TakeDamage(bullet.damage);
            Destroy(bullet.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        if (pv.IsMine)
        {
            Health -= damage;
            pv.RPC("SyncHealth", RpcTarget.All, Health);
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
