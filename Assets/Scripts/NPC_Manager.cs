using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    public GameObject npcPrefab;
    public GameObject[] npcs;
    public HumanController[] npcControllers;

    // Start is called before the first frame update
    void Start()
    {
        npcs = new GameObject[1];
        npcControllers = new HumanController[npcs.Length];

        SpawnNPC();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnNPC()
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            npcs[i] = Instantiate(npcPrefab);

            npcs[i].transform.position = new Vector3(65, 0, 15);
            npcs[i].SetActive(true);

            npcControllers[i] = npcs[i].GetComponent<HumanController>();
            npcControllers[i].id = "npc" + i;
        }
    }
}
