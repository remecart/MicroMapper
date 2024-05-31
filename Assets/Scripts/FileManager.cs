using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static FileManager instance;

    [SerializeField]
    private string beatSaberPath;
    
    private string customLevelFolderPath = "Beat Saber_Data\\CustomLevels";
    
    void Start()
    {
        instance = this;
    }
    
    public string GetBeatSaberPath() {
        return beatSaberPath;
    }
    
    public bool IsValidBeatSaberPath() {
        return Directory.Exists(beatSaberPath) && Directory.GetDirectories(beatSaberPath).Contains("Beat Saber_Data");
    }
    
    public string GetCustomLevelFolderPath() {
        return Path.Combine(GetBeatSaberPath(), customLevelFolderPath);
    }
    
    public string GetMapPath(string songName) {
        return Path.Combine(GetCustomLevelFolderPath(),songName);
        
    }
    
    public string[] GetAllFilesInMapFolder(string songName) {
        return Directory.GetFiles(GetMapPath(songName));
    }

    public bool IsDifficultyContained(string songPath, ObjectManager._difficulty difficulty) {
        return Directory.GetFiles(songPath).Contains(difficulty.ToString() + ".dat");
    }

    public ObjectManager._difficulty ParseDifficulty(string difficultyString)
    {
        difficultyString = difficultyString.Replace("Standard","");
        return ObjectManager._difficulty.TryParse(difficultyString, out ObjectManager._difficulty difficulty) ? difficulty : ObjectManager._difficulty.Easy;
    }
    
    public ObjectManager._difficulty[] GetDifficulties(string songPath) {
        List<ObjectManager._difficulty> difficulties = new List<ObjectManager._difficulty>();
        foreach (string file in Directory.GetFiles(songPath)) {
            if (Path.GetExtension(file).Equals(".dat")) {
                difficulties.Add(ParseDifficulty(Path.GetFileNameWithoutExtension(file)));
            }
        }
        return difficulties.ToArray();
    }
    
    public string GetDifficultyFileContent(ObjectManager._difficulty difficulty, string songPath) {
        var combine = Path.Combine(songPath, difficulty.ToString() + "Standard" + ".dat");

        return File.ReadAllText(combine);
    }


}
