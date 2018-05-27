using UnityEngine;

namespace FinGameWorks.Scrips.Data
{
    public class CardData
    {
        public int index;

        public string category
        {
            get { return _category; }
            set
            {
                _category = value;
                mockup();
            }
        }

        private string _category;
        public Vector3 position;

        public string description;
        

        public static CardData randomNew()
        {
            CardData c = new CardData();
            c.position = new Vector3(Random.value,Random.value,Random.value)*0.5f - 0.25f * Vector3.one;
            c.category = "hahaha";
            return c;
        }

        public void mockup()
        {
            if (category == "coke")
            {
                description = @"可乐 
中文名 可乐 
外文名 Cola 
别    称 汽水 
主要原料 水,二氧化碳，碳水化合物 
是否含防腐剂 否 
主要营养成分 钠，能量
主要食用功效 清凉、降温 
适宜人群 青年，中年 
副作用 胀气、影响钙的吸收 
储藏方法 冷藏室、冰箱，常温 
拼    音 kě lè 
配    料 糖、碳酸水、焦糖、磷酸、咖啡因 
所属分类 碳酸饮料";
            }

            if (category == "apple")
            {
                description = @"苹果
别名：滔婆、柰、柰子、频婆、平波、天然子
营养素含量(每100克)
营养素含量(每100克)
热量(大卡)54.00
碳水化合物(克)12.30
脂肪(克)0.20
蛋白质(克)0.20
纤维素(克)1.20
维生素A(微克)3.00
评价：一种碳水化合物、水分、纤维、钾含量都较高的水果，对于缓解便秘、消除水肿均有一定帮助，适宜减肥时食用";
            }
            if (category == "macbook")
            {
                description = @"苹果电脑 15 寸 
2017 Touchbar 款
商品名称：AppleMacBook 
Pro商品编号：4335127
商品毛重：3.3kg
系统：macOS
厚度：15.1mm—20.0mm
内存容量：16G
待机时长：9小时以上
处理器：Intel ";
            }
        }
    }
}