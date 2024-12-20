#if R3_SUPPORT && UNITASK_SUPPORT
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JHS.Library.ResourceFactory.Runtime
{
    public class ResourcePoolFactory<T> : ResourceFactory<T>, IDisposable where T : Object
    {
        #region Inner Classes
        
        public class ResourcePoolFactoryBuilder : ResourceFactoryBuilderBase<ResourcePoolFactoryBuilder>
        {
            public ResourcePoolFactory<T> Build(string path, int poolSize)
            {
                var resourceFactory = new ResourcePoolFactory<T>(
                    path: path,
                    poolSize: poolSize,
                    isAddressable: isAddressable,
                    isInject: isInject
                );

                return resourceFactory;
            }
        }

        #endregion
        
        #region Constructor

        ResourcePoolFactory(string path, bool isAddressable, bool isInject, int poolSize) : base(path, isAddressable, isInject)
        {
            AddPool(poolSize).Forget();
        }

        #endregion

        #region Fields

        readonly List<T> pool = new();
        Transform container;

        #endregion

        #region Static Methods

        public new static ResourcePoolFactoryBuilder Builder => new();

        #endregion

        #region Override Methods

        public override T Load(Transform parent = null, Action<T> onPreInitialize = null)
        {
            if (TryDequeue(out var instance))
            {
                if (instance is Component component)
                {
                    component.transform.SetParent(parent);
                    component.gameObject.SetActive(true);
                }
                else if (instance is GameObject gameObject)
                {
                    gameObject.transform.SetParent(parent);
                    gameObject.SetActive(true);
                }
                
                return instance;
            }
            else
            {
                instance = base.Load(parent, onPreInitialize);

                if (instance == null) return instance;

                if (instance is Component component) component.gameObject.SetActive(true);
                else if (instance is GameObject gameObject) gameObject.SetActive(true);
                
                return instance;
            }
        }

        public override async UniTask<T> LoadAsync(Transform parent = null, Action<T> onPreInitialize = null)
        {
            if (TryDequeue(out var instance))
            {
                if (instance is Component component)
                {
                    component.transform.SetParent(parent);
                    component.gameObject.SetActive(true);
                }
                else if (instance is GameObject gameObject)
                {
                    gameObject.transform.SetParent(parent);
                    gameObject.SetActive(true);
                }
                
                return instance;
            }
            else
            {
                instance = await base.LoadAsync(parent, onPreInitialize);

                if (instance == null) return instance;

                if (instance is Component component) component.gameObject.SetActive(true);
                else if (instance is GameObject gameObject) gameObject.SetActive(true);
                
                return instance;
            }
        }
    
        public override void Release(T resource) => Enqueue(resource);

        #endregion

        #region Pooling

        async UniTaskVoid AddPool(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                Enqueue(await base.LoadAsync());
            }
        }

        void Enqueue(T instance)
        {
            if(!instance) return;

            if ((typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T))) && !container)
            {
                container = GameObject.Find("ResourceFactoryPool")?.transform;
                if(!container) container = new GameObject("ResourceFactoryPool").transform;
            }
            
            pool.Add(instance);

            if (instance is Component component)
            {
                component.gameObject.SetActive(false);
                component.transform.SetParent(container);
                component.OnDestroyAsObservable().Take(1).DefaultIfEmpty().Subscribe(_ => pool.Remove(instance));
            }
            else if (instance is GameObject gameObject)
            {
                gameObject.SetActive(false);
                gameObject.transform.SetParent(container);
                gameObject.OnDestroyAsObservable().Take(1).DefaultIfEmpty().Subscribe(_ => pool.Remove(instance));
            }
        }

        T Dequeue()
        {
            var instance = pool.LastOrDefault();
            pool.RemoveAt(pool.Count - 1);
            return instance;
        }
        
        bool TryDequeue(out T instance)
        {
            var any = pool.Any();
            if (any) instance = Dequeue();
            else instance = null;
            return any;
        }

        #endregion

        public void Dispose()
        {
            if(container) Object.Destroy(container.gameObject);
        }
    }
}
#endif