using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierPosition : MonoBehaviour
{
    public Simulator simulator;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController.playerState == PlayerState.Ready)
            {
                for (int i = 0; i < simulator.customers.Length; i++)
                {
                    if (simulator.customerStates[i] == CustomerState.Waiting
                        && i == simulator.firstQueueCustomerIndex
                        && simulator.numFoodAvailableForCustomer() >= simulator.customerNumFoodDemand[i])
                    {
                        StartCoroutine(simulator.moveFoodOneByOneToCustomer(
                            simulator.customers[i].transform, i
                        ));

                        simulator.customerStates[i] = CustomerState.PickingFood;

                        break;
                    }
                }
            }
        }
    }
}
