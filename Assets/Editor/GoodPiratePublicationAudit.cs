using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GoodPirate113.Editor
{
    public static class GoodPiratePublicationAudit
    {
        public static void Run()
        {
            var failures = new List<string>();
            var records = new List<string> { "scene\tgameobjects\tmonobehaviours\tpersistent_callbacks\tmissing_scripts" };
            foreach (var entry in EditorBuildSettings.scenes)
            {
                if (!entry.enabled) continue;
                var scene = EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Single);
                int objects = 0, components = 0, callbacks = 0, missing = 0;
                foreach (var root in scene.GetRootGameObjects())
                foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                {
                    objects++;
                    missing += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
                    foreach (var component in transform.GetComponents<MonoBehaviour>())
                    {
                        if (component == null) continue;
                        components++;
                        var serialized = new SerializedObject(component);
                        var iterator = serialized.GetIterator();
                        while (iterator.Next(true))
                        {
                            if (!iterator.propertyPath.EndsWith(".m_MethodName", StringComparison.Ordinal) || iterator.propertyType != SerializedPropertyType.String || string.IsNullOrEmpty(iterator.stringValue)) continue;
                            var prefix = iterator.propertyPath.Substring(0, iterator.propertyPath.Length - "m_MethodName".Length);
                            var target = serialized.FindProperty(prefix + "m_Target")?.objectReferenceValue;
                            callbacks++;
                            bool resolved = false;
                            if (target != null)
                                foreach (var method in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                                    if (method.Name == iterator.stringValue) { resolved = true; break; }
                            if (!resolved) failures.Add(entry.path + ": " + component.name + ": " + iterator.stringValue);
                        }
                    }
                }
                if (missing != 0) failures.Add(entry.path + ": missing scripts=" + missing);
                records.Add($"{entry.path}\t{objects}\t{components}\t{callbacks}\t{missing}");
            }
            Directory.CreateDirectory("Logs");
            File.WriteAllLines("Logs/publication-scenes.tsv", records);
            File.WriteAllLines("Logs/publication-scene-failures.txt", failures);
            if (failures.Count > 0) throw new InvalidOperationException("Publication scene audit failed: " + string.Join(", ", failures));
            Debug.Log("Publication scene and callback audit passed");
        }
    }
}
