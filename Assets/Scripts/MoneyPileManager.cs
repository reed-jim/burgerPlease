using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MoneyPileState
{
    NotSpawn,
    Active
}

public class MoneyPileManager : MonoBehaviour
{
    public GameObject player;
    public GameObject moneyPilePrefab;
    public GameObject[] moneyPiles;
    public TMP_Text moneyTakenTMP;
    public ParticleSystem particle;

    public string[] moneyPileBelongTo;
    public MoneyPileState[] moneyPileStates;

    public ResourceManager resourceManager;
    public UI_Manager uiManager;
    public float speed;

    private Renderer m_renderer;
    private float deltaTime;
    private int unitValue;

    // Destroy() must be the last to be called
    // Start is called before the first frame update
    void Start()
    {
        moneyPileBelongTo = new string[moneyPiles.Length];
        moneyPileStates = new MoneyPileState[moneyPiles.Length];

        for (int i = 0; i < moneyPiles.Length; i++)
        {
            moneyPiles[i] = Instantiate(moneyPilePrefab);

            moneyPiles[i].SetActive(false);

            moneyPiles[i].GetComponent<MoneyPile>().index = i;
        }
        moneyPileBelongTo = new string[moneyPiles.Length];

        m_renderer = GetComponent<Renderer>();
        deltaTime = Time.deltaTime;
        unitValue = Random.Range(5, 20);
    }

    public void SpawnMoneyPile(Vector3 spawnPosition, int tableIndex)
    {
        int moneyPileIndexBelongToTable = GetMoneyIndexBelongToTable(tableIndex);
        int moneyPileIndex;

        GameObject moneyPile;
        /*GameObject[] topFaces = new GameObject[2];
        GameObject bottom;*/

        moneyPileIndex = moneyPileIndexBelongToTable == -1 ?
            GetAvailableMoneyPile() : moneyPileIndexBelongToTable;
        moneyPile = moneyPiles[moneyPileIndex];


        float yScale = moneyPileIndexBelongToTable == -1 ? Random.Range(3, 12) * 3 :
            Random.Range(3, 12) * 3 + moneyPile.transform.localScale.y;

        moneyPile.transform.localScale = new Vector3(
            moneyPile.transform.localScale.x,
            yScale,
            moneyPile.transform.localScale.z
        );

        moneyPile.transform.position = new Vector3(
            spawnPosition.x,
            0.5f * moneyPile.transform.localScale.y,
            spawnPosition.z
        );

        moneyPile.gameObject.SetActive(true);

        moneyPileBelongTo[moneyPileIndex] = "table" + tableIndex;
        moneyPileStates[moneyPileIndex] = MoneyPileState.Active;


        /*for (int i = 0; i < moneyPile.transform.childCount - 2; i++)
        {
            topFaces[i] = moneyPile.transform.GetChild(i).gameObject;
        }

        bottom = moneyPile.transform.GetChild(moneyPile.transform.childCount - 2).gameObject;

        float yScale = moneyPileIndexBelongToTable == -1 ? Random.Range(1, 10) * 3 :
            Random.Range(1, 10) * 3 + bottom.transform.localScale.y;

        bottom.transform.localScale = new Vector3(
            bottom.transform.localScale.x,
            yScale,
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

        moneyPile.gameObject.SetActive(true);

        moneyPileBelongTo[moneyPileIndex] = "table" + tableIndex;
        moneyPileStates[moneyPileIndex] = MoneyPileState.Active;*/
    }

    int GetMoneyIndexBelongToTable(int tableIndex)
    {
        for (int i = 0; i < moneyPiles.Length; i++)
        {
            if (moneyPileBelongTo[i] == "table" + tableIndex)
            {
                return i;
            }
        }

        return -1;
    }

    int GetAvailableMoneyPile()
    {
        for (int i = 0; i < moneyPiles.Length; i++)
        {
            if (moneyPileStates[i] == MoneyPileState.NotSpawn)
            {
                return i;
            }
        }

        return -1;
    }

    public void ResetProperties(int index)
    {
        moneyPiles[index].SetActive(false);
        moneyTakenTMP.gameObject.SetActive(false);

        moneyPiles[index].GetComponent<MoneyPile>().totalMoneyTaken = 0;
        moneyPileBelongTo[index] = "none";
        moneyPileStates[index] = MoneyPileState.NotSpawn;
    }
}
