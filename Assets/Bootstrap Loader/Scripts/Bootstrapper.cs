using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async Awaitable Init()
    {
        Debug.Log("Bootstrapping....");
        await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
    }
}
