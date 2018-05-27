using FinGameWorks.Scripts.ViewModel;
using MarkLight;
using UnityEngine;

namespace FinGameWorks.Scripts.Manager
{
    public class UIManager : Singleton<UIManager>
    {
        public ViewPresenter ViewPresenter;
        public MainView MainView;
        public GUIScene GUIScene;

        private void Start()
        {
            ViewPresenter = FindObjectOfType<ViewPresenter>();
            MainView = FindObjectOfType<MainView>();
            GUIScene = FindObjectOfType<GUIScene>();
        }
    }
}