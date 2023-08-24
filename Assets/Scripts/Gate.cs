using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject player;
    public GameObject doorLeft;
    public GameObject doorRight;

    private bool isOpening = false;
    private bool isClosing = false;

    public float deltaAngle;

    private void Start()
    {
        StartCoroutine(CheckColision());
    }

    /*private void OnTriggerExit(Collider other)
    {
        StartCoroutine(OpenDoor(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(OpenDoor(true));
    }*/

    IEnumerator OpenDoor(bool isOpen)
    {
        float doorLeftAngle = doorLeft.transform.eulerAngles.y;
        if (doorLeftAngle > 180)
        {
            doorLeftAngle -= 360;
        }

        if (doorLeft.activeInHierarchy)
        {
            if (isOpen && !isOpening && !isClosing)
            {
                isOpening = true;

                while (doorLeftAngle > -90 && !isClosing)
                {
                    doorLeft.transform.eulerAngles -= new Vector3(0, deltaAngle, 0);
                    doorRight.transform.eulerAngles += new Vector3(0, deltaAngle, 0);

                    doorLeftAngle = doorLeft.transform.eulerAngles.y;
                    if (doorLeftAngle > 180)
                    {
                        doorLeftAngle -= 360;
                    }

                    yield return new WaitForSeconds(0.02f);
                }

                if (!isClosing)
                {
                    doorLeft.transform.eulerAngles = new Vector3(0, -90, 0);
                    doorRight.transform.eulerAngles = new Vector3(0, 270, 0);
                }

                isOpening = false;
            }

            if (!isOpen && !isClosing)
            {
                isClosing = true;

                yield return new WaitForSeconds(0.2f);

                while (doorLeftAngle < 0)
                {
                    doorLeft.transform.eulerAngles += new Vector3(0, deltaAngle, 0);
                    doorRight.transform.eulerAngles -= new Vector3(0, deltaAngle, 0);

                    doorLeftAngle = doorLeft.transform.eulerAngles.y;
                    if (doorLeftAngle > 180)
                    {
                        doorLeftAngle -= 360;
                    }

                    yield return new WaitForSeconds(0.02f);
                }

                doorLeft.transform.eulerAngles = new Vector3(0, 0, 0);
                doorRight.transform.eulerAngles = new Vector3(0, 180, 0);

                isClosing = false;
            }
        }
    }

    IEnumerator CheckColision()
    {
        while (true)
        {
            if (gameObject.activeInHierarchy)
            {
                if (Mathf.Abs(player.transform.position.x - transform.position.x) < 25
                    && Mathf.Abs(player.transform.position.z - transform.position.z) < 25)
                {
                    StartCoroutine(OpenDoor(true));
                }

                if (Mathf.Abs(player.transform.position.x - transform.position.x) > 26
                    || Mathf.Abs(player.transform.position.z - transform.position.z) > 26)
                {
                    StartCoroutine(OpenDoor(false));
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
