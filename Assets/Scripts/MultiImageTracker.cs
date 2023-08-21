using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultiImageTracker : MonoBehaviour
{
    public ARTrackedImageManager arTrackedImageManager;
    public GameObject[] arPrefabs;

    public Dictionary<string, GameObject> spawnObjects;

    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public Dictionary<string, AudioClip> spawnClips;
    // Start is called before the first frame update
    void Start()
    {
        spawnObjects = new Dictionary<string, GameObject>();
        spawnClips = new Dictionary<string, AudioClip>();

        for (int i = 0; i < arPrefabs.Length; i++) 
        {
            var spawnObj = Instantiate(arPrefabs[i]);
            spawnObj.SetActive(false);
            spawnObj.name = arPrefabs[i].name;
            spawnObjects.Add(spawnObj.name, spawnObj);
            spawnClips.Add(spawnObj.name, audioClips[i]);

            Debug.Log($"Initialize Success");
        }
    }

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }
    void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs arg)
    {
        foreach(var trackedImg in arg.added)
        {
            AddSpawnObject(trackedImg);
        }

        foreach (var trackedImg in arg.updated)
        {
            UpdateSpawnObject(trackedImg);
        }

        foreach (var trackedImg in arg.removed)
        {
            RemoveSpawnObject(trackedImg);
        }
    }

    void AddSpawnObject(ARTrackedImage trackedImg)
    {
        var refImgName = trackedImg.referenceImage.name;

        spawnObjects[refImgName].transform.position = trackedImg.transform.position;
        spawnObjects[refImgName].transform.rotation = trackedImg.transform.rotation;
        spawnObjects[refImgName].SetActive(true);

        if (audioSource.clip == null || audioSource.clip != spawnClips[refImgName])
        {
            audioSource.clip = spawnClips[refImgName];
            audioSource.Play();
            Debug.Log($"The clip {spawnClips[refImgName].name} is playing");
        }
    }

    void UpdateSpawnObject(ARTrackedImage trackedImg)
    {
        var refImgName = trackedImg.referenceImage.name;

        spawnObjects[refImgName].transform.position = trackedImg.transform.position;
        spawnObjects[refImgName].transform.rotation = trackedImg.transform.rotation;
    }

    void RemoveSpawnObject(ARTrackedImage trackedImg)
    {
        var refImgName = trackedImg.referenceImage.name;
        spawnObjects[refImgName].SetActive(false);
        Debug.Log($"ADDDDD: {refImgName} had been removed");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"There are {arTrackedImageManager.trackables.count} images being tracked");

        //foreach(var trackedImg in arTrackedImageManager.trackables)
        //{
        //    Debug.Log($"Image: {trackedImg.referenceImage.name} is at {trackedImg.transform.position}");
        //}
    }
}

