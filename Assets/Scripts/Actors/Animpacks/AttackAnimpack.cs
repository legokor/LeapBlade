using System.Collections.Generic;
using UnityEngine;

namespace Actors.Animpacks {
    public abstract class AttackAnimpack {
        public abstract AttackAnimation[] Animations { get; }
        public abstract AnimationData KnockOut { get; }
        public abstract AnimationData Death { get; }

        static bool ArrayEndsWith<T>(T[] arr, List<T> ending) {
            if (arr.Length < ending.Count)
                return false;
            for (int i = 0, offset = arr.Length - ending.Count; i < ending.Count; ++i)
                if (!arr[i + offset].Equals(ending[i]))
                    return false;
            return true;
        }

        public AttackAnimation GetAttack(List<AttackDirection> combo) {
            List<AttackAnimation> candidates = new List<AttackAnimation>();

            for (int i = 0; i < Animations.Length; ++i)
                if (ArrayEndsWith(Animations[i].Combo, combo))
                    candidates.Add(Animations[i]);

            int min = int.MaxValue;
            for (int i = 0, c = candidates.Count; i < c; ++i)
                if (min > candidates[i].Combo.Length)
                    min = candidates[i].Combo.Length;
            candidates.RemoveAll(x => x.Combo.Length > min);

            if (candidates.Count == 0)
                return null;
            return candidates[Random.Range(0, candidates.Count)];
        }
    }
}