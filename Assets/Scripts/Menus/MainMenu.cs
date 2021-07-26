using Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Menu {
    public RelativeDolly[] dollies;
    public int dollySwitchTime = 4;
    public Transform cam;

    float startTime = float.NaN;

    public void StartGame() {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void Exit() => Application.Quit();

    void Awake() => Time.timeScale = .05f;

    void LateUpdate() {
        if (float.IsNaN(startTime))
            startTime = Time.unscaledTime;
        int dolly = (int)(Time.unscaledTime - startTime) / dollySwitchTime % dollies.Length;
        cam.SetPositionAndRotation(dollies[dolly].transform.position, dollies[dolly].transform.rotation);
    }
}