using System;

namespace Management
{
    public static class EventSystem
    {
        public static event Action<string> OnSceneLoaded;
        public static void Broadcast_OnSceneLoaded(string p_sceneName) => OnSceneLoaded?.Invoke(p_sceneName);
        public static event Action<string> OnSceneUnloaded;
        public static void Broadcast_OnSceneUnloaded(string p_sceneName) => OnSceneUnloaded?.Invoke(p_sceneName);
        public static event Action OnSceneGroupLoaded;
        public static void Broadcast_OnSceneGroupLoaded() => OnSceneGroupLoaded?.Invoke();
    }
}

