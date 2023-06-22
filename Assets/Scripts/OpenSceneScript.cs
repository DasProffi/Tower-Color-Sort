using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSceneScript : MonoBehaviour
{
    public string sceneName;
    public bool isGoingToAutosolveMode = false;
    
    public void OpenScene()
    {
        GameState.Instance.IsAutosolving = isGoingToAutosolveMode;
        SceneManager.LoadScene(sceneName);
    }
}
