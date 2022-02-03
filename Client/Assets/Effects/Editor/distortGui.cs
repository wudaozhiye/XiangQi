using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class distortGui : ShaderGUI
{
    MaterialProperty _mask;

    MaterialProperty _BASE_ALPHA;

    MaterialProperty _shift;

    MaterialProperty _normal_map;

    MaterialProperty _dissolve_texture;

    MaterialProperty _DISSOLVE_UV;

    MaterialProperty _edge_hardness;

    MaterialProperty _dissolve;

    MaterialProperty _use_curve_dissolve;

    MaterialProperty _use_curve_POWER;

    public override void OnGUI(
        MaterialEditor materialEditor,
        MaterialProperty[] properties
    )
    {
        EditorGUILayout.LabelField("折射测试版本", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _mask = FindProperty("_mask", properties);
        materialEditor.TextureProperty(_mask, "基础遮罩");

        // materialEditor.TextureProperty(new GUIContent("基础遮罩"), _mask);
        _BASE_ALPHA = FindProperty("_BASE_ALPHA", properties);
        materialEditor.ShaderProperty(_BASE_ALPHA, "基础透明度");

        _shift = FindProperty("_shift", properties);
        materialEditor.ShaderProperty(_shift, "折射强度");

        _normal_map = FindProperty("_normal_map", properties);
        materialEditor.ShaderProperty(_normal_map, "法线贴图");

        _dissolve_texture = FindProperty("_dissolve_texture", properties);

        // materialEditor.ShaderProperty(_dissolve_texture,"溶解贴图");
        materialEditor
            .TexturePropertySingleLine(new GUIContent("溶解贴图"),
            _dissolve_texture);

        _DISSOLVE_UV = FindProperty("_DISSOLVE_UV", properties);
        materialEditor
            .ShaderProperty(_DISSOLVE_UV,
            "溶解贴图uv");

        _edge_hardness = FindProperty("_edge_hardness", properties);
        materialEditor.ShaderProperty(_edge_hardness, "溶解软硬");

        _dissolve = FindProperty("_dissolve", properties);
        materialEditor.ShaderProperty(_dissolve, "溶解");

        _use_curve_dissolve = FindProperty("_use_curve_dissolve", properties);
        materialEditor.ShaderProperty(_use_curve_dissolve, "custom1_X控制溶解");
        _use_curve_POWER = FindProperty("_use_curve_POWER", properties);
        materialEditor.ShaderProperty(_use_curve_POWER, "custom1_y控制折射");

        EditorGUILayout
            .LabelField("默认使用custom1_zw控制噪波位移",
            EditorStyles.miniButton);

        EditorGUILayout.EndVertical();






        materialEditor.RenderQueueField();

        #region [注意事项]
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout
            .LabelField("警告！！！该shader完全未经过性能优化，请勿引入到项目中",
            EditorStyles.miniButton);
        EditorGUILayout
            .LabelField("主要目的是方便动画师或者特效师制作个人demo，请勿用于任何商业用途",
            EditorStyles.miniButton);
        EditorGUILayout
            .LabelField("个人知乎账号ID:shuang-miao-80 后续可能会有更新",
            EditorStyles.miniButton);
        EditorGUILayout
            .LabelField("技术谈论Q群:755239075", EditorStyles.miniButton);

        EditorGUILayout.EndVertical();


#endregion

    }
}
