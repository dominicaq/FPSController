using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetLibrary
{
    private static bool s_Initialized = false;
    
    public static bool Ready
    {
        get { return s_Initialized; }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        Addressables.InitializeAsync().Completed += InitDone;
    }

    private static void InitDone(AsyncOperationHandle<IResourceLocator> obj)
    {
        s_Initialized = true;
        Debug.Log("Assets loaded!");
    }

    public static TObject LoadAsset<TObject>(object key)
    {
        if(!s_Initialized)
            throw new Exception("Assets haven't been initialized yet!");

        var op = Addressables.LoadAssetAsync<TObject>(key);
        IsValidObject(op, key);

        return op.Result;
    }

    public static GameObject Instantiate(object key, Transform parent = null, bool instantiateInWorldSpace = false)
    {
        if(!s_Initialized)
            throw new Exception("Assets haven't been initialized yet!");
        
        var op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace); 
        IsValidObject(op, key);
        
        return op.Result;
    }

    private static void IsValidObject(AsyncOperationHandle op, object key)
    {
        if (op.Result == null)
        {
            if(!op.IsDone)
                throw new Exception("Sync Instantiate failed to finish! " + key);
            
            var message = "Sync Instantiate has null result " + key;
            if (op.OperationException != null)
                message += " Exception: " + op.OperationException;

            throw new Exception(message);
        }        
    }
}