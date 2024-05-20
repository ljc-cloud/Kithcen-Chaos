using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            playButton.onClick.AddListener(() =>
            {
                Loader.Load(Loader.Scene.GameScene);
            });
            quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });

            Time.timeScale = 1;
        }
    }
}