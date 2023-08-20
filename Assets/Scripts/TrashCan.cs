using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public Simulator simulator;
    public PlayerController playerController;
    private Vector3 trashSize;

    // Start is called before the first frame update
    void Start()
    {
        trashSize = GetComponent<MeshRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(playerController.playerState == PlayerState.HoldingTrashMoving)
            {
                StartCoroutine(MoveTrashOneByOneToTrashCan());
            }
        }
    }

    IEnumerator MoveTrashOneByOneToTrashCan()
    {
        for (int i = 0; i < simulator.trashBelongTo.Length; i++)
        {
            if (simulator.trashBelongTo[i] == "player")
            {
                int trashIndex = i;

                StartCoroutine(
                    simulator.CurveMove(
                        simulator.trashs[i].transform,
                        simulator.trashs[i].transform.position,
                        transform.position + new Vector3(0, trashSize.y, 0),
                        12,
                        0,
                        () =>
                        {
                            simulator.resetTrashProperties(trashIndex);
                        }
                    )
                );

                simulator.trashStates[i] = TrashState.MovingToTrashCan;


                yield return new WaitForSeconds(0.2f);
            }
        }

        playerController.playerState = PlayerState.Ready;
    }
}
