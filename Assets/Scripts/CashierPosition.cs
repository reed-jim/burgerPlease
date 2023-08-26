using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierPosition : MonoBehaviour
{
    public Simulator simulator;
    public PlayerController playerController;

    public GameObject player;

    private MaterialPropertyBlock propertyBlock;
    private MeshRenderer meshRenderer;
    private Color initialColor;

    bool isInside = false;
    bool isSetColor = false;
    Vector3 size;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();

        initialColor = meshRenderer.material.color;

        size = meshRenderer.bounds.size;
    }

    /*   private void OnTriggerExit(Collider other)
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
       }*/

    void OnInside()
    {
        if (!isSetColor)
        {
            propertyBlock.SetColor("_BaseColor", initialColor + new Color(40 / 255f, 40 / 255f, 40 / 255f));
            meshRenderer.SetPropertyBlock(propertyBlock);

            isSetColor = true;
        }

        if (playerController.playerState == PlayerState.Ready)
        {
            for (int i = 0; i < simulator.customers.Length; i++)
            {
                if (simulator.customerStates[i] == CustomerState.FirstQueue
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

    void OnOutside()
    {
        if (!isSetColor)
        {
            propertyBlock.SetColor("_BaseColor", initialColor);
            meshRenderer.SetPropertyBlock(propertyBlock);

            isSetColor = true;
        }
    }

    public IEnumerator CheckCashier()
    {
        while (true)
        {
          /*  Debug.Log("d " + Mathf.Abs(player.transform.position.x - transform.position.x));*/
            if (Mathf.Abs(player.transform.position.x - transform.position.x) < size.x / 2
                && Mathf.Abs(player.transform.position.z - transform.position.z) < size.z / 2)
            {
                if (!isInside)
                {
                    isSetColor = false;
                }

                OnInside();

                isInside = true;
            }

            if (Mathf.Abs(player.transform.position.x - transform.position.x) > size.x / 2 + 1
                || Mathf.Abs(player.transform.position.z - transform.position.z) > size.z / 2 + 1)
            {
                if (isInside)
                {
                    isSetColor = false;
                }

                OnOutside();

                isInside = false;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}
