using Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneGroupManager
    {
        SceneGroup activeSceneGroup;
        public async Task LoadAdditiveScenes(SceneGroup p_sceneGroup, IProgress<float> p_progress, bool reloadDuplicateScene = false)
        {
            activeSceneGroup = p_sceneGroup;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for(int i = 0; i<sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }
            
            var totalScenesToLoad = activeSceneGroup.scenes.Count;
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            //put all scenes inside async operations and load them additively
            for(var i =0; i<totalScenesToLoad; i++)
            {
                var sceneData = p_sceneGroup.scenes[i];
                if (reloadDuplicateScene == false && loadedScenes.Contains(sceneData.Name)) continue;
                var operation = SceneManager.LoadSceneAsync(sceneData.reference.Path, LoadSceneMode.Additive);
                await Task.Delay(TimeSpan.FromSeconds(2.5f));
                operationGroup.operations.Add(operation);
                EventSystem.Broadcast_OnSceneLoaded(sceneData.Name);
            }

            //wait until all operations in the group is done
            while (!operationGroup.IsDone)
            {
                p_progress?.Report(operationGroup.progress);
                await Task.Delay(100);
            }
            Scene activeScene = SceneManager.GetSceneByName(activeSceneGroup.FindSceneNameByType(SceneType.ActiveScene));
            if (activeScene.IsValid()) SceneManager.SetActiveScene(activeScene);

            await UnloadParticularScene("Bootstrapper");
            
            EventSystem.Broadcast_OnSceneGroupLoaded();
        }

        public async Task LoadSingleScene(SceneData p_scene, IProgress<float> p_progress)
        {
            var operationGroup = new AsyncOperationGroup(1);
            var operation = SceneManager.LoadSceneAsync(p_scene.reference.Path, LoadSceneMode.Single);
            await Task.Delay(TimeSpan.FromSeconds(2.5f));
            operationGroup.operations.Add(operation);
            EventSystem.Broadcast_OnSceneLoaded(p_scene.Name);

            while (!operationGroup.IsDone)
            {
                p_progress?.Report(operationGroup.progress);
                await Task.Delay(100);
            }
            Scene activeScene = SceneManager.GetSceneByName(p_scene.Name);
            if (activeScene.IsValid()) SceneManager.SetActiveScene(activeScene);

            await UnloadParticularScene("Bootstrapper");
        }

        public async Task UnloadScenes() {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;
            int sceneCount = SceneManager.sceneCount;

            for(int i = sceneCount -1; i >= 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if(!sceneAt.isLoaded) continue;
                var sceneName = sceneAt.name;

                //do not unload same scene or bootstarpper scene
                if (sceneName.Equals(activeScene)) continue; 
                scenes.Add(sceneName);
            }

            //Create an AsyncOperationGroup
            var operationGroup = new AsyncOperationGroup(scenes.Count);
            foreach(var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if(operation == null) continue;
                operationGroup.operations.Add(operation);
                EventSystem.Broadcast_OnSceneUnloaded(scene);
            }
            //wait until all operations in the group is done
            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }
            await Resources.UnloadUnusedAssets();
        }

        public async Task UnloadParticularScene(string p_sceneName)
        {
            var bootstrapperScene = SceneManager.GetSceneByName(p_sceneName);
            if (bootstrapperScene.IsValid() && bootstrapperScene.isLoaded)
            {
                var unloadOp = SceneManager.UnloadSceneAsync(p_sceneName);
                if (unloadOp != null)
                {
                    while (!unloadOp.isDone)
                    {
                        await Task.Delay(100);
                    }
                    EventSystem.Broadcast_OnSceneUnloaded(p_sceneName);
                }
            }
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> operations;
        public float progress => operations.Count == 0 ? 0 : operations.Average(o => o.progress);
        public bool IsDone => operations.All( o => o.isDone);

        public AsyncOperationGroup(int initialCapacity)
        {
            operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}

