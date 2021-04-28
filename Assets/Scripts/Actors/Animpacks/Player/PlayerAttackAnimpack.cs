namespace Actors.Animpacks.Player {
    public class PlayerAttackAnimpack : AttackAnimpack {
        const float a1_left_start = 0;
        const float a1_left_hit = .433f;
        const float a1_left_end = .667f;
        const float a1_right_start = a1_left_end;
        const float a1_right_hit = .733f;
        const float a1_right_end = .933f;
        const float a1_end = 1.667f;
        const float a2_left_start = 0;
        const float a2_left_hit = .4f;
        const float a2_left_end = .667f;
        const float a2_down_start_combo = a2_left_end;
        const float a2_down_start_heavy = .967f;
        const float a2_down_start_light = 1.467f;
        const float a2_down_hit = 1.5f;
        const float a2_down_end = 1.8f;
        const float a2_end = 2.9f;

        readonly AttackAnimation[] animations = new AttackAnimation[] {
            new AttackAnimation(new AnimationData("Attack_01", a1_left_start, a1_left_hit, a1_left_end, a1_end, AttackDirection.Left, 6),
                new[] { AttackDirection.Left }),
            new AttackAnimation(new AnimationData("Attack_02", a2_left_start, a2_left_hit, a2_left_end, a2_end, AttackDirection.Left, 6),
                new[] { AttackDirection.Left }),
            new AttackAnimation(new AnimationData("Attack_01", a1_right_start, a1_right_hit, a1_right_end, a1_end, AttackDirection.Right, 4),
                new[] { AttackDirection.Right }),
            new AttackAnimation(new AnimationData("Attack_02", a2_down_start_light, a2_down_hit, a2_down_end, a2_end, AttackDirection.Down, 4),
                new[] { AttackDirection.Down }),
            new AttackAnimation(new AnimationData("Attack_02", a2_down_start_combo, a2_down_hit, a2_down_end, a2_end, AttackDirection.Down, 10),
                new[] { AttackDirection.Left, AttackDirection.Down }),
            new AttackAnimation(new AnimationData("Attack_02", a2_down_start_heavy, a2_down_hit, a2_down_end, a2_end, AttackDirection.Down, 10),
                new[] { AttackDirection.Down, AttackDirection.Down }),
        };

        readonly AnimationData knockOut = new AnimationData("Knock Out", 6.833f);
        readonly AnimationData death = new AnimationData("Death", 4.4f);

        public override AttackAnimation[] Animations => animations;
        public override AnimationData KnockOut => knockOut;
        public override AnimationData Death => death;
    }
}