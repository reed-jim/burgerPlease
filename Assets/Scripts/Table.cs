using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Simulator simulator;
    public Util util;
    public PlayerController playerController;

    public GameObject player;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (simulator.tableStates[index] == TableState.Dirty)
        {
            if (other.CompareTag("Player"))
            {
                if (playerController.playerState == PlayerState.Ready)
                {
                    simulator.RotateChairs(index, new Vector3(0, 90, 0), new Vector3(0, 270, 0));

                    simulator.tableStates[index] = TableState.EmptyBoth;

                    simulator.moveTrashOneByOneByPlayer(
                         "player",
                         player.transform,
                         index
                     );
                }
            }
            if (other.CompareTag("NPC"))
            {
                HumanController humanController = other.GetComponent<HumanController>();

                if (humanController.humanState == HumanState.Moving)
                {
                    simulator.RotateChairs(index, new Vector3(0, 90, 0), new Vector3(0, 270, 0));

                    simulator.tableStates[index] = TableState.EmptyBoth;

                    humanController.SetNextStateAfterMoving(HumanState.PickingTrash);

                    simulator.moveTrashOneByOneByNPC(
                        humanController.id,
                        other.transform,
                        index
                    );
                }
            }
        }
    }
}
