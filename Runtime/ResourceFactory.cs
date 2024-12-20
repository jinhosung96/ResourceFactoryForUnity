using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
#if VCONTAINER_SUPPORT
using VContainer.Unity;
#endif

#if R3_SUPPORT && UNITASK_SUPPORT
using R3;
using R3.Triggers;
using Cysharp.Threading.Tasks;

namespace JHS.Library.ResourceFactory.Runtime
{
    public class ResourceFactory<T> where T : Object
    {
        #region Inner Classes

        public class ResourceFactoryBuilder : ResourceFactoryBuilderBase<ResourceFactoryBuilder>
        {
            public ResourceFactory<T> Build(string path)
            {
                var resourceFactory = new ResourceFactory<T>(
                    path: path,
                    isAddressable: isAddressable,
                    isInject: isInject
                );

                return resourceFactory;
            }
        }

        #endregion

        #region Fields

        readonly bool isInject;

        readonly bool isAddressable;

        #endregion

        #region Properties

        public string Path { get; }

        #endregion

        #region Constructor

        protected ResourceFactory(string path, bool isAddressable, bool isInject)
        {
            Path = path;
            this.isAddressable = isAddressable;
            this.isInject = isInject;
        }

        #endregion

        #region Static Methods

        public static ResourceFactoryBuilder Builder => new ResourceFactoryBuilder();

        #endregion

        #region Virtual Methods

        public virtual T Load(Transform parent = null, Action<T> onPreInitialize = null)
        {
            T resource = null;

            if (isAddressable)
            {
#if ADDRESSABLE_SUPPORT
                resource = typeof(Component).IsAssignableFrom(typeof(T)) ? Addressables.LoadAssetAsync<GameObject>(Path).WaitForCompletion().GetComponent<T>() : Addressables.LoadAssetAsync<T>(Path).WaitForCompletion();
#endif
            }
            else resource = Resources.Load<T>(Path);

            if (!resource) return null;
            if (TryInstantiate(parent, resource, onPreInitialize, out var instance)) return instance;

            return resource;
        }

        public virtual async UniTask<T> LoadAsync(Transform parent = null, Action<T> onPreInitialize = null)
        {
            T resource = null;

            if (isAddressable)
            {
#if ADDRESSABLE_SUPPORT
                if (typeof(Component).IsAssignableFrom(typeof(T)))
                    resource = (await Addressables.LoadAssetAsync<GameObject>(Path)).GetComponent<T>();
                else resource = await Addressables.LoadAssetAsync<T>(Path);
#endif
            }
            else resource = await Resources.LoadAsync<T>(Path) as T;

            if (!resource) return null;
            if (TryInstantiate(parent, resource, onPreInitialize, out var instance)) return instance;

            return resource;
        }

        public virtual void Release(T resource)
        {
            if (resource is Component component)
                Object.Destroy(component.gameObject);
            else if (resource is GameObject gameObject)
                Object.Destroy(gameObject);
            else
            {
#if ADDRESSABLE_SUPPORT
                if (isAddressable) Addressables.Release(resource);
#endif
            }
        }

        #endregion

        bool TryInstantiate(Transform parent, T resource, Action<T> onPreInitialize, out T result)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
            {
                GameObject handle = null;
                if (resource is Component resourceComponent) handle = resourceComponent.gameObject;
                else if (resource is GameObject resourceGameObject) handle = resourceGameObject;
                
                if (handle != null && onPreInitialize != null) handle.SetActive(false);


                T instance = null;
                if (isInject)
                {
#if VCONTAINER_SUPPORT
                    instance = VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(resource, parent);
#endif
                }
                else instance = Object.Instantiate(resource, parent);

                instance.name = instance.name.Replace("(Clone)", string.Empty);

                if (instance != null && onPreInitialize != null)
                {
                    if (instance is Component instanceComponent)
                    {
                        onPreInitialize(instance);
                        instanceComponent.gameObject.SetActive(true);
                    }
                    else if (instance is GameObject instanceGameObject)
                    {
                        onPreInitialize(instance);
                        instanceGameObject.SetActive(true);
                    }
                }

#if ADDRESSABLE_SUPPORT
                {
                    if (instance is Component instanceComponent)
                        instanceComponent.OnDestroyAsObservable().Where(isAddressable, (_, b) => b).Subscribe(handle, (_, h) => Addressables.Release(h));
                    else if (instance is GameObject instanceGameObject)
                        instanceGameObject.OnDestroyAsObservable().Where(isAddressable, (_, b) => b).Subscribe(handle, (_, h) => Addressables.Release(h));
                }
#endif

                result = instance;
                return true;
            }

            result = null;
            return false;
        }
    }
}
#endif