using Actors;
using Cavern;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Menus {
    public class PauseMenu : Menu {
        public GameObject pauseObject;
        public Actor[] actors;
        public Transform[] camTargets;
        public Transform returnTarget;

        void SetPause(bool state) {
            active = state;
            pauseObject.SetActive(state);
            Time.timeScale = state ? 0 : 1;
            AudioListener3D.Current.Volume = state ? .25f : 1;
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
            base.Update();
        }
    }
}