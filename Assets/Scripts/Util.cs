using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    private float deltaTime;

    public delegate void MoveToCallback();

    // Start is called before the first frame update
    void Start()
    {
        deltaTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveTo(Transform tf, Vector3 destination, float speed, MoveToCallback callback)
    {
        tf.LookAt(destination);
        
        while (Vector3.Distance(tf.position, destination) > 5)
        {
            tf.Translate(tf.forward * speed * deltaTime, Space.World);

            yield return new WaitForSeconds(0.03f);
        }
        
        callback();
    }
}
