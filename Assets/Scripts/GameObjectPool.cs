using System.Collections.Generic;
using UnityEngine;

namespace GameTest.DarioMunoz
{
    /// <summary>
    /// Simple generic GameObject pool. Avoids repeated Instantiate/Destroy calls
    /// by recycling deactivated objects.
    /// </summary>
    public class GameObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Stack<GameObject> _available;

        public GameObjectPool(GameObject prefab, Transform parent, int preWarmCount = 0)
        {
            _prefab = prefab;
            _parent = parent;
            _available = new Stack<GameObject>();

            // Pre-warm: instantiate objects upfront to avoid runtime spikes
            for (int i = 0; i < preWarmCount; i++)
            {
                var obj = Object.Instantiate(_prefab, _parent);
                obj.SetActive(false);
                _available.Push(obj);
            }
        }

        /// <summary>
        /// Gets an object from the pool. Instantiates a new one if the pool is empty.
        /// </summary>
        public GameObject Get()
        {
            GameObject obj;

            if (_available.Count > 0)
            {
                obj = _available.Pop();
                obj.SetActive(true);
            }
            else
            {
                obj = Object.Instantiate(_prefab, _parent);
            }

            return obj;
        }

        /// <summary>
        /// Returns an object to the pool instead of destroying it.
        /// </summary>
        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            _available.Push(obj);
        }

        /// <summary>
        /// Destroys all pooled objects. Use only on scene unload.
        /// </summary>
        public void Clear()
        {
            while (_available.Count > 0)
            {
                var obj = _available.Pop();
                if (obj != null)
                    Object.Destroy(obj);
            }
        }
    }
}
