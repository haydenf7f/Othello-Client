using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

using System.Linq;

public class BackgroundMusic : MonoBehaviour {
    private static BackgroundMusic instance = null;
    private AudioSource audioSource;
    private List<AudioClip> clips;

    void Awake()
    {
        // Check if there is already an instance of BackgroundMusic
        if (instance != null && instance != this)
        {
            // Destroy the game object since it is a duplicate
            Destroy(this.gameObject);
            return;
        }

        // Set the instance to this instance
        instance = this;

        // Make the game object persistent across scenes
        DontDestroyOnLoad(this.gameObject);

        audioSource = GetComponent<AudioSource>();
        clips = LoadClipsFromFolder("Music");

        // Shuffle the list of clips
        clips = clips.OrderBy(x => Random.value).ToList();

        PlayNextClip();
    }

    void PlayNextClip() {
        // If there are no more clips, shuffle and start over
        if (clips.Count == 0) {
            clips = clips.OrderBy(x => Random.value).ToList();
        }

        // Play the next clip and remove it from the list
        audioSource.clip = clips[0];
        audioSource.Play();
        clips.RemoveAt(0);

        // Call this function again after the clip finishes playing
        Invoke("PlayNextClip", audioSource.clip.length);
    }

    List<AudioClip> LoadClipsFromFolder(string folderPath) {
        List<AudioClip> clips = new List<AudioClip>();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + folderPath);

        foreach (string filePath in fileEntries) {
            // Ignore meta files
            if (filePath.EndsWith(".meta")) {
                continue;
            }
            else if(filePath.EndsWith(".mp3") || filePath.EndsWith(".wav")) {
                // Load the clip and add it to the list
                UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.UNKNOWN);
                www.SendWebRequest();
                while (!www.isDone) { }
                if (www.result == UnityWebRequest.Result.Success) {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    clips.Add(clip);
                }
            }
        }

        return clips;
    }
}
