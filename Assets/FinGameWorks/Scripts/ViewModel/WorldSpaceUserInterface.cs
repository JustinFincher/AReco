using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;

namespace FinGameWorks.Scripts.ViewModel
{
    public class WorldSpaceUserInterface : UserInterface
    {
        public _Vector3 WorldPosition;

        public SphereCollider sphereCollider;
        
        public override void Initialize()
        {
            base.Initialize();

            transform.position = WorldPosition.Value;
            if (GetComponent<SphereCollider>() == null)
            {
                gameObject.AddComponent<SphereCollider>();
            }

            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = 0.05f;
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            
            transform.position = WorldPosition.Value;
            if (GetComponent<SphereCollider>() == null)
            {
                gameObject.AddComponent<SphereCollider>();
            }

            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = 0.05f;
        }

        public override void LayoutChanged()
        {
            base.LayoutChanged();
            transform.position = WorldPosition.Value;
//            RectTransform.
        }

        public override void RectTransformChanged()
        {
            base.RectTransformChanged();
            transform.position = WorldPosition.Value;
        }

        private void Update()
        {
            transform.position = WorldPosition.Value;
        }
    }
}