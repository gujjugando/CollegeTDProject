﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager GM;
    public Transform kingPrefab, OffenseHQ;
    public Transform myMap;
    public Image loadingBackground;
    public Text percentDoneText;
    public Transform pausePanel;
    public Vector3 spawnPosition;

    //All Lists
    public ScriptableRankProperties[] rankProps;
    public GameObject[] interfaceActiveInBattle;
    public GameObject[] interfaceBeforeBattle;

    int rank;
    ScriptableRankProperties currentRankProps;
    PathGenerator generator;
    Transform myGeneratedMap;

    public int towersDead = 0;
    public int barricadesDead = 0;
    public bool gateBroken = false;

    private GameResultPanel resultPanel;
    private UnityAction nextLevelAction;
    private UnityAction mainMenuAction;

    float waitTime = 5f;
    bool loadingDone = false;

    public static GameManager Instance()
    {
        if (!GM)
        {
            GM = FindObjectOfType(typeof(GameManager)) as GameManager;
            if (!GM)
                Debug.LogError("There needs to be one active gameOverPanel script on a GameObject in your scene.");
        }

        return GM;
    }

    void Start()
    {
        StatisticsManager.SM.OnDataSet += StartLoading;
    }

    void StartLoading()
    {
        //1. Loading should appear
        //2. Get value from StatisticsManager
        //3. Set the values for Path
        //4. Give the  values to PathGenerator Class
        //5. Activate the interface before battle
        //5. End
        loadingDone = false;
        loadingBackground.gameObject.SetActive(true);

        //Instantiate MyMap
        myGeneratedMap = Instantiate(myMap, Vector3.zero, Quaternion.identity) as Transform;
        myGeneratedMap.gameObject.name = "MyMap";

        //Create OffenseHeadquaters
        Transform OHQ = Instantiate(OffenseHQ, transform.position, Quaternion.identity) as Transform;
        OHQ.name = "OffenseHeadQuaters";

        BeforeBattle(false);
        InBattle(false);
        rank = StatisticsManager.SM.GetDetails("Player_Rank");
        currentRankProps = rankProps[rank - 1];
        generator = myGeneratedMap.gameObject.GetComponent<PathGenerator>();
        generator.OnPercentChange += UpdatePercentValue;
        generator.SetValuesAndGenerate(currentRankProps);
        spawnPosition = generator.spawnPointPosition;
        resultPanel = GameResultPanel.Instance();
        gateBroken = false;
    }

    public void UpdatePercentValue(int value)
    {
        percentDoneText.text = value + " %";
        if(value == 100)
        {
            loadingDone = true;
        }
    }

    void Update()
    {
        if(loadingDone)
        {
            if(Input.GetMouseButtonDown(0))
            {
                RemoveLoading();
                loadingDone = false;
            }
        }
        if(gateBroken){
            //Display game Over dialog
            gateBroken = false;
            GateDestroyed();

        }
    }
    public void RemoveLoading()
    {
        loadingBackground.gameObject.SetActive(false);
        BeforeBattle(true);
    }

    public int GetDefenseType(string defense_type)
    {
        if(defense_type == "Arrow_Tower")
        {
            return Constants.ARROW_TOWER;
        }
        else if(defense_type == "Bomb_Tower")
        {
            return Constants.BOMB_TOWER;
        }
        else if(defense_type == "Block_Barricade")
        {
            return Constants.BLOCK_BARRICADE;
        }
        else if(defense_type == "Ground_Barricade")
        {
            return Constants.GROUND_BARRICADE;
        }
        else if(defense_type == "Gate")
        {
            return Constants.GATE;
        }
        return -1;
    }

    void InBattle(bool state)
    {
        foreach (GameObject b in interfaceActiveInBattle)
        {
            b.gameObject.SetActive(state);
        }
    }

    void BeforeBattle(bool state)
    {
        foreach (GameObject b in interfaceBeforeBattle)
        {
            b.gameObject.SetActive(state);
        }
    }

    public void OnBattleBtnClicked(Transform t)
    {
        t.gameObject.SetActive(false);
        InBattle(true);
        Instantiate(kingPrefab, spawnPosition, Quaternion.identity);
    }

    public void OnPauseClicked()
    {
        pausePanel.gameObject.SetActive(true);
        InBattle(false);
        Time.timeScale = 0f;
    }
    
    public void OnResumeClicked()
    {
        pausePanel.gameObject.SetActive(false);
        InBattle(true);
        Time.timeScale = 1f;
    }

    public void EntityDestoryed(string name)
    {
        if(name == "ArrowTower" || name == "BombTower")
        {
            towersDead++;
        }if(name == "BlockBarricade" || name == "GroundBarricade")
        {
            barricadesDead++;
        }if(name == "Gate")
        {
            gateBroken = true;
        }
    }

    private void GateDestroyed()
    {
        Time.timeScale = 0;
        nextLevelAction = new UnityAction(StartNextLevel);
        mainMenuAction = new UnityAction(OpenMainMenu);
        int towersXp = towersDead * currentRankProps.towersXp;
        if(towersDead == currentRankProps.numOfArrowTowers + currentRankProps.numOfBombTowers)
        {
            towersXp += currentRankProps.allTowers;
        }
        int barricadesXp = barricadesDead * currentRankProps.barricadeXp;
        int gateXp = 0;
        if(gateBroken)
        {
            gateXp += currentRankProps.gateXp;
        }
        resultPanel.ShowResult(nextLevelAction, mainMenuAction, towersXp, barricadesXp, gateXp);
    }

    private void StartNextLevel()
    {
        UIManager.UM.LoadScene(2);
    }

    private void OpenMainMenu()
    {
        UIManager.UM.LoadScene(0);
    }
}
