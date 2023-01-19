using System;

namespace vsSurvivors.Utils {
    public static class StringExtensions {
        public static void RemoveComponent<T>(this GameObject gameObject) where T : Component {
            GameObject.Destroy(gameObject.GetComponent<T>());
        }

        public static void Add(this string str, string desc) {
            LanguageAPI.Add(str, desc);
        }

        public static void RemoveComponents<T>(this GameObject gameObject) where T : Component {
            T[] coms = gameObject.GetComponents<T>();
            for (int i = 0; i < coms.Length; i++) {
                GameObject.DestroyImmediate(coms[i]);
            }
        }

        public static T GetRandom<T>(this List<T> list, Xoroshiro128Plus rng = null) {
            if (rng == null) {
                return list[UnityEngine.Random.RandomRangeInt(0, list.Count)];
            }
            else {
                return list[rng.RangeInt(0, list.Count)];
            }
        }

        public static void AddComponent<T>(this Component self) where T : Component {
            self.gameObject.AddComponent<T>();
        }
    }
}