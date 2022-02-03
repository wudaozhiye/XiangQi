using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Flowmap_GUI : ShaderGUI
{
    MaterialProperty _Zwrite;

    MaterialProperty _Cull;

    MaterialProperty _add_or_blend;

    MaterialProperty _base_texture;

    MaterialProperty _HUE;

    MaterialProperty _Saturation;

    MaterialProperty _Value;
    MaterialProperty _Base_color;
    MaterialProperty _Glow;

    MaterialProperty _alpha_power;

    MaterialProperty _emissive_map;

    MaterialProperty _Emissive_color;
    MaterialProperty _Emissive_power;
    MaterialProperty _use_dissolve;
    MaterialProperty _dissolve_texture;
    MaterialProperty _edge_hardness;
    MaterialProperty _dissolve;
    MaterialProperty _use_curve_dissolve;
    MaterialProperty _flowmap;
    MaterialProperty _flow;
    MaterialProperty _use_custom1_z_flow;
    MaterialProperty _Distort_tex;
    MaterialProperty _Distort_uv;
    MaterialProperty _Distort_mask;
    MaterialProperty _Distort_mask_uv;
    MaterialProperty _X_speed;
    MaterialProperty _Y_speed;
    MaterialProperty _Time_scale;
    MaterialProperty _Distort_power;



    public override void OnGUI(
        MaterialEditor materialEditor,
        MaterialProperty[] properties
    )
    {
        EditorGUILayout.LabelField("皮皮虾牌Flowmapshader", EditorStyles.miniButton);


        EditorGUILayout.LabelField("混合模式", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);



        _add_or_blend = FindProperty("_add_or_blend", properties);
        materialEditor.ShaderProperty(_add_or_blend, "混合模式0为add,1为blend");

        EditorGUILayout.EndVertical();

        #region [基础贴图]




        EditorGUILayout.LabelField("基础贴图", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);


        _base_texture = FindProperty("_base_texture", properties);
        materialEditor
            .TexturePropertySingleLine(new GUIContent("基础贴图"),
            _base_texture);



        _HUE = FindProperty("_HUE", properties);
        materialEditor.ShaderProperty(_HUE, "色相");

        _Saturation = FindProperty("_Saturation", properties);
        materialEditor.ShaderProperty(_Saturation, "饱和度");

        _Value = FindProperty("_Value", properties);
        materialEditor.ShaderProperty(_Value, "亮度");


        _Base_color = FindProperty("_Base_color", properties);
        materialEditor.ShaderProperty(_Base_color, "基础颜色");


        _Glow = FindProperty("_Glow", properties);
        materialEditor.ShaderProperty(_Glow, "基础亮度");

        _alpha_power = FindProperty("_alpha_power", properties);
        materialEditor.ShaderProperty(_alpha_power, "基础透明度");
        EditorGUILayout.EndVertical();
        #endregion

        #region [自发光]

        EditorGUILayout.LabelField("追加自发光模块", EditorStyles.miniButton);


        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _emissive_map = FindProperty("_emissive_map", properties);
        materialEditor
    .TexturePropertySingleLine(new GUIContent("自发光遮罩"),
    _emissive_map);

        _Emissive_color = FindProperty("_Emissive_color", properties);
        materialEditor.ShaderProperty(_Emissive_color, "自发光颜色");


        _Emissive_power = FindProperty("_Emissive_power", properties);
        materialEditor.ShaderProperty(_Emissive_power, "自发光亮度");

        EditorGUILayout.LabelField("默认使用custom1的y控制亮度", EditorStyles.miniButton);




        EditorGUILayout.EndVertical();


        #endregion

        #region [溶解]

        EditorGUILayout.LabelField("溶解模块", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _use_dissolve = FindProperty("_use_dissolve", properties);
        materialEditor.ShaderProperty(_use_dissolve, "使用溶解效果");

        if (_use_dissolve.floatValue == 1)
        {
            _dissolve_texture = FindProperty("_dissolve_texture", properties);
            materialEditor
        .TexturePropertySingleLine(new GUIContent("溶解贴图"),
        _dissolve_texture);


            _edge_hardness = FindProperty("_edge_hardness", properties);
            materialEditor.ShaderProperty(_edge_hardness, "溶解的边缘硬度");

            _dissolve = FindProperty("_dissolve", properties);
            materialEditor.ShaderProperty(_dissolve, "溶解的数值");

            _use_curve_dissolve =
                FindProperty("_use_curve_dissolve", properties);
            materialEditor
                .ShaderProperty(_use_curve_dissolve,
                "使用custom1的x调整溶解程度");

        }

        EditorGUILayout.EndVertical();


        #endregion

        #region [flowmap]

        EditorGUILayout.LabelField("flowmap模块", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _flowmap = FindProperty("_flowmap", properties);
        materialEditor
    .TexturePropertySingleLine(new GUIContent("flowmap贴图"),
    _flowmap);


        _flow = FindProperty("_flow", properties);
        materialEditor.ShaderProperty(_flow, "flowmap强度");

        _use_custom1_z_flow = FindProperty("_use_custom1_z_flow", properties);
        materialEditor.ShaderProperty(_use_custom1_z_flow, "使用custom1_z控制强度");
        EditorGUILayout.EndVertical();


        #endregion


        #region [扰动模块]



        EditorGUILayout.LabelField("扰动模块", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _Distort_tex = FindProperty("_Distort_tex", properties);
        materialEditor
    .TexturePropertySingleLine(new GUIContent("扰动贴图"),
    _Distort_tex);

        _Distort_uv = FindProperty("_Distort_uv", properties);
        materialEditor.ShaderProperty(_Distort_uv, "扰动贴图uv");



        _Distort_mask = FindProperty("_Distort_mask", properties);
        materialEditor
    .TexturePropertySingleLine(new GUIContent("扰动遮罩贴图"),
    _Distort_mask);

        _Distort_mask_uv = FindProperty("_Distort_mask_uv", properties);
        materialEditor.ShaderProperty(_Distort_mask_uv, "遮罩uv");



        _X_speed = FindProperty("_X_speed", properties);
        materialEditor.ShaderProperty(_X_speed, "扰动X方向");

        _Y_speed = FindProperty("_Y_speed", properties);
        materialEditor.ShaderProperty(_Y_speed, "扰动Y方向");

        _Time_scale = FindProperty("_Time_scale", properties);
        materialEditor.ShaderProperty(_Time_scale, "扰动速度");
        _Distort_power = FindProperty("_Distort_power", properties);
        materialEditor.ShaderProperty(_Distort_power, "扰动强度");

        EditorGUILayout.EndVertical();


        #endregion



        #region [额外追加]



        EditorGUILayout.LabelField("额外设置", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _Zwrite = FindProperty("_Zwrite", properties);
        materialEditor.ShaderProperty(_Zwrite, "_Zwrite");

        _Cull = FindProperty("_Cull", properties);
        materialEditor.ShaderProperty(_Cull, "_Cull");


        EditorGUILayout.EndVertical();



        #endregion

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
