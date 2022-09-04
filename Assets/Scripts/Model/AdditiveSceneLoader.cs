using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame.Model
{
    public class AdditiveSceneLoader : MonoBehaviour
    {
        private NetworkRunner _networkRunner;

        public void Initialize(NetworkRunner networkRunner)
        {
            _networkRunner = networkRunner;
        }

        public void LoadScene(string sceneName, Action OnComplete = null)
        {
            LoadSceneTask(sceneName, OnComplete).Forget();
        }

        private async UniTask LoadSceneTask(string sceneName, Action OnComplete)
        {
            // _networkRunner.InvokeSceneLoadStart();

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene scene = SceneManager.GetSceneByName(sceneName);

            var networkObjects = new List<NetworkObject>();
            var gameObjects = scene.GetRootGameObjects();
            var result = new List<NetworkObject>();

            // get all root gameobjects and move them to this runners scene
            foreach (var go in gameObjects)
            {
                networkObjects.Clear();
                go.GetComponentsInChildren(true, networkObjects);

                foreach (var sceneObject in networkObjects)
                {
                    if (sceneObject.Flags.IsSceneObject())
                    {
                        if (sceneObject.gameObject.activeInHierarchy || sceneObject.Flags.IsActivatedByUser())
                        {
                            Assert.Check(sceneObject.NetworkGuid.IsValid);
                            result.Add(sceneObject);
                        }
                    }
                }

                // if (addVisibilityNodes) {
                //     // register all render related components on this gameobject with the runner, for use with IsVisible
                //     RunnerVisibilityNode.AddVisibilityNodes(go, Runner);
                // }
            }

            _networkRunner.RegisterSceneObjects(result);

            if (OnComplete != null)
                OnComplete.Invoke();

            // _networkRunner.InvokeSceneLoadDone();
        }

        public void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
