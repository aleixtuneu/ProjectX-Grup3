using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource sfxAudioSourcePrefab; // Prefab for 3D event sounds

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play non-spatialized event SFX globally (e.g., UI clicks)
    public void Play(AudioClip clip)
    {
        if (clip != null && Camera.main != null)
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, 1f);
    }

    // Override to play spatialized event SFX at a position in world
    public void Play(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            AudioSource newSource = Instantiate(sfxAudioSourcePrefab, position, Quaternion.identity);
            newSource.clip = clip;
            newSource.spatialBlend = 1f;
            newSource.minDistance = 5f; // max volume radius
            newSource.maxDistance = 20f; // distance where volume fades to 0

            newSource.Play();
            Destroy(newSource.gameObject, clip.length);
        }
    }
    
    // Override to play spatialized event SFX at a position in world with a custom radius
    public void Play(AudioClip clip, Vector3 position, float minDistance, float maxDistance)
    {
        if (clip == null)
        {
            AudioSource newSource = Instantiate(sfxAudioSourcePrefab, position, Quaternion.identity);
            newSource.clip = clip;
            newSource.spatialBlend = 1f;
            newSource.minDistance = minDistance; // max volume radius
            newSource.maxDistance = maxDistance; // distance where volume fades to 0

            newSource.Play();
            Destroy(newSource.gameObject, clip.length);
        }
    }
}