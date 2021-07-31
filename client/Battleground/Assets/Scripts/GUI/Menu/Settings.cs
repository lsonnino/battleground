using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public Slider musicSlider;
    public Text musicValue;
    public Slider soundSlider;
    public Text soundValue;

    void Start() {
        musicSlider.value = ((int) (100 * User.currentUser.musicVolume));
        musicValue.text = "" + musicSlider.value + "%";
        soundSlider.value = ((int) (100 * User.currentUser.soundVolume));
        soundValue.text = "" + soundSlider.value + "%";
    }

    public void OnValuesChanged() {
        musicValue.text = "" + musicSlider.value + "%";
        soundValue.text = "" + soundSlider.value + "%";

        User.currentUser.musicVolume = musicSlider.value / 100f;
        User.currentUser.soundVolume = soundSlider.value / 100f;

        User.currentUser.Save();

        FindObjectOfType<MusicManager>().SetVolume(User.currentUser.musicVolume);
        FindObjectOfType<AudioManager>().SetVolume(User.currentUser.soundVolume);
    }

    public void Back() {
        SceneManager.LoadScene("Scenes/Main Menu");
    }
}
