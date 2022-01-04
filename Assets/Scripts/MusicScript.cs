using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public static MusicScript MusicInstance;

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }
}
