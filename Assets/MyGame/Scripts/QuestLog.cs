using Naninovel;
using TMPro;
using UnityEngine;

namespace MyGame.Scripts
{
    public class QuestLogController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI questText;
        [SerializeField] private GameObject panel;

        private ICustomVariableManager variableManager;

        private void Start()
        {
            variableManager = Engine.GetService<ICustomVariableManager>();
            variableManager.OnVariableUpdated += OnVariableChanged;
        
            var currentValue = variableManager.GetVariableValue("questStage");
            UpdateQuest(currentValue);
            panel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (variableManager != null)
                variableManager.OnVariableUpdated -= OnVariableChanged;
        }

        private void OnVariableChanged(CustomVariableUpdatedArgs args)
        {
            if (args.Name == "questStage")
                UpdateQuest(args.Value);
        }

        private void UpdateQuest(string stage)
        {
            if (!int.TryParse(stage, out var questStage)) return;

            switch (questStage)
            {
                case 1:
                    panel.SetActive(true);
                    questText.text = "Задание:\nПоговорить с Нани.";
                    break;
                case 2:
                    questText.text = "Задание:\nОтправиться в дом.";
                    break;
                case 3:
                    questText.text = "Задание:\nНайти одинаковые карточки.";
                    break;
                case 4:
                    questText.text = "Задание:\nВернуться к Юне.";
                    break;
                case 5:
                    questText.text = "Задание:\nРешить на кого сделать расклад.";
                    break;
                case 6:
                    panel.SetActive(false); 
                    break;
                default:
                    questText.text = "";
                    break;
            }
        }
    }
}