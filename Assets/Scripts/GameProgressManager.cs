using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProgressStep
{
    Wait,
    CreateGate,
    CreateFirstTable,
    CreateTable,
    CreateStove,
    CreateCounter,
    PickupFoodTutorialStart,
    PickupFoodTutorial,
    PutFoodOnCounterTutorialStart,
    PutFoodOnCounterTutorial,
    CashierTutorialStart,
    CashierTutorial,
    PickTrashTutorialStart,
    PickTrashTutorial,
    ThrowTrashTutorialStart,
    ThrowTrashTutorial,
    TutorialComplete,
    DoneBasic
}

public class GameProgressManager : MonoBehaviour
{
    public Simulator simulator;
    public NPC_Manager npc_Manager;
    public Util util;

    public bool isEnableTutorial;
    public ProgressStep progressStep;

    public bool isTableUpgradeSpawn = false;
    public bool isCashierUpgradeSpawn = false;
    public bool isPackageTableUpgradeSpawn = false;
    public bool isTakeawayCounterUpgradeSpawn = false;
    public bool isOfficeTableUpgradeSpawn = false;

    // Start is called before the first frame update
    private void Awake()
    {
        progressStep = ProgressStep.CreateGate;
    }

    void Start()
    {
        if (isEnableTutorial)
        {
            StartCoroutine(Progressing());
        }
    }

