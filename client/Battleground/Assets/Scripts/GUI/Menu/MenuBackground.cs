using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    public static MenuBackground instance;

    // Start is called before the first frame update
    void Awake() {
        // Avoid having multiple instances of AudioManager
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Remove() {
        Destroy(gameObject);
    }
}
