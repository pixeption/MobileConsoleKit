using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MobileConsole.Editor
{
    public static class AssetHelper
    {
        public static T LoadUniqueAsset<T>(string fileName = "") where T : ScriptableObject
        {
            var type = typeof(T);
            string filter = "t:" + type.Name;
            if (!string.IsNullOrEmpty(fileName))
            {
                filter += " " + fileName;
            }

            string[] guids = AssetDatabase.FindAssets(filter);
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var asset = AssetDatabase.LoadAssetAtPath(path, type);
                return (T)asset;
            }
            else
            {
                return default(T);
            }
        }

        public static string GetUniqueAssetPath(string fileName)
        {
            string[] filePaths = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories);
            foreach (var path in filePaths)
            {
                if (path.EndsWith(fileName))
                {
                    return path;
                }
            }

            return null;
        }

        public static string ConvertToUnityPath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath) || !absolutePath.StartsWith(Application.dataPath))
            {
                throw new Exception("Path is null or not inside Unity project folder");
            }

            return "Assets" + absolutePath.Substring(Application.dataPath.Length); ;
        }
    }
}