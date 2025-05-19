using Naninovel;
using UnityEngine;

namespace MyGame.Scripts
{
    public class MiniGameInitializer : MonoBehaviour
    {
        public void Start()
        {
            var input = Engine.GetService<IInputManager>();
            var player = Engine.GetService<IScriptPlayer>();

            var uiManager = Engine.GetService<IUIManager>();

            foreach (var ui in uiManager.GetManagedUIs())
            {
                ui.Hide();
            }
            input.ProcessInput = false;
            player.Stop(); 
        }
    }
}