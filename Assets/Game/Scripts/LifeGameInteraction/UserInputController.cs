using Assets.Game.Scripts.LifeGame;
using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Units.Brain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInputController : MonoBehaviour
{
    [Header("Map")]
    public int mapWidth = 100, mapHeight = 100;
    [Header("Neurons")]
    public int[] neuronsNumberInLayers = new int[] { 5, 6, 4 };
    [Header("Generations")]
    public float startSynopsValueRandomRange = 10f;
    public float synopsValueMutationRange = 0.1f;
    public float chanceOfUnitSoftMutation = 0.1f;
    public float chanceOfUnitHardMutation = 0.1f;
    public float chanceOfSynopsSoftMutation = 0.1f;
    public float chanceOfSynopsHardMutation;
    public int countOfUnits = 1;
    public float maxMinSuccessAncestorRatio = 10;



    [Header("General")]
    public int countOfTicksInIteration = 50;
    public int iterationsToSkip = 0;
    public int currentIterationCount = 0;

    LifeGameController lifeGameController = LifeGameController.Instance;
    MapController mapController = MapController.Instance;
    MapRenderer mapRenderer;

    void SynchronyseStateWithControllers()
    {
        currentIterationCount = lifeGameController.currentIterationCount;

        //map
        if (mapController.width != mapWidth || mapController.height != mapHeight)
        {
            mapController.CreateMap();
            mapController.width = mapWidth;
            mapController.height = mapHeight;
        }

        //neurons
        if (NeuronController.Instance.neuronsNumberInLayers != neuronsNumberInLayers)
        {
            NeuronController.Instance.neuronsNumberInLayers = neuronsNumberInLayers;
            NeuronController.Instance.Initialize();
        }
        if (GenerationController.Instance.neuronsNumberInLayers != neuronsNumberInLayers)
        {
            GenerationController.Instance.neuronsNumberInLayers = neuronsNumberInLayers;
            GenerationController.Instance.Initialize();
        }

        //Generations
        GenerationController.Instance.startSynopsValueRandomRange = startSynopsValueRandomRange;
        GenerationController.Instance.synopsValueMutationRange = synopsValueMutationRange;
        GenerationController.Instance.chanceOfUnitSoftMutation = chanceOfUnitSoftMutation;
        GenerationController.Instance.chanceOfUnitHardMutation = chanceOfUnitHardMutation;
        GenerationController.Instance.chanceOfSynopsSoftMutation = chanceOfSynopsSoftMutation;
        GenerationController.Instance.chanceOfSynopsHardMutation = chanceOfSynopsHardMutation;
        GenerationController.Instance.countOfUnits = countOfUnits;
        GenerationController.Instance.maxMinSuccessAncestorRatio = maxMinSuccessAncestorRatio;

        //lifeGameController
        lifeGameController.countOfTicksInIteration = countOfTicksInIteration;
    }
    void UpdateView()
    {
        mapRenderer.RenderMap(mapController);
    }
    public void SetUpNewLifeGame()
    {
        SynchronyseStateWithControllers();
        lifeGameController.SetUpNewLifeGame();
        mapRenderer.CreateMap(mapController);
        UpdateView();
    }
    public void SetUpNewLifeIteration()
    {
        SynchronyseStateWithControllers();
        lifeGameController.SetUpNewLifeIteration();
        UpdateView();
    }
    public void RunTicksToTheEndOfIteration()
    {
        SynchronyseStateWithControllers();
        lifeGameController.RunTicksToTheEndOfIteration();
        UpdateView();
    }
    public void RunTick()
    {
        SynchronyseStateWithControllers();
        lifeGameController.RunTick();
        UpdateView();
    }
    public IEnumerator SkipIterationsCoroutine()
    {
        yield return null;
        StopPlayingTicksSequently();
        SynchronyseStateWithControllers();
        lifeGameController.RunMultipleIterations(iterationsToSkip);
        UpdateView();
    }
    public void SkipIterations()
    {
        SynchronyseStateWithControllers();
        StartCoroutine(SkipIterationsCoroutine());
    }

    #region playing ticks
    public float secondsPefFrame = 0.5f;
    public void PlayTicksSequently()
    {
        SynchronyseStateWithControllers();
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
        SynchronyseStateWithControllers();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    PlayTicksSequently();
    }
}
