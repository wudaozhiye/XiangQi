using System.Collections;
using System.Collections.Generic;
using UniFramework.Event;
using UniFramework.Module;
using UnityEngine;
using YooAsset;

public class Boot : MonoBehaviour
{
    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    // Start is called before the first frame update
    void Awake()
    {
        // 初始化BetterStreaming
        BetterStreamingAssets.Initialize();

        // 初始化事件系统
        UniEvent.Initalize();

        // 初始化管理系统
        UniModule.Initialize();
        
        // 初始化资源系统
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);
        
        // 创建补丁管理器
        UniModule.CreateModule<PatchManager>();
        // 开始补丁更新流程
        PatchManager.Instance.Run(PlayMode);
    }
    
}
