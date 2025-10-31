using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameDefinition;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransit
{
    static CancellationTokenSource _transitTokenSource;
    private static string _currentScneneType = "";
    private static string _requestSceneType = "";
    public static void RequestGotoScene(SceneType sceneType)
    {
        if (_transitTokenSource != null)
        {
            throw new Exception($"transition not complete {_currentScneneType} to {_requestSceneType}");
        }

        RequestGotoSceneAsyncPack(sceneType).Forget();
    }
    private static async UniTask<Unit> RequestGotoSceneAsyncPack(SceneType sceneType)
    {
        var currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == sceneType.ToString())
        {
            throw new ArgumentException($" try go to same scene{sceneType.ToString()}.");
        }

        _transitTokenSource = new CancellationTokenSource();
        var token = _transitTokenSource.Token;

        try
        {
            await RequestGotoSceneAsync(SceneType.Transition_Internal, token);
            await UniTask.DelayFrame(60);
            await RequestGotoSceneAsync(sceneType, token);
        }
        finally
        {
            _transitTokenSource.Dispose();
            _transitTokenSource = null;
        }

        return Unit.Default;
    }
    private static async UniTask<bool> RequestGotoSceneAsync(SceneType sceneType, CancellationToken token)
    {
        var currentScene = SceneManager.GetActiveScene();

        _currentScneneType = currentScene.name;
        _requestSceneType = sceneType.ToString();
        Debug.Log($"request transist scene {currentScene.name} to {sceneType}");

        try
        {
            await SceneManager.LoadSceneAsync(sceneType.ToString())
            .ToUniTask(cancellationToken: token);

            var newScene = SceneManager.GetActiveScene();
            _currentScneneType = newScene.name;
            _requestSceneType = "";
        }
        catch (Exception ex)
        {
            Debug.LogError($"transist failed scene {currentScene.name} to {sceneType} {ex}");
            throw;// todo do sth?
        }

        return true;
    }
}
