using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UserInputController))]
public class UserInputController_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = target as UserInputController;

        if (GUILayout.Button("New life game"))
            myScript.SetUpNewLifeGame();
        if (GUILayout.Button("New iteration"))
            myScript.SetUpNewLifeIteration();
        if (GUILayout.Button("Finish iteration"))
            myScript.RunTicksToTheEndOfIteration();
        if (GUILayout.Button("Skip iteration"))
            myScript.SkipIterations();
        if (GUILayout.Button("Tick"))
            myScript.RunTick();
        if (GUILayout.Button("Play"))
            myScript.PlayTicksSequently();
    }
}
