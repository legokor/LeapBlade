namespace Actors.Animpacks {
    public readonly struct AnimationData {
        /// <summary>
        /// Name of the animation clip.
        /// </summary>
        public string Clip { get; }

        /// <summary>
        /// Start position of the clip in seconds.
        /// </summary>
        public float Start { get; }

        /// <summary>
        /// Time in seconds when the enemy is hit.
        /// </summary>
        public float Hit { get; }

        /// <summary>
        /// End frame of the clip in seconds.
        /// </summary>
        public float End { get; }

        /// <summary>
        /// Length of the entire animation clip.
        /// </summary>
        public float ClipLength { get; }

        /// <summary>
        /// Attack direction of this clip.
        /// </summary>
        public AttackDirection Direction { get; }

        /// <summary>
        /// Attack damage.
        /// </summary>
        public float Damage { get; }

        public AnimationData(string clip, float clipLength) {
            Clip = clip;
            Start = 0;
            Hit = float.NaN;
            End = ClipLength = clipLength;
            Direction = AttackDirection.None;
            Damage = float.NaN;
        }

        public AnimationData(string clip, float start, float hit, float end, float clipLength, AttackDirection direction, float damage) {
            Clip = clip;
            Start = start;
            Hit = hit;
            End = end;
            ClipLength = clipLength;
            Direction = direction;
            Damage = damage;
        }
    }
}