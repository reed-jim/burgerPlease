using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeawayCounter : MonoBehaviour
{
    public Simulator simulator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("NPC"))
        {
            HumanController npcController = other.GetComponent<HumanController>();

            StartCoroutine(MovePackageOneByOneToTakeawayCounter(npcController));
        }
        else if(other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();

            StartCoroutine(MovePackageOneByOneToTakeawayCounter(controller));
        }
        /* else if(other.CompareTag("Car"))
        {
            int carIndex = 0;

            for (int i = 0; i < simulator.cars.Length; i++)
            {
                if(GameObject.ReferenceEquals(simulator.cars[i], other.gameObject))
                {
                    carIndex = i;

                    break;
                }
            }

            simulator.carStates[carIndex] = CarState.Waiting;
        }*/
    }

    IEnumerator MovePackageOneByOneToTakeawayCounter(HumanController npcController)
    {
        int capacity = 1;
        int currentMoved = 0;

        for (int i = 0; i < simulator.packages.Length; i++)
        {
            int packageIndex = i;

            if (simulator.packageBelongTo[i] == npcController.id)
            {
                simulator.packageBelongTo[i] = "takeawayCounter";

                StartCoroutine(
                    simulator.CurveMove(
                        simulator.packages[i].transform,
                        simulator.packages[i].transform.position,
                        transform.position
                        + new Vector3(0,
                        (simulator.takeawayCounterSize.y + simulator.packageSize.y) / 2,
                        0),
                        12,
                        0,
                        () =>
                        {
                            simulator.packageStates[packageIndex] = PackageState.InTakeawayCounter;   
                        }
                    )
                );

                npcController.ResetProperties();
                simulator.packageStates[i] = PackageState.Putting;

                currentMoved++;
            }

            if (currentMoved < capacity)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator MovePackageOneByOneToTakeawayCounter(PlayerController playerController)
    {
        int capacity = 1;
        int currentMoved = 0;

        for (int i = 0; i < simulator.packages.Length; i++)
        {
            int packageIndex = i;

            if (simulator.packageBelongTo[i] == "player")
            {
                simulator.packageBelongTo[i] = "takeawayCounter";

                StartCoroutine(
                    simulator.CurveMove(
                        simulator.packages[i].transform,
                        simulator.packages[i].transform.position,
                        transform.position
                        + new Vector3(0,
                        (simulator.takeawayCounterSize.y + simulator.packageSize.y) / 2,
                        0),
                        12,
                        0,
                        () =>
                        {
                            simulator.packageStates[packageIndex] = PackageState.InTakeawayCounter;
                        }
                    )
                );

                playerController.ResetProperties();
                simulator.packageStates[i] = PackageState.Putting;

                currentMoved++;
            }

            if (currentMoved < capacity)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
