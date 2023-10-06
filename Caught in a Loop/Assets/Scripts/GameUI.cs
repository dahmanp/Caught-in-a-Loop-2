using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI winText;
    public Image AwinBackground;
    public Image MwinBackground;

    public static int UIid;

    public PlayerController[] players;

    public GameObject LowHealthUI;

    private PlayerController player;

    // instance
    public static GameUI instance;

    void Awake()
    {
        instance = this;
    }

    public void Initialize(PlayerController localPlayer)
    {
        player = localPlayer;
        healthBar.maxValue = player.maxHp;
        healthBar.value = player.curHp;

        UpdatePlayerInfoText();
        UpdateAmmoText();
    }

    public void UpdateHealthBar()
    {
        healthBar.value = player.curHp;

    }

    private void Update()
    {
        if (healthBar.value <= 25)
        {
            LowHealthUI.SetActive(true);
        } else
        {
            LowHealthUI.SetActive(false);
        }
    }

    public void UpdatePlayerInfoText()
    {
        if (GameManager.instance.alivePlayers < 10)
        {
            playerInfoText.text = GameManager.instance.alivePlayers + "      " + player.kills;
        } else
        {
            playerInfoText.text = GameManager.instance.alivePlayers + "    " + player.kills;
        }
    }

    public void UpdateAmmoText()
    {
        ammoText.text = player.weapon.curAmmo + "/" + player.weapon.maxAmmo;
    }

    public void SetWinText(string winnerName)
    {
        MwinBackground.gameObject.SetActive(true);
        winText.text = winnerName + " was the last one standing.";
        winText.gameObject.SetActive(true);
        /*if (UIid % 2 == 0)
        {
            MwinBackground.gameObject.SetActive(true);
            winText.text = winnerName + " was the last one standing.";
        } else
        {
            AwinBackground.gameObject.SetActive(true);
            winText.text = winnerName + " was the last one standing.";
        } */
    }
}