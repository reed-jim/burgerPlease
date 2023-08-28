using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public Simulator simulator;
    public GameProgressManager gameProgressManager;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController.playerState == PlayerState.HoldingTrashMoving)
            {
                playerController.playerState = PlayerState.HoldingTrashStanding;

                MoveTrashOneByOneToTrashCanByPlayer("player");
            }
        }
        else if (other.CompareTag("NPC"))
        {
            HumanController humanController = other.GetComponent<HumanController>();

            if (humanController.humanState == HumanState.HoldingAndMoving)
            {
                humanController.SetNextStateAfterHoldingMoving();

                MoveTrashOneByOneToTrashCanByNPC(
                    humanController.id,
                    humanController
                );
            }
        }
    }

    IEnumerator MoveTrashOneByOneToTrashCan(string owner, OnMoveAllTrash onMoveAllTrash)
    {
        for (int i = 0; i < simulator.trashBelongTo.Length; i++)
        {
            if (simulator.trashBelongTo[i] == owner)
            {
                int trashIndex = i;

                StartCoroutine(
                    simulator.CurveMove(
                        simulator.trashs[i].transform,
                        simulator.trashs[i].transform.position,
                        transform.position + new Vector3(0, 5, 0),
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

        onMoveAllTrash();
    }

    void MoveTrashOneByOneToTrashCanByPlayer(string owner)
    {
        StartCoroutine(MoveTrashOneByOneToTrashCan(
            owner,
            () =>
            {
                if(gameProgressManager.progressStep == ProgressStep.ThrowTrashTutorial)
                {
                    gameProgressManager.progressStep = ProgressStep.TutorialComplete;
                }

                playerController.playerState = PlayerState.Ready;
            }
        ));
    }

    void MoveTrashOneByOneToTrashCanByNPC(string owner, HumanController humanController)
    {
        StartCoroutine(MoveTrashOneByOneToTrashCan(
            owner,
            () =>
            {
                humanController.ResetProperties();
            }
        ));
    }

    delegate void OnMoveAllTrash();
}
