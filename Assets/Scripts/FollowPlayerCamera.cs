using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    FollowPlayer,
    ZoomStart,
    Zoom
}

public class FollowPlayerCamera : MonoBehaviour
{
    public Vector3 offset;
    public GameObject player;

    public CameraState cameraState;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x + offset.x,
            transform.position.y, player.transform.position.z + offset.z);
        if (cameraState == CameraState.FollowPlayer)
        {
            /*  transform.position = new Vector3(player.transform.position.x + offset.x,
              transform.position.y, player.transform.position.z + offset.z);*/
        }
        else if (cameraState == CameraState.ZoomStart)
        {
            StartCoroutine(Zoom());
            cameraState = CameraState.Zoom;
        }
    }

    public IEnumerator Zoom()
    {
        int phase = 0;

        while (phase < 2)
        {
            if (phase == 0)
            {
                if (Camera.main.orthographicSize > 70)
                {
                    Camera.main.orthographicSize -= 2;
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    phase++;
                }
            }
            else if (phase == 1)
            {
                if (Camera.main.orthographicSize < 110)
                {
                    Camera.main.orthographicSize += 2;
                }
                else
                {
                    phase++;
                }
            }

            yield return new WaitForSeconds(0.002f);
        }
    }
}
