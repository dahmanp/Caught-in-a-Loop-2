using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Components")]
    public Rigidbody rig;

    public int id;
    public Player photonPlayer;

    public GameObject PlayerMesh;
    public GameObject GunMesh;
    public GameObject shield;

    public bool team; // marenox is false aptiostell is true
    public Material marenox;
    public Material aptiostell;

    public int curHp;
    public int maxHp;
    public int kills;
    public bool dead;
    private bool flashingDamage;
    public MeshRenderer mr;
    public PlayerWeapon weapon;

    private int curAttackerId;

    [SerializeField] private TextMeshProUGUI Nametag;

    void Update()
    {
        if (!photonView.IsMine || dead)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        Move();

        if (Input.GetMouseButtonDown(0))
            weapon.TryShoot();
    }

    void Move()
    {
        // get the input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // calculate a direction relative to where we're facing
        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.velocity.y;
        // set that as our velocity
        rig.velocity = dir;
    }

    void SetName()
    {
        Nametag.text = photonView.Owner.NickName;
    }

    void TryJump()
    {
        // create a ray facing down
        Ray ray = new Ray(transform.position, Vector3.down);
        // shoot the raycast
        if (Physics.Raycast(ray, 0.8f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;

        if (id % 2 == 0)
        {
            team = false;
            this.gameObject.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = marenox;
        }
        else
        {
            team = true;
            this.gameObject.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = aptiostell;
        }

        // is this not our local player?
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
        else
        {
            GameUI.instance.Initialize(this);
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        if (dead)
            return;
        curHp -= damage;
        curAttackerId = attackerId;
        // flash the player red
        photonView.RPC("DamageFlash", RpcTarget.Others);
        // update the health bar UI
        GameUI.instance.UpdateHealthBar();
        // die if no health left
        if (curHp <= 0)
            photonView.RPC("Die", RpcTarget.All);
    }

    [PunRPC]
    void DamageFlash()
    {
        if (flashingDamage)
            return;
        StartCoroutine(DamageFlashCoRoutine());
        IEnumerator DamageFlashCoRoutine()
        {
            flashingDamage = true;
            Color defaultColor = mr.material.color;
            mr.material.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            mr.material.color = defaultColor;
            flashingDamage = false;
        }
    }

    [PunRPC]
    void Die()
    {
        curHp = 0;
        dead = true;
        GameManager.instance.alivePlayers--;
        // host will check win condition
        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();
        // is this our local player?
        if (photonView.IsMine)
        {
            if (curAttackerId != 0)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);
            // set the cam to spectator
            GetComponentInChildren<CameraController>().SetAsSpectator();
            // disable the physics and hide the player
            rig.isKinematic = true;
            transform.position = new Vector3(0, -50, 0);
        }
    }

    [PunRPC]
    public void AddKill()
    {
        kills++;
        GameUI.instance.UpdatePlayerInfoText();
    }

    [PunRPC]
    public void Heal(int amountToHeal)
    {
        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHp);
        // update the health bar UI
        GameUI.instance.UpdateHealthBar();
    }

    [PunRPC]
    public void Shield(int duration)
    {
        photonView.RPC("Shield", RpcTarget.Others);
    }

    void addshield()
    {
        shield.gameObject.SetActive(true);
        Invoke("removeshield", 20.0f);
    }

    void removeshield()
    {
        shield.gameObject.SetActive(false);
    }

    [PunRPC]
    public void Zoom(int amountToZoom)
    {
        moveSpeed = moveSpeed + amountToZoom;
        Invoke("removeshield", 10.0f);
    }

    void removeZoom(int amountToZoom)
    {
        moveSpeed = moveSpeed - amountToZoom;
    }

    [PunRPC]
    public void Invisible(int duration)
    {
        PlayerMesh.gameObject.SetActive(false);
        GunMesh.gameObject.SetActive(false);
        Invoke("makeVisible", duration);
    }

    void makeVisible()
    {
        PlayerMesh.gameObject.SetActive(true);
        GunMesh.gameObject.SetActive(true);
    }

    [PunRPC]
    public void XRay(int duration)
    {
        if (photonView.IsMine)
        {
            //Map.gameObject.GetComponent<MeshRenderer>().material = Invis;
        }
        //Map.gameObject.GetComponent<MeshRenderer>().material = Invis;
        //Map.gameObject.GetComponent<MeshRenderer>().material = Default;
    }
}
