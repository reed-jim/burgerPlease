using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public Util util;

    public TMP_Text moneyTMP;

    public int money = 5000;

    // Start is called before the first frame update
    void Start()
    {
        moneyTMP.text = "$" + util.ToShortFormNumber(money); 
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
