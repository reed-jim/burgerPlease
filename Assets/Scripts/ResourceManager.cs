using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public Util util;
    public Simulator simulator;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetMoney()
    {
        return money;
    }
}
