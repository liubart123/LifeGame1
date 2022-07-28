using Assets.Game.Scripts.LifeGame;
using Assets.Game.Scripts.LifeGame.Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInputController : MonoBehaviour
{
    public int mapWidth = 100, mapHeight = 100;
    public int[] neuronsNumberInLayers = new int[] { 5, 6, 4 };
    public int countOfTicksInIteration = 50;
    public int countOfUnits = 200;
    public float maxMinSuccessAncestorRatio = 10;
    public int currentIterationCount = 0;
    public int iterationsToSkip = 0;


    public float chanceOfMutationForUnit = 0.1f;
    public float chanceOfMutation = 0.1f;
    public float chanceOfRemovingSynops = 0.1f;

    public int startNumberOfNeuorns;
    public int numberOfNeuronsForMutation;

    LifeGameController lifeGameController = LifeGameController.Instance;
    MapController mapController = MapController.Instance;
    MapRenderer mapRenderer;

    void UnpdateLifeGameParameters()
    {
        lifeGameController.mapWidth = mapWidth;
        lifeGameController.mapHeight = mapHeight;
        lifeGameController.neuronsNumberInLayers = neuronsNumberInLayers;
        lifeGameController.countOfTicksInIteration = countOfTicksInIteration;
        lifeGameController.countOfUnits = countOfUnits;
        lifeGameController.maxMinSuccessAncestorRatio = maxMinSuccessAncestorRatio;
        lifeGameController.chanceOfMutation = chanceOfMutation;
        lifeGameController.chanceOfMutationForUnit = chanceOfMutationForUnit;
        lifeGameController.chanceOfRemovingSynops = chanceOfRemovingSynops;

        lifeGameController.startNumberOfNeuorns = startNumberOfNeuorns;
        lifeGameController.numberOfNeuronsForMutation = numberOfNeuronsForMutation;


        currentIterationCount = lifeGameController.currentIterationCount;
    }
    void UpdateView()
    {
        mapRenderer.RenderMap(mapController);
    }
    public void SetUpNewLifeGame()
    {
        UnpdateLifeGameParameters();
        lifeGameController.SetUpNewLifeGame();
        mapRenderer.CreateMap(mapController);
        UpdateView();
    }
    public void SetUpNewLifeIteration()
    {
        UnpdateLifeGameParameters();
        lifeGameController.SetUpNewLifeIteration();
        UpdateView();
    }
    public void RunTicksToTheEndOfIteration()
    {
        UnpdateLifeGameParameters();
        lifeGameController.RunTicksToTheEndOfIteration();
        UpdateView();
    }
    public void RunTick()
    {
        UnpdateLifeGameParameters();
        lifeGameController.RunTick();
        UpdateView();
    }
    public IEnumerator SkipIterationsCoroutine()
    {
        yield return null;
        StopPlayingTicksSequently();
        UnpdateLifeGameParameters();
        lifeGameController.RunMultipleIterations(iterationsToSkip);
        UpdateView();
    }
    public void SkipIterations()
    {
        StartCoroutine(SkipIterationsCoroutine());
    }

    #region playing ticks
    public float secondsPefFrame = 0.5f;
    public void PlayTicksSequently()
    {
        if (isPLaying)
            StopAllCoroutines();
        else
            StartCoroutine(PlayTick());
        isPLaying = !isPLaying;
    }
    bool isPLaying = false;
    void StopPlayingTicksSequently()
    {
        StopAllCoroutines();
        isPLaying = false;
    }
    IEnumerator PlayTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPefFrame);
            lifeGameController.RunTick();
            UpdateView();
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mapRenderer = FindObjectOfType<MapRenderer>();
        UnpdateLifeGameParameters();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    PlayTicksSequently();
    }
}
