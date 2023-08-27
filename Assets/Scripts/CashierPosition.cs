using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierPosition : MonoBehaviour
{
    public Simulator simulator;
    public Util util;
    public NPC_Manager npcManager;
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

    IEnumerator OnInside()
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
                    util.SetIdleAnimation(playerController.playerAnimator);

                    yield return new WaitForSeconds(0.2f);

                    player.transform.eulerAngles = new Vector3(0, 180, 0);

                    util.SetCashingAnimation(playerController.playerAnimator);

                    yield return new WaitForSeconds(2f);

                    util.SetCashingFinishAnimation(playerController.playerAnimator);

                    StartCoroutine(util.ScaleUpDownEffect(simulator.counter.transform, 0.15f));

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
        while (!npcManager.npcs[0].activeInHierarchy)
        {
            if (Mathf.Abs(player.transform.position.x - transform.position.x) < 1.1f * size.x / 2
                && Mathf.Abs(player.transform.position.z - transform.position.z) < 1.1f * size.z / 2)
            {
                if (!isInside)
                {
                    isSetColor = false;
                }

                StartCoroutine(OnInside());

                isInside = true;

                yield return new WaitForSeconds(3f);
            }

            if (Mathf.Abs(player.transform.position.x - transform.position.x) > 1.1f * size.x / 2 + 1
                || Mathf.Abs(player.transform.position.z - transform.position.z) > 1.1f * size.z / 2 + 1)
            {
                if (isInside)
                {
                    isSetColor = false;

                    OnOutside();

                    isInside = false;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
