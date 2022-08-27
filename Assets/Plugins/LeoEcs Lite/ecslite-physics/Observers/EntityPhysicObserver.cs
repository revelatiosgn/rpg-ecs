using UnityEngine;
using Voody.UniLeo.Lite;

namespace LeoLite.EntityPhysics
{
    public class EntityPhysicObserver : MonoBehaviour
    {
        protected ConvertToEntity _convertCurrent;
        
        private void Start()
        {
            if (TryGetComponent(out ConvertToEntity convertCurrent))
            {
                _convertCurrent = convertCurrent;
            }
            else
            {
                Debug.LogError("Отсутствует компонент ConvertToEntity на объекте или Convert Mode не 'Convert and Save', невозможно отправлять события о столкновениях.", gameObject);
                Destroy(this);
            }
        } 
    }
}