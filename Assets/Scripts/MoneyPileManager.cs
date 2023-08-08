using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyPileManager : MonoBehaviour
{
    public GameObject player;
    public GameObject moneyPilePrefab;
    public TMP_Text moneyTakenTMP;
    public ParticleSystem particle;
    public ResourceManager resourceManager;
    public UI_Manager uiManager;
    public float speed;

    private Renderer m_renderer;
    private float deltaTime;
    private int unitValue;
    private bool isTaken = false;

    // Destroy() must be the last to be called
    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<Renderer>();
        deltaTime = Time.deltaTime;
        unitValue = Random.Range(5, 20);
    }

    private void OnTriggerEnter(Collider other)
    {
       /* isTaken = true;
        StartCoroutine(GetMoney());

        moneyTakenTMP.gameObject.SetActive(true);
        particle.gameObject.SetActive(true);
        particle.Play();*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (transform.position.y + m_renderer.bounds.size.y / 2 > 0f)
        {
            isTaken = false;
            particle.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(HandleAllMoneyTaken());

        }
    }

    IEnumerator HandleAllMoneyTaken()
    {
        yield return new WaitForSeconds(1f);

        moneyTakenTMP.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    IEnumerator GetMoney()
    {
        while (isTaken)
        {
            transform.Translate(Vector3.down * speed * deltaTime);
            resourceManager.money += unitValue;
            moneyTakenTMP.text = "$" + resourceManager.money;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SpawnMoneyPile(Vector3 spawnPosition)
    {
        GameObject moneyPile = Instantiate(moneyPilePrefab);

        GameObject[] topFaces = new GameObject[2];
        GameObject bottom;

        for (int i = 0; i < moneyPile.transform.childCount - 2; i++)
        {
            topFaces[i] = moneyPile.transform.GetChild(i).gameObject;
        }

        bottom = moneyPile.transform.GetChild(moneyPile.transform.childCount - 2).gameObject;

        bottom.transform.localScale = new Vector3(
            bottom.transform.localScale.x,
            Random.Range(1, 10) * 3,
            bottom.transform.localScale.z
            );

        bottom.transform.localPosition = new Vector3(
            0, bottom.transform.localScale.y / 2, 0
            );

        int numRow = 1;
        int numCol = (moneyPile.transform.childCount - 2) / numRow;
        
        for (int i = 0; i < numRow; i++)
        {         
            for (int j = 0; j < numCol; j++)
            {
                topFaces[j + numCol * i].transform.localScale = new Vector3(
                    bottom.transform.localScale.x / numCol,
                    topFaces[j + numCol * i].transform.localScale.y,
                    bottom.transform.localScale.z / numRow
                );
                
                topFaces[j + numCol * i].transform.localPosition = new Vector3(
                    j * bottom.transform.localScale.x / numCol - bottom.transform.localScale.x / 2 +
                    topFaces[j + numCol * i].transform.localScale.x / 2,
                    bottom.transform.localScale.y + topFaces[j + numCol * i].transform.localScale.y / 2,
                    -(i * bottom.transform.localScale.z / numRow - bottom.transform.localScale.z / 2 +
                    topFaces[j + numCol * i].transform.localScale.z / 2)
                );
            } 
        }

        moneyPile.transform.position = new Vector3(
            spawnPosition.x,
            0,
            spawnPosition.z
        );
    }
}
