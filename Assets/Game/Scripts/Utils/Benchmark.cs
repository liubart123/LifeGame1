using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Benchmark : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }
    }

    void Test()
    {
        //TestOperation(x => x = 2 + 2, "2+2");
        //TestOperation(x => x = x + 2, "x+2");
        //TestOperation(x => x = x + x, "x+x");
        //TestOperation(x => x = 2 * 2, "2*2");
        //TestOperation(x => x = x * 2, "x*2");
        //TestOperation(x => x = x * x, "x*x");
        //TestOperation(x => x = Mathf.Pow(2 % 1000, 2 % 1000), "2^2");
        //TestOperation(x => x = Mathf.Pow(x % 1000, 2 % 1000), "x^2");
        //TestOperation(x => x = Mathf.Pow(2 % 1000, x % 1000), "2^x");
        //TestOperation(x => x = Mathf.Pow(x % 1000, x % 1000), "x^x");

        float a;
        int b = 15;
        float[] arr1 = new float[20 * 20 * 20];
        float[,,] arr2 = new float[20, 20, 20];
        float[][][] arr3 = new float[20][][];
        for (int i = 0; i < 20; i++)
        {
            arr3[i] = new float[20][];
            for (int j = 0; j < 20; j++)
            {
                arr3[i][j] = new float[20];
            }
        }

        TestOperation(x => a = arr1[b * b * b], "arr1");
        TestOperation(x => a = arr2[b, b, b], "arr2");
        TestOperation(x => a = arr3[b][b][b], "arr3");
    }

    void TestOperation(Action<float> action, string text)
    {
        Stopwatch sw = new Stopwatch();

        sw.Start();
        for (int i = 0; i < 100000000; i++)
        {
            action(i%100);
        }
        sw.Stop();

        UnityEngine.Debug.Log(text + $" elapsed={sw.Elapsed}");
    }
}
