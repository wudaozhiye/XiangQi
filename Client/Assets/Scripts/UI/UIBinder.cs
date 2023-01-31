using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif


[System.Serializable]
public class SerializeableDictionaryStringAndUIComp : SerializableDictionary<string, Component>
{
}

public class UIBinder : UIBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        this.BindUICompToDict_InRun();
        this.ChangeLanguageTypeText();
    }
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void PrefabStageListenter()
    {
        UnityEditor.SceneManagement.PrefabStage.prefabSaving += OnPrefabSaved;
    }

    private static void OnPrefabSaved(GameObject obj)
    {
        Debug.Log("!!!!!!!!!!!!!on prefab saved!!!!!!!!!!!");
        var binder = obj.GetComponent<UIBinder>();
        if (binder != null)
        {
            binder.BindCompToDict();
        }
    }
#endif
    public const string NAMESPACE = "UIBinderGenerator";

    [SerializeField] private bool isSingleton = false;

    [SerializeField, Header("Auto Generate Subclass Script")]
    private bool generateSubclass = false;

    [SerializeField] private SerializeableDictionaryStringAndUIComp _item_map;

    [SerializeField] private List<Text> _default_text_list;

    public Dictionary<string, Component> getItemMap()
    {
        if (_item_map == null)
        {
            _item_map = new SerializeableDictionaryStringAndUIComp();
        }

        return _item_map;
    }

    public T GetUIComponent<T>(string key, bool need_debug = true) where T : Component
    {
        Component ui;
        _item_map.TryGetValue(key, out ui);
        if (need_debug && ui == null)
        {
            Debug.LogErrorFormat("Can not find UITemplate: {0} Key={1}", name, key);
        }

        return ui as T;
    }

    public List<Text> GetDefaultTextList()
    {
        return _default_text_list;
    }

