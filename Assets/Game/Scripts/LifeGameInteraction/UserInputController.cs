using Assets.Game.Scripts.LifeGame;
using Assets.Game.Scripts.LifeGame.Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInputController : MonoBehaviour
{
    LifeGameController lifeGameController = LifeGameController.Instance;
    MapController mapController = MapController.Instance;
    MapRenderer mapRenderer;
    public TMP_InputField tickNoInputField;
    public TMP_InputField iterationsNumberToSkipField;
    public TMP_InputField iterationsNumberField;

    public string CountOfUnits {
        set
        {
            int.TryParse(value, out lifeGameController.countOfUnits);
        }
    }
    public string PercentOfAncestors
    {
        set
        {
            int percent;
            if (int.TryParse(value, out percent))
                lifeGameController.countOfAncestors = (int)(lifeGameController.countOfUnits * percent / 100f);
        }
    }
    public string SizeOfMap
    {
        set
        {
            int.TryParse(value, out lifeGameController.mapWidth);
            int.TryParse(value, out lifeGameController.mapHeight);
        }
    }
    public string CountOfTicks
    {
        set
        {
            int.TryParse(value, out lifeGameController.countOfTicksInIteration);
        }
    }

    void UpdateView()
    {
        UpdateIterationNo();
        UpdateTickNo();
        mapRenderer.RenderMap(mapController);
    }
    void UpdateTickNo()
    {
        tickNoInputField.text = LifeTickController.Instance.currentTickCount.ToString();
    }
    void UpdateIterationNo()
    {
        iterationsNumberField.text = lifeGameController.currentIterationCount.ToString();
    }
    public void SetUpNewLifeGame()
    {
        lifeGameController.SetUpNewLifeGame();
        mapRenderer.CreateMap(mapController);
        UpdateView();
    }
    public void SetUpNewLifeIteration()
    {
        lifeGameController.SetUpNewLifeIteration();
        UpdateView();
    }
    public void RunTicksToTheEndOfIteration()
    {
        lifeGameController.RunTicksToTheEndOfIteration();
        UpdateView();
    }
    public void RunTick()
    {
        lifeGameController.RunTick();
        UpdateView();
    }
    public void SkipIterations()
    {
        int iterations = 0;
        if (!int.TryParse(iterationsNumberToSkipField.text, out iterations))
            return;
        lifeGameController.RunMultipleIterations(iterations);
        UpdateView();
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

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PlayTicksSequently();
    }
}
