using Management;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] float fillSpeed = 0.01f;
        [SerializeField] private Image loadingBar;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private SceneGroup[] sceneGroups;
        [SerializeField] private SceneData sceneData;
        [SerializeField] bool canLoadSceneGroup, canLoadSingleScene;

        private float targetProgress;
        private bool isLoading;
        public static SceneLoader instance;
        public readonly SceneGroupManager sceneGroupManager = new();

        private void Awake()
        {
            if(instance == null) { instance = this;  DontDestroyOnLoad(gameObject); }
            else Destroy(gameObject);
            EventSystem.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            EventSystem.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            EventSystem.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded..."); 
        }

        private async void Start()
        {
            if(canLoadSceneGroup && !canLoadSingleScene) await LoadSceneGroup(0);

            if (canLoadSingleScene && !canLoadSceneGroup) await LoadScene();

        }

        private void Update()
        {
            if (!isLoading) return;
            float currentFillAmount = loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

            float dynamicFillSpeed = progressDifference * fillSpeed;
            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress , dynamicFillSpeed * Time.deltaTime);
        }

        public async Task LoadSceneGroup(int p_index)
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            if (p_index < 0 || p_index >= sceneGroups.Length) { Debug.LogError("Invalid group index: "+p_index); return; }

            LoadingProgress progress = new();
            progress.OnProgress += target => targetProgress = Mathf.Max(target, targetProgress);

            EnableLoadingScreen();
            await sceneGroupManager.LoadAdditiveScenes(sceneGroups[p_index], progress);
            EnableLoadingScreen(false);
        }

        public async Task LoadScene()
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            LoadingProgress progress = new();
            progress.OnProgress += target => targetProgress = Mathf.Max(target, targetProgress);
            EnableLoadingScreen();
            await sceneGroupManager.LoadSingleScene(sceneData, progress);
            EnableLoadingScreen(false);
        }

        private void EnableLoadingScreen(bool enable = true)
        {
            isLoading = enable;
            loadingScreen.SetActive(enable);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> OnProgress;
        const float ratio = 1f;

        public void Report(float value)
        {
            OnProgress?.Invoke(value / ratio);
        }
    }
}
