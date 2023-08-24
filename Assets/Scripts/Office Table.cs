using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OfficeTableFunction
{
    HRUpgrade,
    PlayerUpgrade
}

public class OfficeTable : MonoBehaviour
{
    public UI_Manager uiManager;

    public OfficeTableFunction officeTableFunction;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(officeTableFunction == OfficeTableFunction.HRUpgrade)
        {
            uiManager.OpenHRUpgradeScreen();
        }
        else if (officeTableFunction == OfficeTableFunction.PlayerUpgrade)
        {
            uiManager.OpenPlayerUpgradeScreen();
        }
    }
}
