using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;
using static XLua.LuaEnv;

/// <summary>
/// 用来加载lua文件和proto文件的特殊方法 工具类
/// </summary>
public class LoaderHelper
{
    /// <summary>
    /// 初始化自定义Lua加载器
    /// </summary>
    public static void InitCustomLoaders()
    {
#if UNITY_EDITOR
        if (GlobalConfig.BundleMode == false)
        {
            InitCustomLoaders_Editor();
        }
        else
        {
            InitCustomLoaders_Runtime();
        }       
#else
        InitCustomLoaders_Runtime();
#endif
    }

    /// <summary>
    /// 编辑器模式下的自定义Lua加载器
    /// </summary>
    public static void InitCustomLoaders_Editor()
    {
        DirectoryInfo baseDir = new DirectoryInfo(Application.dataPath + "/GAssets");

        // 遍历所有模块

        DirectoryInfo[] Dirs = baseDir.GetDirectories();

        foreach (DirectoryInfo moduleDir in Dirs)
        {
            string moduleName = moduleDir.Name;

            CustomLoader Loader = (ref string scriptPath) =>
            {
                string assetPath = Application.dataPath + "/GAssets/" + moduleName + "/Src/" + scriptPath.Trim() + ".lua";

                byte[] result = File.ReadAllBytes(assetPath);

                return result;
            };

            Main.Instance.luaEnv.AddLoader(Loader);
        }
    }

    /// <summary>
    /// 真机模式(即AB包模式)下的自定义Lua加载器
    /// </summary>
    public static void InitCustomLoaders_Runtime()
    {
        string[] moduleList = { "Launch" };

        foreach (string moduleName in moduleList)
        {
            CustomLoader Loader = (ref string scriptPath) =>
            {
                string assetPath = "Assets/GAssets/" + moduleName + "/Src/" + scriptPath.Trim() + ".lua.bytes";

                TextAsset textAsset = AssetLoader.Instance.CreateAsset<TextAsset>(moduleName, assetPath, Main.Instance.gameObject);

                // 解密

                string result = AESHelper.Decipher(textAsset.text, AESHelper.keyValue);

                // 返回字节数组的形式

                return Encoding.UTF8.GetBytes(result);
            };

            Main.Instance.luaEnv.AddLoader(Loader);
        }
    }

    /// <summary>
    /// 加载PB文件
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="protoPath"></param>
    /// <returns></returns>
    public static string LoadProtoFile(string moduleName, string protoPath)
    {
#if UNITY_EDITOR
        if (GlobalConfig.BundleMode == false)
        {
            return LoadProtoFile_Editor(moduleName, protoPath);
        }
        else
        {
            return LoadProtoFile_Runtime(moduleName, protoPath);
        }
#else
        return LoadProtoFile_Runtime(moduleName, protoPath);
#endif
    }

    /// <summary>
    /// 编辑器模式下加载PB文件
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="protoPath"></param>
    /// <returns></returns>
    public static string LoadProtoFile_Editor(string moduleName, string protoPath)
    {
        string assetPath = Application.dataPath + protoPath.Substring(6);

        string result = File.ReadAllText(assetPath);

        return result;
    }

    /// <summary>
    /// 真机模式(即AB包模式)下加载PB文件
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="protoPath"></param>
    /// <returns></returns>
    public static string LoadProtoFile_Runtime(string moduleName, string protoPath)
    {
        string assetPath = protoPath + ".bytes";

        TextAsset textAsset = AssetLoader.Instance.CreateAsset<TextAsset>(moduleName, assetPath, Main.Instance.gameObject);

        // 解密并直接返回

        return AESHelper.Decipher(textAsset.text, AESHelper.keyValue);
    }
}