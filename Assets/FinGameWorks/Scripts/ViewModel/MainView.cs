using System;
using System.Collections.Generic;
using FinGameWorks.Scripts.Manager;
using MarkLight;
using MarkLight.Views;
using MarkLight.Views.UI;
using UnityEngine;

namespace FinGameWorks.Scripts.ViewModel
{
    public class MainView : UIView
    {
        public Label MLMainResultLabel;
        public Label RaycastResultLabel;
        public Label MLResultLabel;

        public void ClearAll()
        {
            UIManager.Instance.GUIScene.CardDatas.Clear();
        }
    }
}