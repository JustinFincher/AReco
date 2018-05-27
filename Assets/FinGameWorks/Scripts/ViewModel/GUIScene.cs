using FinGameWorks.Scrips.Data;
using MarkLight;

namespace FinGameWorks.Scripts.ViewModel
{
    public class GUIScene : MarkLight.View
    {
        public ObservableList<CardData> CardDatas = new ObservableList<CardData>();

        private void Start()
        {
#if UNITY_EDITOR
//            CardDatas.Add(CardData.randomNew());
//            CardDatas.Add(CardData.randomNew());
//            CardDatas.Add(CardData.randomNew());
#endif
        }

        public void AddCard(CardData data)
        {
            CardDatas.Add(data);
            for (int i = 0; i < CardDatas.Count; i++)
            {
                CardDatas[i].index = i;
            }
        }
    }
}