using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageTable : MonoBehaviour
{
    public Simulator simulator;
    public Util util;

    private bool isContinueCheck;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        isContinueCheck = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.playerState == PlayerState.HoldingFoodMoving)
            {
                int packageIndex = GetFillingPackageIndex();

                if (packageIndex == -1)
                {
                    return;
                }


                StartCoroutine(MoveFoodOneByOneToPackage(playerController, packageIndex));

                util.SetHoldingFoodStandingAnimation(playerController.playerAnimator);

                playerController.playerState = PlayerState.PackagingFood;
            }
            else if (playerController.playerState == PlayerState.Ready)
            {
                if (playerController.gameObject.transform.position.x < transform.position.x)
                {
                    PickingPackage(playerController);
                }
                else
                {
                    isContinueCheck = true;

                    StartCoroutine(ContinueCheck(other.transform, playerController));
                }
            }
        }
    }

    IEnumerator MoveFoodOneByOneToPackage(PlayerController playerController, int packageIndex)
    {
        int capacity = simulator.numFoodHoldOf("player");
        int currentMoved = 0;

        for (int i = 0; i < simulator.foods.Length; i++)
        {
            int foodIndex = i;

            if (simulator.foodBelongTo[i] == "player")
            {
                simulator.foodBelongTo[i] = "package" + packageIndex;
                simulator.foodIndexInPackage[i] = simulator.numFillOfPackage(packageIndex) - 1;

                StartCoroutine(
                    simulator.CurveMove(
                        simulator.foods[i].transform,
                        simulator.foods[i].transform.position,
                        simulator.GetFoodPositionInPackage(
                            packageIndex,
                            simulator.foodIndexInPackage[i]
                            ),
                        12,
                        0,
                        () =>
                        {
                            simulator.foodStates[foodIndex] = FoodState.InPackage;

                            if (simulator.foodIndexInPackage[foodIndex] == 3)
                            {
                                simulator.packageBelongTo[packageIndex] = "package table filled";
                                simulator.packageColumnIndex[packageIndex] = simulator.numPackageBelongTo("package table filled") - 1;

                                StartCoroutine(
                                    simulator.CurveMove(
                                        simulator.packages[packageIndex].transform,
                                        simulator.packages[packageIndex].transform.position,
                                        new Vector3(
                                            transform.position.x
                                            + simulator.packageSize.x / 2 + 0.1f * simulator.packageTableSize.x,
                                            transform.position.y
                                            + simulator.packageTableSize.y + simulator.packageSize.y / 2
                                            + simulator.packageColumnIndex[packageIndex] * simulator.packageSize.y,
                                            transform.position.z + simulator.packageTableSize.z / 2
                                        ),
                                        12,
                                        0,
                                        () =>
                                        {
                                            for (int j = 0; j < simulator.foods.Length; j++)
                                            {
                                                if (simulator.foodBelongTo[j] == "package" + packageIndex)
                                                {
                                                    simulator.foods[j].transform.position =
                                                    simulator.GetFoodPositionInPackage(packageIndex, simulator.foodIndexInPackage[j]);

                                                    simulator.foodStates[j] = FoodState.InPackage;
                                                }
                                            }

                                            simulator.packageStates[packageIndex] = PackageState.Filled;
                                        }
                                    )
                                );

                                for (int j = 0; j < simulator.foods.Length; j++)
                                {
                                    if (simulator.foodBelongTo[j] == "package" + packageIndex)
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
                    simulator.packageStates[packageIndex] = PackageState.Moving;
                }

                simulator.foodStates[i] = FoodState.Putting;

                currentMoved++;

                if (currentMoved < capacity)
                {
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    playerController.ResetProperties();
                    break;
                }
            }
        }
    }

    IEnumerator ContinueCheck(Transform tf, PlayerController playerController)
    {
        while (isContinueCheck)
        {
            if (tf.position.x < transform.position.x + 0.25f * simulator.packageTableSize.x)
            {
                PickingPackage(playerController);

                isContinueCheck = false;
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    int GetFillingPackageIndex()
    {
        int packageIndex = -1;

        for (int i = 0; i < simulator.packages.Length; i++)
        {
            if (simulator.packageStates[i] == PackageState.Filling)
            {
                packageIndex = i;
                break;
            }
        }

        return packageIndex;
    }

    int GetFilledPackageIndex()
    {
        int packageIndex = -1;

        for (int i = 0; i < simulator.packages.Length; i++)
        {
            if (simulator.packageStates[i] == PackageState.Filled)
            {
                if (packageIndex != -1)
                {
                    if (simulator.packageColumnIndex[i] > simulator.packageColumnIndex[packageIndex])
                    {
                        packageIndex = i;
                    }
                }
                else
                {
                    packageIndex = i;
                }
            }
        }

        return packageIndex;
    }

    void PickingPackage(PlayerController controller)
    {
        int packageIndex = GetFilledPackageIndex();

        if (packageIndex != -1)
        {
            if (controller.playerState == PlayerState.Ready)
            {
                StartCoroutine(
                    simulator.CurveMove(
                        simulator.packages[packageIndex].transform,
                        simulator.packages[packageIndex].transform.position,
                        controller.gameObject.transform.position
                        + new Vector3(0, 7, 0)
                        + controller.gameObject.transform.forward * 4,
                        12,
                        0,
                        () =>
                        {
                            controller.playerState = PlayerState.HoldingPackageMoving;
                            simulator.packageBelongTo[packageIndex] = "player";
                            simulator.packageStates[packageIndex] = PackageState.Moving;
                        }
                    )
                );

                for (int i = 0; i < simulator.foods.Length; i++)
                {
                    if (simulator.foodBelongTo[i] == "package" + packageIndex)
                    {
                        simulator.foodStates[i] = FoodState.MovingWithPackage;
                    }
                }

                util.SetHoldingFoodStandingAnimation(controller.playerAnimator);

                controller.playerState = PlayerState.PickingPackage;
            }
        }
    }
}
