using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Simulator simulator;
    public Util util;
    public GameObject player;
    public int index;

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
        if (simulator.tableStates[index] == TableState.Dirty)
        {
            if (other.CompareTag("Player"))
            {
                simulator.RotateChairs(index, new Vector3(0, 90, 0), new Vector3(0, 270, 0));

                simulator.tableStates[index] = TableState.EmptyBoth;
                player.GetComponent<PlayerController>().playerState = PlayerState.HoldingTrash;

                StartCoroutine(
                    simulator.moveTrashOneByOne(
                        "player",
                        player.transform,
                        index
                    )
                );
            }
        }
    }
}
