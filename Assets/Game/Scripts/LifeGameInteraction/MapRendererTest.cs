using Assets.Game.Scripts.LifeGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRendererTest : MonoBehaviour
{
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
        //LifeGameController lg = LifeGameController.GetInstance();
        //lg.SetUpNewLifeIteration();
        //mapRenderer.RenderMap(lg.mapController);
        //while (true)
        //{

        //    mapRenderer.RenderMap(lg.mapController);
        //    yield return null;
        //}
    }
}
