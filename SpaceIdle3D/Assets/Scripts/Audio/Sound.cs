using UnityEngine;

[System.Serializable]
public class Sound
{
    [HideInInspector] public AudioSource source;
    public string name;
    public AudioClip clip;
    public bool loop = false;
    [Range(0f, 1f)] public float volume;
    [Range(.5f, 2f)] public float pitch;
}