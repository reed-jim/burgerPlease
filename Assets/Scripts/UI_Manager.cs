using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public TMP_Text moneyTMP;
    public TMP_Text[] moneyTakenTMP;
    public ResourceManager resourceManager;

    // Start is called before the first frame update
    void Start()
    {
        moneyTMP.text = resourceManager.money.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
