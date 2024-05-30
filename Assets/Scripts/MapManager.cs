using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    private string mapPath;
    private Info mapInfo;
    void Start()
    {
        instance = this;
    }
    
    public void LoadMap(string path)
    {
        mapPath = path;
    }
    
    public AudioClip GetAudioClip()
    {
        var songName = "song.ogg";
        foreach (var file in Directory.GetFiles(mapPath))
        {
            var ext = Path.GetExtension(file);
            
            if (ext == ".ogg" || ext == ".wav" || ext == ".egg")
            {
                songName = Path.GetFileName(file);
                break;
            }
        }
        
        return Resources.Load<AudioClip>(Path.Combine(mapPath,songName));
    }
    
    public Info GetMapInfo()
    {
        if(Directory.GetFiles(mapPath).Contains("info.dat") && mapInfo == null)
        {
            mapInfo = JsonUtility.FromJson<Info>(File.ReadAllText(Path.Combine(mapPath, "info.dat")));
        }
        
        if (mapInfo == null)
        {
            var infoPath = Path.Combine(mapPath, "Info.dat");
            if (File.Exists(infoPath))
            {
                mapInfo = JsonUtility.FromJson<Info>(File.ReadAllText(infoPath));
            }else
            {
                throw new Exception("There was no info.dat file in the map folder."); 
            }

        }
        return mapInfo;
    }
}