#if UNITY_EDITOR

    #region Bind Menu

    int auto_id = 0;

    [ContextMenu("UIBinder - Bind All Objects")]
    public void BindCompToDict()
    {
        UIBinder binder = this;

        UnityEditor.Undo.RecordObject(binder, "Bind All Objects");

        binder.getItemMap().Clear();
        string error = "";
        BindCompToDict(binder, binder.transform, ref error, false);
        if (!string.IsNullOrEmpty(error))
        {
            binder.getItemMap().Clear();
            if (UnityEditor.EditorUtility.DisplayDialog("[Error] Duplicated Name:", error, "AutoFix", "ManualFix"))
            {
                error = "";
                BindCompToDict(binder, binder.transform, ref error, true);
            }
        }

        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(binder);
    }

    void BindCompToDict(UIBinder binder, Transform cur_transform, ref string error, bool autoRename)
    {
        for (int i = 0; i < cur_transform.childCount; ++i)
        {
            Transform child = cur_transform.GetChild(i);
            Component ui_base = child.GetComponent<UIBehaviour>();
            Component[] append_comp_list = child.GetComponents<Component>();
            child.name = child.name.Replace(" ", "");
            child.name = child.name.Replace("(", "");
            child.name = child.name.Replace(")", "");
            foreach (var item in append_comp_list)
            {
                Type t = GetAppedComp(item);
                if (t != null)
                {
                    ui_base = item;
                    break;
                }
            }

            bool serializable = (child.name.IndexOf("_") != -1) || IsUISuffixType(child.name); //有下划线，或者是其他tpl
            if (serializable)
            {
                string key = child.name;
                if (ui_base == null)
                    ui_base = child.GetComponent<RectTransform>();
                if (ui_base == null)
                    ui_base = child.GetComponent<Transform>();
                AddToCompDict(binder, ref error, autoRename, child, ui_base, ref key);
            }

            if (child.GetComponent<UIBinder>() || IsUISuffixType(child.name))
            {
                continue;
            }
            else
            {
                //下一个层级
                BindCompToDict(binder, child, ref error, autoRename);
            }
        }
    }

    void AddToCompDict(UIBinder binder, ref string error, bool autoRename, Transform child, Component ui_base,
        ref string key)
    {
        if (binder.getItemMap().ContainsKey(child.name))
        {
            if (autoRename)
            {
                key += "_" + auto_id++;
                child.name = key;
                binder.getItemMap().Add(key, ui_base);
            }
            else
            {
                error += GetGameObjectFullPath(child.gameObject) + "\n";
            }
        }
        else
        {
            binder.getItemMap().Add(key, ui_base);
        }
    }

    string GetGameObjectFullPath(GameObject go)
    {
        string path = "/" + go.name;
        while (go.transform.parent != null)
        {
            go = go.transform.parent.gameObject;
            path = "/" + go.name + path;
        }

        return path;
    }

    #endregion

    #region Bind UIText

    [ContextMenu("UIBinder - Bind UI Texts")]
    public void BindUICompToDict()
    {
        UIBinder binder = this;

        UnityEditor.Undo.RecordObject(binder, "Bind UI Texts");

        binder.GetDefaultTextList().Clear();
        string error = "";
        BindUICompToDict(binder, binder.transform, ref error, false);
        //if (!string.IsNullOrEmpty(error))
        //{
        //    binder.getItemMap().Clear();
        //    if (UnityEditor.EditorUtility.DisplayDialog("[Error] Duplicated Name:", error, "AutoFix", "ManualFix"))
        //    {
        //        error = "";
        //        BindUICompToDict(binder, binder.transform, ref error, true);
        //    }
        //}

        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(binder);
    }

    #endregion

    #region Generate Menu

    [ContextMenu("UIBinder - Generate UI Code")]
    void GenerateMenu()
    {
        GenerateCode();

        if (generateSubclass)
        {
            GenerateSubclass();
        }

        UIBinder binder = this;
        GenerateChildCode(binder.transform);

        UnityEditor.AssetDatabase.Refresh();
    }

    void GenerateChildCode(Transform cur_transform)
    {
        for (int i = 0; i < cur_transform.childCount; ++i)
        {
            Transform child = cur_transform.GetChild(i);
            UIBinder ui_binder = child.GetComponent<UIBinder>();
            if (ui_binder == null)
            {
                GenerateChildCode(child);
                continue;
            }

            //防止改了名字的对象生成不符合规范的代码
            if (!IsGenerateCodeType(child.name)) continue;

            ui_binder.GenerateCode();
        }
    }

    bool IsGenerateCodeType(string name)
    {
        return name.EndsWith("UITpl") ||
               name.EndsWith("UIScene") ||
               name.EndsWith("UIDialog") ||
               name.EndsWith("UIFlow") ||
               name.EndsWith("UIEffect");
    }

    public void GenerateCode()
    {
        string prefab_name = this.name;
        UIBinder v = this;
        string sb = @"// UIBinder Auto Generated

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Nireus;
using Spine.Unity;

namespace " + NAMESPACE + @"
{
";

        sb += "\tpublic class " + GetClassName(prefab_name) + " : " + GetSuperClassName(prefab_name) + " \n\t{\n";
        sb += "\t\tprotected UIBinder _ui_binder;\n";

        List<string> sortList = new List<string>();
        List<Button> button_list = new List<Button>();
        List<Toggle> toggle_list = new List<Toggle>();
        List<InputField> input_list = new List<InputField>();
        List<ScrollRect> scroll_list = new List<ScrollRect>();

        foreach (var kv in v.getItemMap())
        {
            string str = kv.Value.GetType().Name;
            string line = "\t\tprotected " + str;
            line += " c_" + kv.Key.ToLower() + ";\n";
            if (kv.Value is Button)
            {
                button_list.Add((Button) kv.Value);
            }
            else if (kv.Value is Toggle)
            {
                toggle_list.Add((Toggle) kv.Value);
            }
            else if (kv.Value is InputField)
            {
                input_list.Add((InputField) kv.Value);
            }
            else if (kv.Value is ScrollRect)
            {
                scroll_list.Add((ScrollRect) kv.Value);
            }

            sortList.Add(line);
        }

        sortList.Sort();
        foreach (string s in sortList)
        {
            sb += s;
        }

        sortList.Clear();

        sb += "\n\t\tprotected override void OnAwake()\n\t\t{\n\t\t\tbase.OnAwake();\n";

        sb += "\n\t\t\t_ui_binder = this.gameObject.GetComponent<UIBinder>();\n";
        foreach (var kv in v.getItemMap())
        {
            string s = "";
            string str = kv.Value.GetType().Name;
            s += ("\t\t\tc_" + kv.Key.ToLower() + " = _ui_binder.GetUIComponent<" + str + ">(\"" + kv.Key + "\");\n");
            sortList.Add(s);
        }

        sortList.Sort();
        foreach (string s in sortList)
        {
            sb += s;
        }

        sb += "\n";
        //button
        foreach (var button in button_list)
        {
            sb += "\t\t\tc_" + button.name.ToLower() + ".onClick.AddListener(_OnClick" + button.name + ");\n";
        }

        //input_field
        foreach (var input in input_list)
        {
            sb += "\t\t\tc_" + input.name.ToLower() + ".onEndEdit.AddListener(delegate { _OnChange" + input.name +
                  "(); });\n";
        }

        //scroll_rect
        foreach (var input in scroll_list)
        {
            sb += "\t\t\tc_" + input.name.ToLower() +
                  ".onValueChanged.AddListener(delegate (Vector2 vec_scale) { _OnChange" + input.name +
                  "(vec_scale); });\n";
        }

        //toggle
        foreach (var toggle in toggle_list)
        {
            sb += "\t\t\tc_" + toggle.name.ToLower() + ".onValueChanged.AddListener(delegate (bool state) { _OnClick" +
                  toggle.name + "(state); });\n";
        }

        sb += "\t\t}\n\n";

        //button ui event
        foreach (var button in button_list)
        {
            sb += "\t\tprotected virtual void _OnClick" + button.name +
                  "()\n\t\t{\n\t\t\tSoundManager.Instance.PlaySoundWithCommonObj(SoundName.ClickBtn);\n\t\t}\n";
        }

        //input_field ui event
        foreach (var input in input_list)
        {
            sb += "\t\tprotected virtual void _OnChange" + input.name + "()\n\t\t{\n\t\t}\n";
        }

        //scroll_rect ui event
        foreach (var input in scroll_list)
        {
            sb += "\t\tprotected virtual void _OnChange" + input.name + "(Vector2 vec_scale)\n\t\t{\n\t\t}\n";
        }

        //toggle ui event
        foreach (var toggle in toggle_list)
        {
            sb += "\t\tprotected virtual void _OnClick" + toggle.name +
                  "(bool state)\n\t\t{\n\t\t\tSoundManager.Instance.PlaySoundWithCommonObj(SoundName.ClickBtn);\n\t\t}\n";
        }

        sb += "\t}\n}\n";

        if (string.IsNullOrEmpty(sb) == false)
        {
            string str_dir = PathConst.SCRIPTS_UIBINDER_UI_GENERATED;
            DirectoryInfo di = new DirectoryInfo(str_dir);
            if (di.Exists == false)
            {
                di.Create();
            }

            //File.WriteAllText(str_dir + prefab_name + ".txt", sb);
            StreamWriter sw;
            FileInfo fi = new FileInfo(str_dir + prefab_name + "Gen.cs");
            sw = fi.CreateText();
            sw.Write(sb);
            sw.Close();
            sw.Dispose();
            //Debug.Log("--------UI Info --- " + sb.ToString());
            Debug.Log("Generate Successfully: " + str_dir + prefab_name + "Gen.cs");
        }
    }

    private void GenerateSubclass()
    {
        string prefab_name = this.name;

        var sb = @"// UIBinder Auto Generated

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

";
        sb += "public class " + prefab_name + " : " + NAMESPACE + "." + GetClassName(prefab_name) + " \n{\n";

        if (isSingleton)
        {
            sb += $@"
    private static {prefab_name} _instance;
    
    public static {prefab_name} Instance
    {{
        get
        {{
            if (_instance == null)
            {{
                CommonAssetLoaderManager.Instance.LoadUI(typeof({prefab_name}));
            }}

            return _instance;
        }}
        
        private set {{ _instance = value; }}
    }}
";

            sb += @"
    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
    }
";

            sb += @"
    protected override void OnDestroy()
    {
        Instance = null;
        base.OnDestroy();
    }
";
        }

        sb += "\n}";

        if (string.IsNullOrEmpty(sb) == false)
        {
            string str_dir = PathConst.SCRIPTS_UIBINDER_SUB_GENERATED;
            DirectoryInfo di = new DirectoryInfo(str_dir);
            if (di.Exists == false)
            {
                di.Create();
            }

            FileInfo fi = new FileInfo(str_dir + prefab_name + ".cs");
            StreamWriter sw = fi.CreateText();
            sw.Write(sb);
            sw.Close();
            sw.Dispose();
            Debug.Log("Generate Successfully: " + str_dir + prefab_name + ".cs");
        }
    }

    static string GetClassName(string prefabName)
    {
        return prefabName + "Gen";
    }

    static string GetSuperClassName(string prefab_name)
    {
        if (prefab_name.EndsWith("UIScene"))
        {
            return "UIScene";
        }
        else if (prefab_name.EndsWith("UIDialog"))
        {
            return "UIDialog";
        }
        else if (prefab_name.EndsWith("UITpl"))
        {
            return "UITemplate";
        }
        else if (prefab_name.EndsWith("UIFlow"))
        {
            return "UITemplate";
        }
        else if (prefab_name.EndsWith("UIEffect"))
        {
            return "UITemplate";
        }

        Debug.LogError(
            "UIBinder gameObject needs naming with specified suffix: \n UIScene, UIDialog, UITpl, UIFlow, UIEffect");
        return "IllegalSuffix";
    }

    #endregion

    #region Generate NotFix Menu

    [ContextMenu("UIBinder - Generate NotFix UI Code")]
    void GenerateNotFixMenu()
    {
        GenerateNotFixCode();

        if (generateSubclass)
        {
            GenerateSubclass();
        }

        UIBinder binder = this;
        GenerateChildCode(binder.transform);

        UnityEditor.AssetDatabase.Refresh();
    }

    public void GenerateNotFixCode()
    {
        string prefab_name = this.name;
        UIBinder v = this;
        string sb = @"// UIBinder Auto Generated

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Nireus;
using Spine.Unity;
using Nireus.Model;
namespace " + NAMESPACE + @"
{
";

        sb += "\tpublic class " + GetClassName(prefab_name) + " : " + GetSuperClassName(prefab_name) + " \n\t{\n";
        sb += "\t\tprotected UIBinder _ui_binder;\n";

        List<string> sortList = new List<string>();
        List<Button> button_list = new List<Button>();
        List<Toggle> toggle_list = new List<Toggle>();
        List<InputField> input_list = new List<InputField>();
        List<ScrollRect> scroll_list = new List<ScrollRect>();

        foreach (var kv in v.getItemMap())
        {
            string str = kv.Value.GetType().Name;
            string line = "\t\tprotected " + str;
            line += " c_" + kv.Key.ToLower() + ";\n";
            if (kv.Value is Button)
            {
                button_list.Add((Button) kv.Value);
            }
            else if (kv.Value is Toggle)
            {
                toggle_list.Add((Toggle) kv.Value);
            }
            else if (kv.Value is InputField)
            {
                input_list.Add((InputField) kv.Value);
            }
            else if (kv.Value is ScrollRect)
            {
                scroll_list.Add((ScrollRect) kv.Value);
            }

            sortList.Add(line);
        }

        sortList.Sort();
        foreach (string s in sortList)
        {
            sb += s;
        }

        sortList.Clear();

        sb += "\n\t\tprotected override void OnAwake()\n\t\t{\n\t\t\tbase.OnAwake();\n";

        sb += "\n\t\t\t_ui_binder = this.gameObject.GetComponent<UIBinder>();\n";
        foreach (var kv in v.getItemMap())
        {
            string s = "";
            string str = kv.Value.GetType().Name;
            s += ("\t\t\tc_" + kv.Key.ToLower() + " = _ui_binder.GetUIComponent<" + str + ">(\"" + kv.Key + "\");\n");
            sortList.Add(s);
        }

        sortList.Sort();
        foreach (string s in sortList)
        {
            sb += s;
        }

        sb += "\n";
        //button
        foreach (var button in button_list)
        {
            sb += "\t\t\tc_" + button.name.ToLower() + ".onClick.AddListener(_OnClick" + button.name + ");\n";
        }

        //input_field
        foreach (var input in input_list)
        {
            sb += "\t\t\tc_" + input.name.ToLower() + ".onEndEdit.AddListener(delegate { _OnChange" + input.name +
                  "(); });\n";
        }

        //scroll_rect
        foreach (var input in scroll_list)
        {
            sb += "\t\t\tc_" + input.name.ToLower() +
                  ".onValueChanged.AddListener(delegate (Vector2 vec_scale) { _OnChange" + input.name +
                  "(vec_scale); });\n";
        }

        //toggle
        foreach (var toggle in toggle_list)
        {
            sb += "\t\t\tc_" + toggle.name.ToLower() + ".onValueChanged.AddListener(delegate (bool state) { _OnClick" +
                  toggle.name + "(state); });\n";
        }

        sb += "\t\t}\n\n";

        //button ui event
        foreach (var button in button_list)
        {
            sb += "\t\tprotected virtual void _OnClick" + button.name +
                  "()\n\t\t{\n\t\t\tSoundManager.Instance.PlaySoundWithCommonObj(SoundName.ClickBtn);\n\t\t}\n";
        }

        //input_field ui event
        foreach (var input in input_list)
        {
            sb += "\t\tprotected virtual void _OnChange" + input.name + "()\n\t\t{\n\t\t}\n";
        }

        //scroll_rect ui event
        foreach (var input in scroll_list)
        {
            sb += "\t\tprotected virtual void _OnChange" + input.name + "(Vector2 vec_scale)\n\t\t{\n\t\t}\n";
        }

        //toggle ui event
        foreach (var toggle in toggle_list)
        {
            sb += "\t\tprotected virtual void _OnClick" + toggle.name +
                  "(bool state)\n\t\t{\n\t\t\tSoundManager.Instance.PlaySoundWithCommonObj(SoundName.ClickBtn);\n\t\t}\n";
        }

        sb += "\t}\n}\n";

        if (string.IsNullOrEmpty(sb) == false)
        {
            string str_dir = PathConst.SCRIPTS_UIBINDER_GENERATED;
            DirectoryInfo di = new DirectoryInfo(str_dir);
            if (di.Exists == false)
            {
                di.Create();
            }

            //File.WriteAllText(str_dir + prefab_name + ".txt", sb);
            StreamWriter sw;
            FileInfo fi = new FileInfo(str_dir + prefab_name + "Gen.cs");
            sw = fi.CreateText();
            sw.Write(sb);
            sw.Close();
            sw.Dispose();
            //Debug.Log("--------UI Info --- " + sb.ToString());
            Debug.Log("Generate Successfully: " + str_dir + prefab_name + "Gen.cs");
        }
    }

    #endregion

#endif
    bool IsUISuffixType(string name)
    {
        return name.ToLower().IndexOf("uitpl") > 0 ||
               name.ToLower().IndexOf("uiscene") > 0 ||
               name.ToLower().IndexOf("uidialog") > 0;
    }

    Type GetAppedComp(Component c)
    {
        //if (!(c is Image || c is RectTransform || c is CanvasRenderer || c is CanvasGroup))
        if (c != null && (c is InputField || c is Canvas || c is ScrollRect ||
                          c is Toggle || c is Dropdown || c is RawImage || c is Button || c is Scrollbar ||
                          c is ToggleGroup))
        {
            return c.GetType();
        }

        return null;
        //return typeof(GameObject);
    }


    public void BindUICompToDict_InRun()
    {
        UIBinder binder = this;
        string error = "";
        BindUICompToDict(binder, binder.transform, ref error, false);
    }

    public void ChangeLanguageTypeText()
    {
        var default_text_list = this.GetDefaultTextList();
        if (default_text_list != null && default_text_list.Count > 0)
        {
            foreach (var text in default_text_list)
            {
                //text.text = Lang.Get(Utils.FixLineBreakSymbol(text.text));
            }
        }
    }

    void BindUICompToDict(UIBinder binder, Transform cur_transform, ref string error, bool autoRename)
    {
        for (int i = 0; i < cur_transform.childCount; ++i)
        {
            Transform child = cur_transform.GetChild(i);
            Component ui_base = child.GetComponent<UIBehaviour>();
            Component[] append_comp_list = child.GetComponents<Component>();
            child.name = child.name.Replace(" ", "");
            child.name = child.name.Replace("(", "");
            child.name = child.name.Replace(")", "");
            foreach (var item in append_comp_list)
            {
                Type t = GetAppedComp(item);
                if (t != null)
                {
                    ui_base = item;
                    break;
                }
            }

            Text text_comp = child.GetComponent<Text>();
            if (null != text_comp)
            {
                _default_text_list.Add(text_comp);
            }

            if (child.GetComponent<UIBinder>() || IsUISuffixType(child.name))
            {
                continue;
            }
            else
            {
                //下一个层级
                BindUICompToDict(binder, child, ref error, autoRename);
            }
        }
    }
}