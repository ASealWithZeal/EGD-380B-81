using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldGenerator generator = (WorldGenerator)target;
        SerializedProperty property;

        EditorGUILayout.LabelField("Floor");
        generator.floor = (GameObject)EditorGUILayout.ObjectField("Floor", generator.floor, typeof(GameObject), true);
        property = serializedObject.FindProperty("floorMats");
        EditorGUILayout.PropertyField(property, new GUIContent("Floor Mats"), true);
        property = serializedObject.FindProperty("floorMatChances");
        EditorGUILayout.PropertyField(property, new GUIContent("Floor Mat Chances"), true);

        EditorGUILayout.LabelField("Walls");
        generator.wall = (GameObject)EditorGUILayout.ObjectField("Wall", generator.wall, typeof(GameObject), true);
        property = serializedObject.FindProperty("wallMats");
        EditorGUILayout.PropertyField(property, new GUIContent("Wall Mats"), true);
        property = serializedObject.FindProperty("wallMatChances");
        EditorGUILayout.PropertyField(property, new GUIContent("Wall Mat Chances"), true);

        EditorGUILayout.LabelField("Liquid");
        generator.liquid = (GameObject)EditorGUILayout.ObjectField("Liquid", generator.liquid, typeof(GameObject), true);
        property = serializedObject.FindProperty("liquidMats");
        EditorGUILayout.PropertyField(property, new GUIContent("Liquid Mats"), true);
        property = serializedObject.FindProperty("liquidMatChances");
        EditorGUILayout.PropertyField(property, new GUIContent("Liquid Mat Chances"), true);


        EditorGUILayout.LabelField("Rock");
        generator.rock = (GameObject) EditorGUILayout.ObjectField("Rock", generator.rock, typeof(GameObject), true);
        generator.rockPrefab = (GameObject)EditorGUILayout.ObjectField("Rock Prefab", generator.rockPrefab, typeof(GameObject), true);
        property = serializedObject.FindProperty("rockMats");
        EditorGUILayout.PropertyField(property, new GUIContent("Rock Mats"), true);
        property = serializedObject.FindProperty("rockMatChances");
        EditorGUILayout.PropertyField(property, new GUIContent("Rock Mat Chances"), true);
        generator.rockSpawnChance = EditorGUILayout.FloatField("Spawn Chance", generator.rockSpawnChance);

        if (GUILayout.Button("Generate Environment"))
        {
            generator.CreateEnvironmentMats();
        }
    }
}
