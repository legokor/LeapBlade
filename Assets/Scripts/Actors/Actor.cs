using Actors.Animpacks;
using Actors.Soundpacks;
using Cavern;
using Game;
using UnityEngine;

namespace Actors {
    public enum AttackDirection {
        None,
        Up,
        Down,
        Left,
        Right
    }

    [RequireComponent(typeof(Animator), typeof(Soundpack), typeof(AudioSource3D))]
    public abstract class Actor : MonoBehaviour {
        /// <summary>
        /// Time before and after the animation end frame where a player can continue a combo.
        /// </summary>
        const float reflexTime = .2f;

        public Announcer Handler { get; set; }

        public float maxHealth = 100;

        public float Health {
            get => health;
            set {
                health = value;
                if (healthBar)
                    healthBar.SetHealth(health / maxHealth);
            }
        }
        float health;

        protected Animator anim;
        protected AudioSource3D source;
        protected Soundpack sounds;

        /// <summary>
        /// Idle animation state name.
        /// </summary>
        protected abstract string IdleState { get; }

        /// <summary>
        /// Gets if any animation is in progress that is not the idle animation.
        /// </summary>
        public bool Animated => !anim.GetCurrentAnimatorStateInfo(0).IsName(IdleState);

        HealthBar healthBar;

        /// <summary>
        /// Finish animating after this time if set.
        /// </summary>
        float animCutoff = float.NaN;

        /// <summary>
        /// Hit the enemy after this time if set.
        /// </summary>
        float hitCutoff = float.NaN;

        /// <summary>
        /// An extra time after the attack animation has finished, when the player can still perform a combo.
        /// </summary>
        float reflexCutoff = float.NaN;

        /// <summary>
        /// Damage for the currently playing attack animation that will be inflicted on the next hit.
        /// </summary>
        float nextDamage;

        public Actor enemy;

        /// <summary>
        /// Current attacking direction that is cast on the enemy after <see cref="hitCutoff"/>.
        /// </summary>
        protected AttackDirection CurrentAttack { get; private set; } = AttackDirection.None;

        /// <summary>
        /// Checks if the player can perform a new attack.
        /// </summary>
        protected bool IsReadyForAttack => float.IsNaN(animCutoff) || animCutoff <= reflexTime || !float.IsNaN(reflexCutoff);

        /// <summary>
        /// Called when this actor is attacked and returns if the attack was successful.
        /// </summary>
        public abstract bool Attacked(AttackDirection direction, float damage);

        /// <summary>
        /// Called when this actor attacks.
        /// </summary>
        public abstract void Attack(AttackDirection direction);

        /// <summary>
        /// Called when this actor has lost a round.
        /// </summary>
        public abstract void KnockedOut();

        /// <summary>
        /// Called when this actor was killed.
        /// </summary>
        public abstract void Died();

        /// <summary>
        /// Custom initialization for the actor.
        /// </summary>
        protected virtual void ActorStart() { }

        /// <summary>
        /// Custom updates for the actor, like player control or AI.
        /// </summary>
        protected abstract void ActorUpdate();

        void Start() {
            anim = GetComponent<Animator>();
            source = GetComponent<AudioSource3D>();
            sounds = GetComponent<Soundpack>();
            healthBar = GetComponent<HealthBar>();
            ActorStart();
        }

        protected void Animate(AnimationData clip) {
            anim.Play(clip.Clip, 0, clip.Start / clip.ClipLength);
            sounds.Play(source, sounds.attacks);
            if (!float.IsNaN(clip.Hit)) {
                animCutoff = clip.End - clip.Start;
                hitCutoff = clip.Hit - clip.Start;
                CurrentAttack = clip.Direction;
                nextDamage = clip.Damage;
            }
        }

        void Update() {
            if (!float.IsNaN(animCutoff)) {
                if ((animCutoff -= Time.deltaTime) <= 0) {
                    anim.CrossFade("Idle", .1f);
                    animCutoff = float.NaN;
                    reflexCutoff = reflexTime;
                }
            }
            if (!float.IsNaN(reflexCutoff)) {
                if ((reflexCutoff -= Time.deltaTime) <= 0) {
                    reflexCutoff = float.NaN;
                    if (float.IsNaN(animCutoff))
                        CurrentAttack = AttackDirection.None;
                }
            }
            if (!float.IsNaN(hitCutoff)) {
                if ((hitCutoff -= Time.deltaTime) <= 0) {
                    if (enemy && enemy.Attacked(CurrentAttack, nextDamage))
                        sounds.Play(source, sounds.hits);
                    hitCutoff = float.NaN;
                }
            }
            ActorUpdate();
        }
    }
}