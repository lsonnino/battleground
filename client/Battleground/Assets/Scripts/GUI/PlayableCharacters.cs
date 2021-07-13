using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacters : MonoBehaviour
{

    // To be instantiated in Unity Editor with the characters prefab
    public GameObject Dino;
    public GameObject Gladiator;
    public GameObject Knight;
    public GameObject Ninja;
    public GameObject Pumpkin;
    public GameObject F_Zombie;
    public GameObject M_Zombie;

    // Will be automatically instantiated
    public GameObject[] playableCharacters;

    void Start() {
        playableCharacters = new GameObject[] {
           // Dino,
           Gladiator,
           Knight,
           Ninja,
           Pumpkin,
           F_Zombie,
           M_Zombie
       };
    }
    void Update() {}

    public Sprite GetWarriorImage(int index) {
        if (index < 0 || index > playableCharacters.Length) { return null; }

        return playableCharacters[index].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
    }
}
