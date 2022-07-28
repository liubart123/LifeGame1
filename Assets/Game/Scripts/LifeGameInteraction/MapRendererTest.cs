using Assets.Game.Scripts.LifeGame;
using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRendererTest : MonoBehaviour
{
    public int numberOfNeuronsForMutation = 1;
    public MapRenderer mapRenderer;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RenderMap());
        }
    }

    IEnumerator RenderMap()
    {
        yield return null;
        int width = 100;
        int height = 100;
        MapController map = new MapController();
        map.CreateMep(width, height);

        Unit baseUnit = new Unit();
        GenerationController.Instance.GenerateNewSynopsesForUnit(baseUnit);
        GenerationController.Instance.chanceOfMutationForUnit = 1;
        GenerationController.Instance.numberOfNeuronsForMutation = numberOfNeuronsForMutation;

        for (int x = 0; x < width; x++)
        {
            //GenerationController.Instance.MutateUnit(baseUnit);
            Unit tempUnit = new Unit();
            GenerationController.Instance.CopySynopsesOfAncestorToDescendant(baseUnit, tempUnit);
            for (int y = 0; y < height; y++)
            {
                for(int i=0;i< numberOfNeuronsForMutation; i++)
                {
                    GenerationController.Instance.MutateSynops(tempUnit);
                }
                Unit tempUnit2 = new Unit();
                GenerationController.Instance.CopySynopsesOfAncestorToDescendant(tempUnit, tempUnit2);
                map.GetCell(x,y).unit = tempUnit2;
            }
        }

        mapRenderer.RenderMap(map);
    }
}
