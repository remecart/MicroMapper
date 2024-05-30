using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

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
    
    
    public void SetAudioClip(AudioSource source)
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

        StartCoroutine(LoadAudioClip(Path.Combine(mapPath, songName),source));
    }
    
    private IEnumerator LoadAudioClip(string path,AudioSource src)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                src.clip = clip;
            }
        }
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
