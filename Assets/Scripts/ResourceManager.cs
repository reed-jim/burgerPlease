using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public TMP_Text moneyTMP;

    public int money = 1000;

    // Start is called before the first frame update
    void Start()
    {
        moneyTMP.text = "$" + money; 
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
