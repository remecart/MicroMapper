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
    public Info mapInfo;
    void Start()
    {
        instance = this;

        string rawData = File.ReadAllText(Path.Combine(FileManager.instance.GetBeatSaberPath(), "info.dat"));
        mapInfo = JsonUtility.FromJson<Info>(rawData);

        Debug.Log(mapInfo._songFilename);
    }
    
    public void LoadMap(string path)
    {
        mapPath = path;
    }
    
    
    public void SetAudioClip(AudioSource source)
    {
        string songPath = Path.Combine(FileManager.instance.GetBeatSaberPath(), mapInfo._songFilename);
        Debug.Log(songPath + " - " + GetAudioTypeFromExtension(songPath));

        Debug.Log(songPath + " <- songPath");
        StartCoroutine(LoadAudioClip(songPath, source, GetAudioTypeFromExtension(songPath)));
    }
    
    private AudioType GetAudioTypeFromExtension(string path)
    {
        string extension = System.IO.Path.GetExtension(path).ToLower();
        switch (extension)
        {
            case ".ogg":
                return AudioType.OGGVORBIS;
            case ".egg":
                return AudioType.OGGVORBIS;
            case ".wav":
                return AudioType.WAV;
            default:
                Debug.LogWarning("Unknown audio type for extension: " + extension);
                return AudioType.UNKNOWN;
        }
    }

    private IEnumerator LoadAudioClip(string path, AudioSource src, AudioType audioType)
    {
        string url = "file:///" + path;

        Debug.Log("Loading audio from URL: " + url); // Log the URL to debug

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading audio: " + www.error);
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
