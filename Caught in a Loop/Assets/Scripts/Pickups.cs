using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Health,
    Ammo,
    Invisibility,
    Zoom,
    Shield,
    XRay
}

public class Pickups : MonoBehaviourPun
{
    public PickupType type;
    public int value;

    private void OnTriggerEnter(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient)
            return;

        if(other.CompareTag("Player"))
        {
            // get the player
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);

            if (type == PickupType.Health)
                player.photonView.RPC("Heal", player.photonPlayer, value);
            else if (type == PickupType.Ammo)
                player.photonView.RPC("GiveAmmo", player.photonPlayer, value);
            else if (type == PickupType.Invisibility)
                player.photonView.RPC("Invisible", player.photonPlayer, value);
            else if (type == PickupType.Zoom)
                player.photonView.RPC("Zoom", player.photonPlayer, value);
            else if (type == PickupType.Shield)
                player.photonView.RPC("Shield", player.photonPlayer, value);
            else if (type == PickupType.XRay)
                player.photonView.RPC("XRay", player.photonPlayer, value);

            // destroy the object
            photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void DestroyPickup()
    {
        Destroy(gameObject);
    }
}
