using Naninovel;
using UnityEngine;

namespace MyGame.Scripts
{
    public class OpenMapButton : MonoBehaviour
    {
        private readonly string mapScriptName = "Map";
        [SerializeField] private GameObject miniGame;
    
        public void OnClick()
        {
            var scriptPlayer = Engine.GetService<IScriptPlayer>();
            if (scriptPlayer == null) 
            {
                    Debug.LogError("IScriptPlayer is null!"); 
                    return;
            }

            if (mapScriptName != null)
            {
                scriptPlayer.PreloadAndPlayAsync(mapScriptName);
            }
            
            if (miniGame != null)
            {
                miniGame.SetActive(false);
            }
        }


    }
}