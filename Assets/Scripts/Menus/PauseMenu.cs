using Actors;
using Cavern;
using UnityEngine;

namespace Menus {
    public class PauseMenu : Menu {
        public GameObject pauseObject;
        public Actor[] actors;
        public Transform[] camTargets;
        public Transform returnTarget;

        float targetVolume = 1;

        void SetPause(bool state) {
            active = state;
            pauseObject.SetActive(state);
            Time.timeScale = state ? 0 : 1;
            targetVolume = state ? .25f : 1;
            for (int i = 0; i < actors.Length; ++i)
                actors[i].enabled = !state;
        }

        public void Continue() {
            camTarget = returnTarget;
            SetPause(false);
        }

        public void Exit() => Application.Quit();

        new void Start() {
            active = false;
            camTarget = returnTarget;
            base.Start();
        }

        new void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
                if (active)
                    Continue();
                else
                    SetPause(true);
            if (active)
                camTarget = camTargets[Selected];
            AudioListener3D.Current.Volume = Mathf.Lerp(AudioListener3D.Current.Volume, targetVolume, 10 * time.DeltaTime);
            base.Update();
        }
    }
}