using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LifeGameController))]
public class LifeGameController_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = target as LifeGameController;

        if (GUILayout.Button("New life game"))
            myScript.userInputController.SetUpNewLifeGame();
        if (GUILayout.Button("New iteration"))
            myScript.userInputController.SetUpNewLifeIteration();
        if (GUILayout.Button("Finish iteration"))
            myScript.userInputController.RunTicksToTheEndOfIteration();
        if (GUILayout.Button("Skip iteration"))
            myScript.userInputController.SkipIterations();
        if (GUILayout.Button("Play"))
            myScript.userInputController.PlayTicksSequently();
    }
}
