using Actors;
using Cavern;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    [RequireComponent(typeof(AudioSource3D))]
    public class Announcer : MonoBehaviour {
        // TODO: game over after final round, full restart or back to menu (level select?)

        /// <summary>
        /// Round announcement sounds for all rounds.
        /// </summary>
        [Tooltip("Round announcement sounds for all rounds.")]
        public AudioClip[] rounds;

        /// <summary>
        /// Round announcement sound count for each round.
        /// </summary>
        [Tooltip("Round announcement sound count for each round.")]
        public int[] clipsPerRound;

        /// <summary>
        /// Round announcement sounds for round endings.
        /// </summary>
        [Tooltip("Round announcement sounds for round endings.")]
        public AudioClip[] kos;

        /// <summary>
        /// Round announcement sound count for each round's ending.
        /// </summary>
        [Tooltip("Round announcement sound count for each round's ending.")]
        public int[] clipsPerKO;

        /// <summary>
        /// Time out announcement sounds.
        /// </summary>
        [Tooltip("Time out announcement sounds.")]
        public AudioClip[] timeOuts;

        /// <summary>
        /// Time out announcement sound count for each voice line.
        /// </summary>
        [Tooltip("Time out announcement sound count for each voice line.")]
        public int[] clipsPerTimeOut;

        public AudioClip[] victorySounds;

        // TODO: death sounds by the same logic

        /// <summary>
        /// A fight is in progress.
        /// </summary>
        public bool Playing { get; set; }

        public Actor fighterLeft;
        public GameObject[] fighterLeftRoundsWon;
        public Actor fighterRight;
        public GameObject[] fighterRightRoundsWon;
        public int roundCount = 3;
        public int roundTime = 30;
        public Text roundTimeText;
        public AnnouncementText roundText;

        AudioSource3D source;

        int fighter1Wins, fighter2Wins, timeRemaining;
        float roundCooldown = float.NaN, koCooldown = float.NaN, timeOutCooldown = float.NaN, timerCooldown = 1;

        int TimeRemaining {
            get => timeRemaining;
            set => roundTimeText.text = (timeRemaining = value).ToString();
        }

        AudioClip GetClip(AudioClip[] set, int[] perRound, int roundOverride = -1) {
            int round = fighter1Wins + fighter2Wins;
            if (fighter1Wins == roundCount - 1 && fighter2Wins == roundCount - 1)
                round = clipsPerRound.Length - 1;
            if (roundOverride != -1)
                round = roundOverride;
            int skipClips = 0;
            for (int i = 0; i < round; ++i)
                skipClips += perRound[i];
            return set[Random.Range(skipClips, skipClips + perRound[round])];
        }

        void PrepareNextRound() {
            fighterLeft.Health = fighterLeft.maxHealth;
            fighterRight.Health = fighterRight.maxHealth;
            AudioClip announcement = GetClip(rounds, clipsPerRound);
            if (fighter1Wins == roundCount - 1 && fighter2Wins == roundCount - 1)
                roundText.NewText("Final Round");
            else
                roundText.NewText("Round " + (fighter1Wins + fighter2Wins + 1));
            source.PlayOneShot(announcement);
            roundCooldown = announcement.length;
            TimeRemaining = roundTime;
        }

        public void ActorHit(Actor victim) {
            const float maxShake = .25f, hitScale = .05f;
            Vector3 cameraShake = hitScale *
                new Vector3(victim == fighterLeft ? -1 : 1, Random.Range(-maxShake, maxShake), Random.Range(-maxShake, maxShake));
            Transform cam = Camera.main.transform;
            cam.position += cam.rotation * cameraShake;
        }

        public void Lost(Actor loser) {
            Playing = false;
            AudioClip announcement = GetClip(kos, clipsPerKO);
            source.PlayOneShot(announcement);
            koCooldown = announcement.length;
            if (loser == fighterLeft)
                fighterRightRoundsWon[fighter2Wins++].SetActive(true);
            else
                fighterLeftRoundsWon[fighter1Wins++].SetActive(true);
            timerCooldown = 1;
            if (fighter1Wins == roundCount || fighter2Wins == roundCount) {
                source.PlayOneShot(victorySounds[Random.Range(0, victorySounds.Length)]);
                loser.Died();
            } else
                loser.KnockedOut();
        }

        void Start() {
            fighterLeft.Handler = fighterRight.Handler = this;
            source = GetComponent<AudioSource3D>();
            PrepareNextRound();
        }

        void Update() {
            if (!float.IsNaN(roundCooldown) && (roundCooldown -= Time.deltaTime) <= 0) {
                roundCooldown = float.NaN;
                Playing = true;
            }
            if (!float.IsNaN(koCooldown) && (koCooldown -= Time.deltaTime) <= 0) {
                koCooldown = float.NaN;
                if (fighter1Wins != roundCount && fighter2Wins != roundCount)
                    PrepareNextRound();
            }
            if (!float.IsNaN(timeOutCooldown) && (timeOutCooldown -= Time.deltaTime) <= 0) {
                timeOutCooldown = float.NaN;
                if (fighterLeft.Health == fighterRight.Health)
                    PrepareNextRound();
                else if (fighterLeft.Health < fighterRight.Health)
                    Lost(fighterLeft);
                else
                    Lost(fighterRight);
            }
            if (Playing) {
                timerCooldown -= Time.deltaTime;
                if (timerCooldown < 0) {
                    ++timerCooldown;
                    if (--TimeRemaining == 0) {
                        Playing = false;
                        AudioClip announcement = GetClip(timeOuts, clipsPerTimeOut, Random.Range(0, clipsPerTimeOut.Length));
                        source.PlayOneShot(announcement);
                        timeOutCooldown = announcement.length;
                    }
                }
            }
        }
    }
}