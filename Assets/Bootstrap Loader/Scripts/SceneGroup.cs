using Eflatun.SceneReference;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SceneManagement
{
    public enum SceneType
    {
        ActiveScene,
        MainMenu,
        UserInterface,
        HUD,
        Tooling
    }

    [Serializable]
    public class SceneGroup
    {
        public string groupName = "New Scene Group";
        public List<SceneData> scenes;

        public string FindSceneNameByType(SceneType type)
        {
            return scenes.FirstOrDefault(scene => scene.SceneType == type)?.Name;
        }
    }

    [Serializable]
    public class SceneData {
        public SceneReference reference;
        public string Name => reference.Name;
        public SceneType SceneType;
    }
}

