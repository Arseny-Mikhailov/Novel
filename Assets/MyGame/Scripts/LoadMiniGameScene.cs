using Naninovel;
using UnityEngine.SceneManagement;
using UniTask = Naninovel.UniTask;

namespace MyGame.Scripts
{
    [CommandAlias("loadMiniGameScene")]
    public class LoadMiniGameScene : Command
    {
        public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
            
            return UniTask.CompletedTask;
        }
    }
}