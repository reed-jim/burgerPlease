using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public Util util;
    public Simulator simulator;
    public PlayerController playerController;
    public UI_Manager uiManager;

    public int money = 5000;
    public int[] stoveCapacities;

    public int[] playerUpgradeCosts;
    public int[] staffUpgradeCosts;

    // Start is called before the first frame update
    void Start()
    {
        stoveCapacities = new int[simulator.stoves.Length];
        playerUpgradeCosts = new int[3];
        staffUpgradeCosts = new int[3];

        for (int i = 0; i < stoveCapacities.Length; i++)
        {
            stoveCapacities[i] = 6;
        }

        for (int i = 0; i < playerUpgradeCosts.Length; i++)
        {
            playerUpgradeCosts[i] = 200;
        }

        for (int i = 0; i < staffUpgradeCosts.Length - 1; i++)
        {
            staffUpgradeCosts[i] = 200;
        }

        staffUpgradeCosts[2] = 500;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetMoney()
    {
        return money;
    }

    public void UpgradePlayerMoveSpeed()
    {
        playerController.speed++;
    }

    public void UpgradePlayerCapicity()
    {
        playerController.capacity++;
    }

    public void UpgradePlayerProfit()
    {
        playerController.profitMultiplier *= 1.1f;
    }

    public void OnSpendingInGameMoney(int amount)
    {
        money -= amount;
        uiManager.SetMainMoneyUI();
    }
}
