using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LoadSongs : MonoBehaviour
{
    public string Path;
    public string WIPPath;
    public GameObject mapPrefab;
    public GameObject songPreview;
    public List<Texture> textures;

    void Start() {
        LoadSongsFromPath();
    }

    void LoadSongsFromPath() {
        string[] folders = Directory.GetDirectories(Path);
        for (int i = 0; i < folders.Length; i++) {
            string rawData = File.ReadAllText(folders[i] + "\\info.dat");
            info info = JsonUtility.FromJson<info>(rawData);
            if (File.Exists(folders[i] + "\\info.dat")) {
                GameObject go = Instantiate(mapPrefab); 
                go.transform.SetParent(songPreview.transform, false);
                go.transform.localScale = new Vector3(1,1,1);

                Texture2D texture = new Texture2D(2, 2);

                if (File.Exists(folders[i] + "\\" + info._coverImageFilename)) {
                    byte[] fileData = File.ReadAllBytes(folders[i] + "\\" + info._coverImageFilename);
                    texture.LoadImage(fileData);
                    go.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>().texture = texture;
                } else {
                    go.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>().texture = textures[0];
                }

                go.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = info._songName;
                go.transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = info._songAuthorName;
                go.transform.GetChild(0).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = folders[i];

                Debug.Log("Finished Loading Song From: " + folders[i]);
            }
        }
        Debug.Log("Successfully Finished Loading Songs From: " + Path);
    }
}

public class info {
    public string _version;
    public string _songName;
    public string _songSubName;
    public string _songAuthorName;
    public string _levelAuthorName;
    public float _beatsPerMinute;
    public float _shuffle;
    public float _shufflePeriod;
    public float _previewStartTime;
    public float _previewDuration;
    public string _songFilename;
    public string _coverImageFilename;
    public string _environmentName;
    public float _songTimeOffset;
    public _customData _customData;
}

public class _customData {
    public _editors _editors;
}

public class _editors {
    public string _lastEditedBy;
}
