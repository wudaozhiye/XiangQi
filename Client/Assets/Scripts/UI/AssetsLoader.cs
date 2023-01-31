using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using YooAsset;
using Object = UnityEngine.Object;

public abstract class AssetsLoader:UIBehaviour
{
    public List<AssetOperationHandle> _loaded_asset = new List<AssetOperationHandle>();
    public Dictionary<string,UnityEngine.Object> _asset_dic = new Dictionary<string, Object>();
    public virtual void CacheLoad(string path)
    {
        //_loaded_asset.Add(path);
    }

    public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
    {
        AssetOperationHandle tempHandle = YooAssets.LoadAssetSync<T>(assetPath);
        UnityEngine.Object temp = tempHandle.AssetObject;
        if (temp == null)
        {
            Debug.LogError($"{assetPath} is null");
            return null;
        }
        if (!_asset_dic.ContainsKey(assetPath))
        {
            _asset_dic.Add(assetPath,temp);
        }
        
        _loaded_asset.Add(tempHandle);
        return temp as T;
    }

    protected override void OnDestroy()
    {
        UnLoadAll();
    }
    public void UnLoadAll()
    {
        ClearAsset();
    }

    private void ClearAsset()
    {
        for (int i = 0; i < _loaded_asset.Count; i++)
        {
            //if (AssetManager.Instance != null)
            {
                _loaded_asset[i].Release();
            }
        }
        _loaded_asset.Clear();

        _asset_dic.Clear();
    }
}
