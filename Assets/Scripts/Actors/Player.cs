using Actors.Animpacks;
using Actors.Animpacks.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Actors {
    public class Player : Actor {
        readonly List<AttackDirection> combo = new List<AttackDirection>();
        readonly PlayerAttackAnimpack attackAnimpack = new PlayerAttackAnimpack();

        protected override string IdleState => "Idle";

        Controls controls;

        public override void Attack(AttackDirection direction) {
            if (!IsReadyForAttack) {
                combo.Clear();
                return;
            }
            if (CurrentAttack == AttackDirection.None)
                combo.Clear();
            combo.Add(direction);
            AttackAnimation anim = attackAnimpack.GetAttack(combo);
            if (anim != null)
                Animate(anim.Data);
            else
                combo.Clear();
        }

        public override bool Attacked(AttackDirection direction, float damage) {
            Health -= damage;
            if (Health <= 0) {
                Health = 0;
                if (Handler)
                    Handler.Lost(this);
                // TODO: death animation
            }
            return true;
        }

        public override void KnockedOut() => Animate(attackAnimpack.KnockOut);

        public override void Died() => Animate(attackAnimpack.Death);

        protected override void ActorStart() => controls = GetComponent<Controls>();

        protected override void ActorUpdate() {
            if (Handler && !Handler.Playing)
                return;
            if (controls) {
                if (Input.GetKeyDown(controls.up))
                    Attack(AttackDirection.Up);
                if (Input.GetKeyDown(controls.left))
                    Attack(AttackDirection.Left);
                if (Input.GetKeyDown(controls.down))
                    Attack(AttackDirection.Down);
                if (Input.GetKeyDown(controls.right))
                    Attack(AttackDirection.Right);
            }
        }
    }
}