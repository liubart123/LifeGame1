using Assets.Game.Scripts.LifeGame;
using Assets.Game.Scripts.LifeGame.Environment;
using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Units.Brain;
using Assets.Game.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// Contains all for game life playing
/// </summary>
public class LifeGameController : SingletonBehavior<LifeGameController>
{
    public int mapWidth = 100, mapHeight = 100;
    public int[] neuronsNumberInLayers = new int[] {5,6,4};
    public int countOfTicksInIteration = 50;
    public int countOfUnits = 200, countOfAncestors = 10;
    public int currentIterationCount = 0;

    public UserInputController userInputController;

    public void SetUpNewLifeGame()
    {
        NeuronController.Instance.Initialize(neuronsNumberInLayers);
        GenerationController.Instance.Initialize(neuronsNumberInLayers, countOfUnits, countOfAncestors);
        EnvironmentController.Instance.Initialize();
        LifeTickController.Instance.Initialize();
        LifeGameEngine.Instance.Initialize();
        NeuronActionController.Instance.Initialize();

        MapController.Instance.CreateMep(mapWidth, mapHeight);
        PopulationController.Instance.SetUpRandomPopulation();
        EnvironmentController.Instance.CreateAndPlaceObstaclesOnMap();
        EnvironmentController.Instance.PlaceUnitsOnMap();
        LifeTickController.Instance.Reset();

        currentIterationCount = 0;
    }

    public void SetUpNewLifeIteration()
    {
        Profiler.BeginSample("SetUpNewIter");
        if (LifeTickController.Instance.currentTickCount < countOfTicksInIteration)
            return;
        MapController.Instance.ResetMap();
        PopulationController.Instance.SetUpNewGenerationPopulation();
        EnvironmentController.Instance.CreateAndPlaceObstaclesOnMap();
        EnvironmentController.Instance.PlaceUnitsOnMap();
        LifeTickController.Instance.Reset();
        currentIterationCount++;
        Profiler.EndSample();
    }

    public void RunTicksToTheEndOfIteration()
    {
        while (LifeTickController.Instance.currentTickCount < countOfTicksInIteration)
            RunTick();
    }

    public void RunTick()
    {
        if (LifeTickController.Instance.currentTickCount >= countOfTicksInIteration)
            return;
        LifeTickController.Instance.RunTick();
    }

    public void RunMultipleIterations(int iterations)
    {
        for(int i = 0; i < iterations; i++)
        {
            RunTicksToTheEndOfIteration();
            SetUpNewLifeIteration();
        }
    }
}
