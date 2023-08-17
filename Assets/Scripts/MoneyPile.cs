using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPile : MonoBehaviour
{
    public float speed;

    private float deltaTime;

    private Simulator simulator;
    private UI_Manager uiManager;
    private MoneyPileManager moneyPileManager;
    public ResourceManager resourceManager;

    private GameObject player;

    private int unitValue;
    private int totalMoneyTaken = 0;

    public int index;

    // Start is called before the first frame update
    void Start()
    {
        simulator = GameObject.Find("Simulator").GetComponent<Simulator>();
        uiManager = GameObject.Find("UI_Manager").GetComponent<UI_Manager>();
        moneyPileManager = GameObject.Find("MoneyPileManager").GetComponent<MoneyPileManager>();
        resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();

        player = GameObject.Find("Player");

        deltaTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Effect(0));

        StartCoroutine(
            simulator.TakingMoneyEffect(
                transform,
                player.transform,
                gameObject.activeInHierarchy
            )
        );
    }

    IEnumerator Effect(int phase)
    {
        GameObject[] topFaces = new GameObject[2];
        GameObject bottom;
        ParticleSystem effect;

        for (int i = 0; i < transform.childCount - 2; i++)
        {
            topFaces[i] = transform.GetChild(i).gameObject;
        }

        bottom = transform.GetChild(transform.childCount - 2).gameObject;
        effect = transform.GetChild(transform.childCount - 1).gameObject.GetComponent<ParticleSystem>();

        if(phase == 0)
        {
            effect.transform.position = new Vector3(
                effect.transform.position.x,
                topFaces[0].transform.position.y,
                effect.transform.position.z
                );
           /* effect.gameObject.SetActive(true);
            effect.Play();*/

            unitValue = ((int)bottom.transform.localScale.y);
            uiManager.moneyTakenTMP[index].gameObject.transform.position = new Vector3(
                transform.position.x, transform.position.y + 20, transform.position.z - 15
            );
            uiManager.moneyTakenTMP[index].gameObject.SetActive(true);

            StartCoroutine(Effect(1));
        }
        else if (phase == 1)
        {
            while (transform.position.y > -bottom.transform.localScale.y - 1)
            {
                transform.Translate(new Vector3(0, -speed * deltaTime, 0));

                setMoneyTMPs();

                yield return new WaitForSeconds(0.03f);
            }

            StartCoroutine(Effect(2));
        }
        else if (phase == 2)
        {
            while (topFaces[0].GetComponent<MeshRenderer>().materials[0].color.a > 0)
            {
                Color color = topFaces[0].GetComponent<MeshRenderer>().materials[0].color;

                for (int i = 0; i < topFaces.Length; i++)
                {
                    topFaces[i].transform.Translate(-1 + 2*i * speed * deltaTime,
                        speed * deltaTime, 0);
                    topFaces[i].GetComponent<MeshRenderer>().materials[0].color = new Color(
                        color.r, color.g, color.b, color.a - 0.05f
                        );
                }

                setMoneyTMPs();

                yield return new WaitForSeconds(0.03f);
            }

            moneyPileManager.ResetProperties(index);
        }
    }

    void setMoneyTMPs()
    {
        totalMoneyTaken += unitValue;
        resourceManager.money += unitValue;
        uiManager.moneyTakenTMP[index].text = "$" + totalMoneyTaken;
        resourceManager.moneyTMP.text = "$" + resourceManager.money;
    }
}
