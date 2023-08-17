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
    DoneBasic
}

public class GameProgressManager : MonoBehaviour
{
    public Simulator simulator;
    public NPC_Manager npc_Manager;

    public bool isEnableTutorial;
    public ProgressStep progressStep;
    private ProgressStep prevStep;

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
                    simulator.upgradeAreas[0].transform.localScale.z
                );
                simulator.upgradeAreas[0].GetComponent<UpgradeArea>().
                    initialScale = simulator.upgradeAreas[0].transform.localScale;

                simulator.upgradeAreaFunctions[0] = UpgradeAreaFunction.CreateGate;

                simulator.tutorialText.text = "Create a door";

                StartCoroutine(simulator.SpawnUpgradeArea(new Vector3(35, 0.5f, -60)));

                prevStep = progressStep;
                progressStep = ProgressStep.Wait;
            }
            else if (progressStep == ProgressStep.DoneBasic)
            {
                bool isSpawnTableUpgrade = true;

                for (int i = 0; i < simulator.upgradeAreas.Length; i++)
                {
                    if (simulator.upgradeAreaFunctions[i] == UpgradeAreaFunction.CreateTable)
                    {
                        isSpawnTableUpgrade = false;
                        break;
                    }
                }

                if (isSpawnTableUpgrade)
                {
                    for (int i = 0; i < simulator.tables.Length; i++)
                    {
                        if (simulator.tableStates[i] == TableState.NotSpawn)
                        {
                            int col = 2;

                            int x = i % col;
                            int y = (i - i % col) / col;

                            StartCoroutine(
                                simulator.SpawnUpgradeArea(
                                    new Vector3(
                                        simulator.tables[0].transform.position.x + x * 40,
                                        simulator.tables[0].transform.position.y,
                                        simulator.tables[0].transform.position.z - y * 30
                                    ),
                                    () =>
                                    {
                                        progressStep = ProgressStep.DoneBasic;
                                    },
                                    UpgradeAreaFunction.CreateTable
                                )
                            );

                            prevStep = progressStep;
                            progressStep = ProgressStep.Wait;

                            break;
                        }
                    }
                }

                if (!npc_Manager.npcs[0].activeInHierarchy)
                {
                    StartCoroutine(
                        simulator.SpawnUpgradeArea(
                            simulator.counter.transform.position - new Vector3(0.8f * simulator.counterSize.x, 0, 0),
                            () =>
                            {

                            },
                            UpgradeAreaFunction.AddStaff
                        )
                    );
                }

                if (!simulator.packageTable.activeInHierarchy)
                {
                    StartCoroutine(
                        simulator.SpawnUpgradeArea(
                            new Vector3(
                                simulator.packageTable.transform.position.x,
                                0,
                                simulator.packageTable.transform.position.z
                            ),
                            () =>
                            {

                            },
                            UpgradeAreaFunction.CreatePackageTable
                        )
                    );
                }

                if (simulator.packageTable.activeInHierarchy && !simulator.takeawayCounter.activeInHierarchy)
                {
                    StartCoroutine(
                        simulator.SpawnUpgradeArea(
                            new Vector3(
                                simulator.takeawayCounter.transform.position.x,
                                0,
                                simulator.takeawayCounter.transform.position.z
                            ),
                            () =>
                            {

                            },
                            UpgradeAreaFunction.CreateTakeawayCounter
                        )
                    );
                }
            }

            yield return new WaitForSeconds(5f);
        }
    }
}
