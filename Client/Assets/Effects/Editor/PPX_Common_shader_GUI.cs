using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PPX_Common_shader_GUI : ShaderGUI
{
    MaterialProperty _Ztest;

    MaterialProperty _Zwrite;

    MaterialProperty _Cull;

    MaterialProperty _add_or_blend;

    MaterialProperty _base_texture;

    MaterialProperty _use_custom2_xyzw_control_base_uv;

    MaterialProperty _base_uv;

    MaterialProperty _base_speed;

    MaterialProperty _use_change_color;

    MaterialProperty _HUE;

    MaterialProperty _Saturation;

    MaterialProperty _Value;

    MaterialProperty _alpha_power;

    MaterialProperty _Fade_distance;

    MaterialProperty _use_doubel_pass;

    MaterialProperty _base_front_color;

    MaterialProperty _base_front_power;

    MaterialProperty _base_back_color;

    MaterialProperty _base_back_power;

    MaterialProperty _use_emissive;

    MaterialProperty _emissive_tex;
    MaterialProperty _connect_base_alone;
    MaterialProperty _emissive_uv;
    MaterialProperty _emissive_speed;
    MaterialProperty _Emissive_color;

    MaterialProperty _Emissive_power;

    MaterialProperty _use_second_tex;

    MaterialProperty _use_dissolve_or_mul;

    MaterialProperty _dissolve_texture;

    MaterialProperty _dissolve_uv;

    MaterialProperty _dissolve_speed;

    MaterialProperty _edge_hardness;

    MaterialProperty _dissolve;

    MaterialProperty _use_custom1_x_dissolve;

    MaterialProperty _use_distort;

    MaterialProperty _Distort_tex;

    MaterialProperty _Distort_mask;

    MaterialProperty _distort_uv;

    MaterialProperty _distort_speed;

    MaterialProperty _distort_power;

    MaterialProperty _use_custom1_z_distort;

    MaterialProperty _use_color_tex;

    MaterialProperty _color_Tex;

    MaterialProperty _color_uv;

    MaterialProperty _color_speed;

    MaterialProperty _reduce_color;

    MaterialProperty _use_displace;

    MaterialProperty _displace_tex;

    MaterialProperty _displace_uv;

    MaterialProperty _displace_speed;

    MaterialProperty _displace_power;

    MaterialProperty _displace_mask_tex;

    MaterialProperty _displace_mask_uv;

    MaterialProperty _displace_mask_speed;

    MaterialProperty _use_custom1_w_displace;

    public override void OnGUI(
        MaterialEditor materialEditor,
        MaterialProperty[] properties
    )
    {
        EditorGUILayout
            .LabelField("皮皮虾牌特效通用shader", EditorStyles.miniButton);
        EditorGUILayout
            .LabelField("基础部分", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _add_or_blend = FindProperty("_add_or_blend", properties);
        materialEditor.ShaderProperty(_add_or_blend, "混合模式0为add,1为blend");
        _base_texture = FindProperty("_base_texture", properties);
        materialEditor
            .TexturePropertySingleLine(new GUIContent("基础贴图"),
            _base_texture);

        _use_custom2_xyzw_control_base_uv =
            FindProperty("_use_custom2_xyzw_control_base_uv", properties);
        materialEditor
            .ShaderProperty(_use_custom2_xyzw_control_base_uv,
            "使用_custom2_xyzw_控制基础UV");
        _base_uv = FindProperty("_base_uv", properties);
        materialEditor.ShaderProperty(_base_uv, "基础uv");
        _base_speed = FindProperty("_base_speed", properties);
        materialEditor.ShaderProperty(_base_speed, "基础流动");
        EditorGUILayout
.LabelField("流速:xy(-1,1)控制方向,z控制速度,w无作用", EditorStyles.miniButton);
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
        _Fade_distance = FindProperty("_Fade_distance", properties);
        materialEditor.ShaderProperty(_Fade_distance, "软粒子");
        _use_doubel_pass = FindProperty("_use_doubel_pass", properties);
        materialEditor.ShaderProperty(_use_doubel_pass, "使用双pass");
        _base_front_color = FindProperty("_base_front_color", properties);
        materialEditor.ShaderProperty(_base_front_color, "正面颜色");
        _base_front_power = FindProperty("_base_front_power", properties);
        materialEditor.ShaderProperty(_base_front_power, "正面颜色强度");

        if (_use_doubel_pass.floatValue == 1)
        {
            _base_back_color = FindProperty("_base_back_color", properties);
            materialEditor.ShaderProperty(_base_back_color, "背面颜色");
            _base_back_power = FindProperty("_base_back_power", properties);
            materialEditor.ShaderProperty(_base_back_power, "背面颜色强度");
        }
        EditorGUILayout.EndVertical();


        #region [自发光]
        EditorGUILayout.LabelField("追加自发光", EditorStyles.miniButton);

        //画box
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _use_emissive = FindProperty("_use_emissive", properties);
        materialEditor.ShaderProperty(_use_emissive, "追加自发光");

        if (_use_emissive.floatValue == 1)
        {
            _emissive_tex = FindProperty("_emissive_tex", properties);
            materialEditor
                .TexturePropertySingleLine(new GUIContent("自发光贴图"),
                _emissive_tex);

            _connect_base_alone = FindProperty("_connect_base_alone", properties);
            materialEditor.ShaderProperty(_connect_base_alone, "独立于基础贴图uv");
            if (_connect_base_alone.floatValue == 1)
            {
                _emissive_uv = FindProperty("_emissive_uv", properties);
                materialEditor.ShaderProperty(_emissive_uv, "uv");
                _emissive_speed = FindProperty("_emissive_speed", properties);
                materialEditor.ShaderProperty(_emissive_speed, "流速");
            }
            _Emissive_color = FindProperty("_Emissive_color", properties);
            materialEditor.ShaderProperty(_Emissive_color, "着色颜色");
            _Emissive_power = FindProperty("_Emissive_power", properties);
            materialEditor.ShaderProperty(_Emissive_power, "亮度");
        }
        EditorGUILayout.EndVertical();


        #endregion



        #region [溶解]
        EditorGUILayout.LabelField("溶解效果", EditorStyles.miniButton);

        _use_second_tex = FindProperty("_use_second_tex", properties);
        materialEditor.ShaderProperty(_use_second_tex, "使用溶解/遮罩效果");
        if (_use_second_tex.floatValue == 1)
        {
            _dissolve_texture = FindProperty("_dissolve_texture", properties);
            materialEditor
                .TexturePropertySingleLine(new GUIContent("溶解/遮罩贴图"),
                _dissolve_texture);
            _dissolve_uv = FindProperty("_dissolve_uv", properties);
            materialEditor.ShaderProperty(_dissolve_uv, "溶解uv");
            _dissolve_speed = FindProperty("_dissolve_speed", properties);
            materialEditor.ShaderProperty(_dissolve_speed, "溶解流速");

            _use_dissolve_or_mul =
                FindProperty("_use_dissolve_or_mul", properties);
            materialEditor
                .ShaderProperty(_use_dissolve_or_mul, "溶解/遮罩效果切换");
            if (_use_dissolve_or_mul.floatValue == 1)
            {
                _edge_hardness = FindProperty("_edge_hardness", properties);
                materialEditor.ShaderProperty(_edge_hardness, "溶解边缘硬度");
                _dissolve = FindProperty("_dissolve", properties);
                materialEditor.ShaderProperty(_dissolve, "溶解");
                _use_custom1_x_dissolve =
                    FindProperty("_use_custom1_x_dissolve", properties);
                materialEditor
                    .ShaderProperty(_use_custom1_x_dissolve,
                    "使用custom1_x_溶解");
            }
        }
        #endregion



        #region [扰动]
        EditorGUILayout.LabelField("扰动", EditorStyles.miniButton);
        _use_distort = FindProperty("_use_distort", properties);
        materialEditor.ShaderProperty(_use_distort, "使用扰动");
        if (_use_distort.floatValue == 1)
        {
            _Distort_tex = FindProperty("_Distort_tex", properties);
            materialEditor
                .TexturePropertySingleLine(new GUIContent("扰动贴图"),
                _Distort_tex);
            _distort_uv = FindProperty("_distort_uv", properties);
            materialEditor.ShaderProperty(_distort_uv, "扰动uv");
            _distort_speed = FindProperty("_distort_speed", properties);
            materialEditor.ShaderProperty(_distort_speed, "扰动流速");
            _Distort_mask = FindProperty("_Distort_mask", properties);
            materialEditor.ShaderProperty(_Distort_mask, "扰动遮罩");
            _distort_power = FindProperty("_distort_power", properties);
            materialEditor.ShaderProperty(_distort_power, "扰动强度");
            _use_custom1_z_distort =
                FindProperty("_use_custom1_z_distort", properties);
            materialEditor
                .ShaderProperty(_use_custom1_z_distort,
                "使用_custom1_z_控制扰动强度");
        }
        #endregion



        #region [着色]
        EditorGUILayout.LabelField("着色", EditorStyles.miniButton);

        _use_color_tex = FindProperty("_use_color_tex", properties);
        materialEditor.ShaderProperty(_use_color_tex, "追加着色");
        if (_use_color_tex.floatValue == 1)
        {
            _color_Tex = FindProperty("_color_Tex", properties);
            materialEditor
                .TexturePropertySingleLine(new GUIContent("着色"), _color_Tex);
            _color_uv = FindProperty("_color_uv", properties);
            materialEditor.ShaderProperty(_color_uv, "uv");
            _color_speed = FindProperty("_color_speed", properties);
            materialEditor.ShaderProperty(_color_speed, "流速");
            _reduce_color = FindProperty("_reduce_color", properties);
            materialEditor.ShaderProperty(_reduce_color, "着色强度削弱");
        }
        #endregion



        #region [置换]
        EditorGUILayout.LabelField("置换", EditorStyles.miniButton);
        _use_displace = FindProperty("_use_displace", properties);
        materialEditor.ShaderProperty(_use_displace, "使用置换");
        if (_use_displace.floatValue == 1)
        {
            _displace_tex = FindProperty("_displace_tex", properties);
            materialEditor
                .TexturePropertySingleLine(new GUIContent("置换贴图"),
                _displace_tex);
            _displace_uv = FindProperty("_displace_uv", properties);
            materialEditor.ShaderProperty(_displace_uv, "置换uv");
            _displace_speed = FindProperty("_displace_speed", properties);
            materialEditor.ShaderProperty(_displace_speed, "流速");
            _displace_power = FindProperty("_displace_power", properties);
            materialEditor.ShaderProperty(_displace_power, "强度");
            _displace_mask_tex = FindProperty("_displace_mask_tex", properties);
            materialEditor
                .TexturePropertySingleLine(new GUIContent("置换遮罩"),
                _displace_mask_tex);
            _displace_mask_uv = FindProperty("_displace_mask_uv", properties);
            materialEditor.ShaderProperty(_displace_mask_uv, "uv");
            _displace_mask_speed =
                FindProperty("_displace_mask_speed", properties);
            materialEditor.ShaderProperty(_displace_mask_speed, "流速");
            _use_custom1_w_displace =
                FindProperty("_use_custom1_w_displace", properties);
            materialEditor
                .ShaderProperty(_use_custom1_w_displace,
                "shi'y_custom1_w_控制置换强度");
        }
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
            .LabelField("技术谈论Q群:755239075", EditorStyles.miniButton);

        EditorGUILayout.EndVertical();


        #endregion

    }
}
