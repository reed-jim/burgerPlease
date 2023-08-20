using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierPosition : MonoBehaviour
{
    public Simulator simulator;
    public PlayerController playerController;
    private MaterialPropertyBlock propertyBlock;
    private MeshRenderer meshRenderer;
    private Color initialColor;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();

        initialColor = meshRenderer.material.color;
    }

    private void OnTriggerExit(Collider other)
    {
        propertyBlock.SetColor("_BaseColor", initialColor);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            propertyBlock.SetColor("_BaseColor", initialColor + new Color(40 / 255f, 40 / 255f, 40 / 255f));
            meshRenderer.SetPropertyBlock(propertyBlock);

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
