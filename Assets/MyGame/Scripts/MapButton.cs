using Naninovel;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Scripts
{
    public class MapButton : MonoBehaviour
    {
        private const string ScriptName = "Map";

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick ()
        {
            var player = Engine.GetService<IScriptPlayer>();
            player.PreloadAndPlayAsync(ScriptName);
        }
    }
}