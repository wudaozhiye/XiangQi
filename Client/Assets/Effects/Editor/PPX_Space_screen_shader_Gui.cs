using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PPX_Space_screen_shader_Gui : ShaderGUI
{
    MaterialProperty _Ztest;

    MaterialProperty _Zwrite;

    MaterialProperty _Cull;

    MaterialProperty _base_texture;

    MaterialProperty _base_uv;

    MaterialProperty _base_speed;

    MaterialProperty _use_change_color;

    MaterialProperty _HUE;

    MaterialProperty _Saturation;

    MaterialProperty _Value;

    MaterialProperty _alpha_power;

    MaterialProperty _base_front_color;

    MaterialProperty _base_front_power;

    MaterialProperty _mask_texture;

    MaterialProperty _mask_uv;

    MaterialProperty _mask_speed;

    MaterialProperty _dissolve_texture;

    MaterialProperty _dissolve_uv;

    MaterialProperty _dissolve_speed;

    MaterialProperty _edge_hardness;

    MaterialProperty _dissolve;

    MaterialProperty _use_custom1_x_dissolve;

    public override void OnGUI(
        MaterialEditor materialEditor,
        MaterialProperty[] properties
    )
    {
        EditorGUILayout
            .LabelField("皮皮虾牌特效通用shader", EditorStyles.miniButton);
        EditorGUILayout.LabelField("基础部分", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _base_texture = FindProperty("_base_texture", properties);
        materialEditor
            .TexturePropertySingleLine(new GUIContent("基础贴图"),
            _base_texture);

        _base_uv = FindProperty("_base_uv", properties);
        materialEditor.ShaderProperty(_base_uv, "基础uv");
        _base_speed = FindProperty("_base_speed", properties);
        materialEditor.ShaderProperty(_base_speed, "基础流动");
        EditorGUILayout
            .LabelField("流速:xy(-1,1)控制方向,z控制速度,w无作用",
            EditorStyles.miniButton);
        _use_change_color = FindProperty("_use_change_color", properties);
        materialEditor.ShaderProperty(_use_change_color, "使用变色");
        if (_use_change_color.floatValue == 1)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _HUE = FindProperty("_HUE", properties);
            materialEditor.ShaderProperty(_HUE, "色相");
            _Saturation = FindProperty("_Saturation", properties);
            materialEditor.ShaderProperty(_Saturation, "饱和度");
            _Value = FindProperty("_Value", properties);
            materialEditor.ShaderProperty(_Value, "明度");
            EditorGUILayout.EndVertical();
        }

        _alpha_power = FindProperty("_alpha_power", properties);
        materialEditor.ShaderProperty(_alpha_power, "基础alpha");

        _base_front_color = FindProperty("_base_front_color", properties);
        materialEditor.ShaderProperty(_base_front_color, "着色");
        _base_front_power = FindProperty("_base_front_power", properties);
        materialEditor.ShaderProperty(_base_front_power, "颜色强度");

        EditorGUILayout.EndVertical();


#region [遮罩]

        EditorGUILayout.LabelField("遮罩贴图", EditorStyles.miniButton);
        _mask_texture = FindProperty("_mask_texture", properties);
        materialEditor
            .TexturePropertySingleLine(new GUIContent("遮罩贴图"),
            _mask_texture);
        _mask_uv = FindProperty("_mask_uv", properties);
        materialEditor.ShaderProperty(_mask_uv, "uv");
        _mask_speed = FindProperty("_mask_speed", properties);
        materialEditor.ShaderProperty(_mask_speed, "流速");


#endregion



#region [溶解]
        EditorGUILayout.LabelField("溶解效果", EditorStyles.miniButton);

        _dissolve_texture = FindProperty("_dissolve_texture", properties);
        materialEditor
            .TexturePropertySingleLine(new GUIContent("溶解/遮罩贴图"),
            _dissolve_texture);
        _dissolve_uv = FindProperty("_dissolve_uv", properties);
        materialEditor.ShaderProperty(_dissolve_uv, "溶解uv");
        _dissolve_speed = FindProperty("_dissolve_speed", properties);
        materialEditor.ShaderProperty(_dissolve_speed, "溶解流速");

        _edge_hardness = FindProperty("_edge_hardness", properties);
        materialEditor.ShaderProperty(_edge_hardness, "溶解边缘硬度");
        _dissolve = FindProperty("_dissolve", properties);
        materialEditor.ShaderProperty(_dissolve, "溶解");
        _use_custom1_x_dissolve =
            FindProperty("_use_custom1_x_dissolve", properties);
        materialEditor
            .ShaderProperty(_use_custom1_x_dissolve, "使用custom1_x_溶解");


#endregion



#region [额外设置]

        EditorGUILayout.LabelField("额外设置", EditorStyles.miniButton);

        //画box
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _Ztest = FindProperty("_Ztest", properties);
        materialEditor.ShaderProperty(_Ztest, "_Ztest");
        _Zwrite = FindProperty("_Zwrite", properties);
        materialEditor.ShaderProperty(_Zwrite, "_Zwrite");
        _Cull = FindProperty("_Cull", properties);
        materialEditor.ShaderProperty(_Cull, "_Cull");

        materialEditor.RenderQueueField();
        EditorGUILayout.EndVertical();
#endregion



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
            .LabelField("技术谈论Q群:453762906", EditorStyles.miniButton);

        EditorGUILayout.EndVertical();


#endregion

    }
}
