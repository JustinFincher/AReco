using LeTai.Asset.TranslucentImage;
using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace FinGameWorks.Scripts.ViewModel
{
    [ExcludeComponent("ImageComponent")]
    public class BlurView:UIView
    {
        public TranslucentImage BackgroundBlur;
        
        [MapTo("BackgroundBlur.vibrancy")]
        public _float BackgroundBlurVibrancy;
        
        [MapTo("BackgroundBlur.brightness")]
        public _float BackgroundBlurBrightness;
        
        [MapTo("BackgroundBlur.spriteBlending")]
        public _float BackgroundBlurSpriteBlending;
        
        [MapTo("BackgroundBlur.flatten")]
        public _float BackgroundBlurFlatten;

        public override void Initialize()
        {
            base.Initialize();
            
            BackgroundBlur = gameObject.GetComponent<TranslucentImage>() ? gameObject.GetComponent<TranslucentImage>() : gameObject.AddComponent<TranslucentImage>();
            BackgroundBlur.material = new Material(Shader.Find("UI/TranslucentImage"));
            ImageComponent = BackgroundBlur;
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            BackgroundBlur = gameObject.GetComponent<TranslucentImage>() ? gameObject.GetComponent<TranslucentImage>() : gameObject.AddComponent<TranslucentImage>();
            BackgroundBlur.material = new Material(Shader.Find("UI/TranslucentImage"));
            ImageComponent = BackgroundBlur;
        }
    }
}