    IEnumerator Progressing()
    {
        while (true)
        {
            if (progressStep == ProgressStep.CreateGate)
            {
                yield return new WaitForSeconds(1f);

                simulator.DisableObjectsForCreateGateStep();

                simulator.upgradeAreas[0].transform.localScale = new Vector3(
                    22,
                    simulator.upgradeAreas[0].transform.localScale.y,
                    20
                );
                simulator.upgradeAreas[0].GetComponent<UpgradeArea>().
                    initialScale = simulator.upgradeAreas[0].transform.localScale;

                util.SetTMPTextOnBackground(simulator.tutorialText, simulator.tutorialTextBackground, "Create a door!");

                simulator.SpawnUpgradeArea(new Vector3(35.5f, 0.5f, simulator.gate.transform.position.z),
                    UpgradeAreaFunction.CreateGate, 500);

                progressStep = ProgressStep.Wait;
            }
            else if (progressStep == ProgressStep.PickupFoodTutorialStart)
            {
                simulator.SpawnTutorialArrow(simulator.foodStorages[0].transform.position);
                util.SetTMPTextOnBackground(simulator.tutorialText, simulator.tutorialTextBackground, "Pick up food!");

                progressStep = ProgressStep.PickupFoodTutorial;
            }
            else if (progressStep == ProgressStep.PutFoodOnCounterTutorialStart)
            {
                simulator.SpawnTutorialArrow(simulator.bowl.transform.position);
                util.SetTMPTextOnBackground(simulator.tutorialText, simulator.tutorialTextBackground, "Put food on the counter!");

                progressStep = ProgressStep.PutFoodOnCounterTutorial;
            }
            else if (progressStep == ProgressStep.CashierTutorialStart)
            {
                simulator.SpawnTutorialArrow(simulator.cashierPosition.transform.position);
                util.SetTMPTextOnBackground(simulator.tutorialText, simulator.tutorialTextBackground, "Go to cashier position!");

                progressStep = ProgressStep.CashierTutorial;
            }
            else if (progressStep == ProgressStep.PickTrashTutorialStart)
            {
                for (int i = 0; i < simulator.tables.Length; i++)
                {
                    if (simulator.tableStates[i] == TableState.Dirty)
                    {
                        simulator.tutorialArrow.SetActive(true);
                        simulator.tutorialText.gameObject.SetActive(true);

                        simulator.SpawnTutorialArrow(simulator.tables[i].transform.position);
                        util.SetTMPTextOnBackground(simulator.tutorialText, simulator.tutorialTextBackground, "Clean the table!");

                        progressStep = ProgressStep.PickTrashTutorial;

                        break;
                    }

                    if(i == simulator.tables.Length - 1)
                    {
                        simulator.tutorialArrow.SetActive(false);
                        simulator.tutorialText.gameObject.SetActive(false);
                    }
                }
            }
            else if (progressStep == ProgressStep.ThrowTrashTutorialStart)
            {
                simulator.SpawnTutorialArrow(simulator.trashCan.transform.position);
                util.SetTMPTextOnBackground(simulator.tutorialText, simulator.tutorialTextBackground, "Throw trash to the dumpster!");

                progressStep = ProgressStep.ThrowTrashTutorial;
            }
            else if (progressStep == ProgressStep.TutorialComplete)
            {
                util.SetTMPTextOnBackground(simulator.tutorialText, simulator.tutorialTextBackground,
                    "Tutorial Completed. Congratulation!");

                simulator.tutorialArrow.SetActive(false);

                isEnableTutorial = false;

                yield return new WaitForSeconds(1.5f);

                simulator.tutorialTextBackground.gameObject.SetActive(false);

                progressStep = ProgressStep.DoneBasic;
            }
            else if (progressStep == ProgressStep.DoneBasic)
            {
                // spawn tables
                if (!isTableUpgradeSpawn)
                {
                    for (int i = 0; i < simulator.tables.Length; i++)
                    {
                        if (simulator.tableStates[i] == TableState.NotSpawn)
                        {
                            int col = 2;

                            int x = i % col;
                            int y = (i - i % col) / col;

                            simulator.SpawnUpgradeArea(
                                new Vector3(
                                    simulator.tables[0].transform.position.x + x * 60,
                                    0,
                                    simulator.tables[0].transform.position.z - y * 40
                                ),
                                UpgradeAreaFunction.CreateTable
                            );

                            isTableUpgradeSpawn = true;

                            break;
                        }
                    }
                }
                // spawn cashier
                if (!npc_Manager.npcs[0].activeInHierarchy && !isCashierUpgradeSpawn)
                {
                    simulator.SpawnUpgradeArea(
                        new Vector3(
                            simulator.counter.transform.position.x - 0.8f * simulator.counterSize.x,
                            0,
                            simulator.counter.transform.position.z
                        ),
                        UpgradeAreaFunction.AddCashier
                    );

                    isCashierUpgradeSpawn = true;
                }
                // spawn package table
                if (!simulator.packageTable.activeInHierarchy && !isPackageTableUpgradeSpawn)
                {
                    simulator.SpawnUpgradeArea(
                        new Vector3(
                            simulator.packageTable.transform.position.x,
                            0,
                            simulator.packageTable.transform.position.z
                        ),
                        UpgradeAreaFunction.CreatePackageTable
                    );

                    isPackageTableUpgradeSpawn = true;
                }
                // spawn takeaway counter
                if (simulator.packageTable.activeInHierarchy
                    && !simulator.takeawayCounter.activeInHierarchy
                    && !isTakeawayCounterUpgradeSpawn)
                {
                    simulator.SpawnUpgradeArea(
                        new Vector3(
                            simulator.takeawayCounter.transform.position.x,
                            0,
                            simulator.takeawayCounter.transform.position.z
                        ),
                        UpgradeAreaFunction.CreateTakeawayCounter
                    );

                    isTakeawayCounterUpgradeSpawn = true;
                }
                // spawn office desk
                if (!isOfficeTableUpgradeSpawn)
                {
                    if (!simulator.officeDesk[0].activeInHierarchy)
                    {
                        simulator.SpawnUpgradeArea(
                            new Vector3(
                                simulator.officeDesk[0].transform.position.x,
                                0,
                                simulator.officeDesk[0].transform.position.z
                            ),
                            UpgradeAreaFunction.CreateHRUpgrade
                        );
                    }

                    if (!simulator.officeDesk[1].activeInHierarchy)
                    {
                        simulator.SpawnUpgradeArea(
                            new Vector3(
                                simulator.officeDesk[1].transform.position.x,
                                0,
                                simulator.officeDesk[1].transform.position.z
                            ),
                            UpgradeAreaFunction.CreatePlayerUpgrade
                        );
                    }

                    isOfficeTableUpgradeSpawn = true;
                }
            }

            yield return new WaitForSeconds(0.4f);
        }
    }
}
