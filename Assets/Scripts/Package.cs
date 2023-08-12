using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    public Simulator simulator;
    public Util util;

    public int index;

    // Start is called before the first frame update
    void Start()
    {
        simulator = GameObject.Find("Simulator").GetComponent<Simulator>();
        util = GameObject.Find("Util").GetComponent<Util>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            HumanController npcController = other.GetComponent<HumanController>();

            if (simulator.packageStates[index] == PackageState.Filling &&
                npcController.humanState == HumanState.HoldingFood)
            {
                StartCoroutine(MoveFoodOneByOneToPackage(npcController));

                util.SetHoldingFoodStandingAnimation(npcController.animator);

                npcController.humanState = HumanState.PackagingFood;
            }
            else if (simulator.packageStates[index] == PackageState.Filled &&
                npcController.humanState == HumanState.Moving)
            {
                StartCoroutine(
                    simulator.CurveMove(
                        transform,
                        transform.position,
                        other.gameObject.transform.position
                        + new Vector3(0, 7, 0)
                        + other.gameObject.transform.forward * 4,
                        12,
                        0,
                        () =>
                        {
                            npcController.humanState = HumanState.HoldingFood;
                            simulator.packageBelongTo[index] = npcController.id;
                            simulator.packageStates[index] = PackageState.Moving;
                        }
                    )
                );

                for (int i = 0; i < simulator.foods.Length; i++)
                {
                    if (simulator.foodBelongTo[i] == "package" + index)
                    {
                        simulator.foodStates[i] = FoodState.MovingWithPackage;
                    }
                }

                util.SetHoldingFoodStandingAnimation(npcController.animator);

                npcController.humanState = HumanState.PickingPackage;
            }
        }
    }

    IEnumerator MoveFoodOneByOneToPackage(HumanController npcController)
    {
        int capacity = simulator.numFoodHoldOf(npcController.id);
        int currentMoved = 0;

        for (int i = 0; i < simulator.foods.Length; i++)
        {
            int foodIndex = i;

            if (simulator.foodBelongTo[i] == npcController.id)
            {
                simulator.foodBelongTo[i] = "package" + index;
                simulator.foodIndexInPackage[i] = simulator.numFillOfPackage(index) - 1;
                
                StartCoroutine(
                    simulator.CurveMove(
                        simulator.foods[i].transform,
                        simulator.foods[i].transform.position,
                        simulator.GetFoodPositionInPackage(
                            index,
                            simulator.foodIndexInPackage[i]
                            ),
                        12,
                        0,
                        () =>
                        {
                            simulator.foodStates[foodIndex] = FoodState.InPackage;

                            if (simulator.foodIndexInPackage[foodIndex] == 3)
                            {
                                StartCoroutine(
                                    simulator.CurveMove(
                                        transform,
                                        transform.position,
                                        new Vector3(
                                            simulator.packageTable.transform.position.x
                                            - simulator.packageTableSize.x / 4,
                                            transform.position.y,
                                            simulator.packageTable.transform.position.z
                                            ),
                                        12,
                                        0,
                                        () =>
                                        {
                                            for (int j = 0; j < simulator.foods.Length; j++)
                                            {
                                                if (simulator.foodBelongTo[j] == "package" + index)
                                                {
                                                    simulator.foodStates[j] = FoodState.InPackage;
                                                }
                                            }

                                            simulator.packageStates[index] = PackageState.Filled;
                                        }
                                    )
                                );

                                for (int j = 0; j < simulator.foods.Length; j++)
                                {
                                    if(simulator.foodBelongTo[j] == "package" + index)
                                    {
                                        simulator.foodStates[j] = FoodState.MovingWithPackage;
                                    }
                                }  
                            }
                        }
                    )
                );

                if (simulator.foodIndexInPackage[i] == 3)
                {
                    simulator.packageStates[index] = PackageState.Moving;
                }

                simulator.foodStates[i] = FoodState.Putting;

                currentMoved++;

                if (currentMoved < capacity)
                {
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    npcController.humanState = HumanState.MovedAllFoodToPackage;
                    break;
                }
            }
        }
    }
}
