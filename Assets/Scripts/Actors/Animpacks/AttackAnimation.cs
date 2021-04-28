namespace Actors.Animpacks {
    public class AttackAnimation {
        public AnimationData Data { get; }

        public AttackDirection[] Combo;

        public AttackAnimation(AnimationData data, AttackDirection[] combo) {
            Data = data;
            Combo = combo;
        }
    }
}