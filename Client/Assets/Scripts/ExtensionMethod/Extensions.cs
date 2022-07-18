using Google.Protobuf.Protocol;
using System;

namespace Extensions {
    public static class ActionEx {
        public static void AddListener(this Action<int, bool> action, Action<int, bool> param) {
            RemoveListener(action, param);
            action += param;
        }

        public static void RemoveListener(this Action<int, bool> action, Action<int, bool> param) {
            action -= param;
        }
    }
}