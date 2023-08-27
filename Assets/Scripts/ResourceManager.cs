using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public Util util;
    public Simulator simulator;
    public PlayerController playerController;

    public int money = 5000;
    public int[] stoveCapacities;

    // Start is called before the first frame update
    void Start()
    {
        stoveCapacities = new int[simulator.stoves.Length];

        for (int i = 0; i < stoveCapacities.Length; i++)
        {
            stoveCapacities[i] = 6;
        }

        simulator.moneyTMP.text = "$" + util.ToShortFormNumber(money);
        simulator.moneyTMP.rectTransform.sizeDelta =
                       new Vector2(simulator.moneyTMP.preferredWidth, simulator.moneyTMP.preferredHeight);
        simulator.moneyBackground.sizeDelta =
            new Vector2(1.1f * simulator.moneyTMP.preferredWidth, 1.1f * simulator.moneyTMP.preferredHeight);
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
        playerController.speed *= 1.1f;
    }

    public void UpgradePlayerCapicity()
    {
        playerController.capacity++;
    }

    public void UpgradePlayerProfit()
    {
        playerController.profitMultiplier *= 1.1f;
    }
}
