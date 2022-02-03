using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PPX_Frensel_shader_GUI : ShaderGUI
{
    MaterialProperty _Ztest;

    MaterialProperty _Zwrite;

    MaterialProperty _Cull;

    MaterialProperty _add_or_blend;

    MaterialProperty _base_tex;

    MaterialProperty _use_custom2_xyzw_control_base_uv;

    MaterialProperty _base_uv;

    MaterialProperty _base_speed;

    MaterialProperty _reduce_color;

    MaterialProperty _Base_color;

    MaterialProperty _Base_color_power;

    MaterialProperty _Base_alpha;

    MaterialProperty _use_normal;

    MaterialProperty _NormalMap_tex;

    MaterialProperty _normal_uv;

    MaterialProperty _normal_speed;

    MaterialProperty _use_frensel;

    MaterialProperty _frensel_range;

    MaterialProperty _frensel_hard;

    MaterialProperty _Frensel_flip;

    MaterialProperty _use_depth;

    MaterialProperty _Fade_distance;

    MaterialProperty _edge_hard;

    MaterialProperty _edge_range;

    MaterialProperty _edge_kill_or_add;

    MaterialProperty _use_dissolve;

    MaterialProperty _dissolve_tex;

    MaterialProperty _disslove_uv;

    MaterialProperty _disslove_speed;

    MaterialProperty _edge_hardness;

    MaterialProperty _dissolve;

    MaterialProperty _use_custom1_x_dissolve;

    MaterialProperty _use_displace;

    MaterialProperty _displace_tex;

    MaterialProperty _displace_uv;

    MaterialProperty _displace_speed;

    MaterialProperty _displace_power;

    MaterialProperty _displace_mask_tex;

    MaterialProperty _displace_mask_uv;

    MaterialProperty _displace_mask_speed;

    MaterialProperty _use_custom1_y_displace;

    public override void OnGUI(
        MaterialEditor materialEditor,
        MaterialProperty[] properties
    )
    {
        EditorGUILayout
            .LabelField("皮皮虾牌——通用边缘特效shader",
            EditorStyles.miniButton);
        EditorGUILayout.LabelField("基础部分", EditorStyles.miniButton);
        EditorGUILayout.LabelField("使用粒子系统的颜色模块可以控制颜色和透明度", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _add_or_blend = FindProperty("_add_or_blend", properties);
        materialEditor.ShaderProperty(_add_or_blend, "混合模式0为add,1为blend");
        _base_tex = FindProperty("_base_tex", properties);
        materialEditor
            .TexturePropertySingleLine(new GUIContent("基础贴图"), _base_tex);

        _base_uv = FindProperty("_base_uv", properties);
        materialEditor.ShaderProperty(_base_uv, "基础uv");
        _base_speed = FindProperty("_base_speed", properties);
        materialEditor.ShaderProperty(_base_speed, "基础流动");
        EditorGUILayout
            .LabelField("流速:xy(-1,1)控制方向,z控制速度,w无作用",
            EditorStyles.miniButton);

        _reduce_color = FindProperty("_reduce_color", properties);
        materialEditor.ShaderProperty(_reduce_color, "基础色减淡");

        _Base_color = FindProperty("_Base_color", properties);
        materialEditor.ShaderProperty(_Base_color, "基础着色");
        _Base_color_power = FindProperty("_Base_color_power", properties);
        materialEditor.ShaderProperty(_Base_color_power, "基础着色强度");
        _Base_alpha = FindProperty("_Base_alpha", properties);
        materialEditor.ShaderProperty(_Base_alpha, "基础alpha");
        EditorGUILayout.EndVertical();


#region [法线]
        EditorGUILayout.LabelField("使用法线", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _use_normal = FindProperty("_use_normal", properties);
        materialEditor.ShaderProperty(_use_normal, "使用法线");
        if (_use_normal.floatValue == 1)
        {
            _NormalMap_tex = FindProperty("_NormalMap_tex", properties);
            materialEditor
                .TexturePropertySingleLine(new GUIContent("法线"),
                _NormalMap_tex);
            _normal_uv = FindProperty("_normal_uv", properties);
            materialEditor.ShaderProperty(_normal_uv, "uv");
            _normal_speed = FindProperty("_normal_speed", properties);
            materialEditor.ShaderProperty(_normal_speed, "流速");
        }

        EditorGUILayout.EndVertical();


#endregion



#region [菲尼尔]

        EditorGUILayout.LabelField("菲尼尔效果", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _use_frensel = FindProperty("_use_frensel", properties);
        materialEditor.ShaderProperty(_use_frensel, "使用菲尼尔");
        if (_use_frensel.floatValue == 1)
        {
            _frensel_range = FindProperty("_frensel_range", properties);
            materialEditor.ShaderProperty(_frensel_range, "范围");
            _frensel_hard = FindProperty("_frensel_hard", properties);
            materialEditor.ShaderProperty(_frensel_hard, "软硬");
            _Frensel_flip = FindProperty("_Frensel_flip", properties);
            materialEditor.ShaderProperty(_Frensel_flip, "翻转");
        }
        EditorGUILayout.EndVertical();


#endregion



#region [边缘深度]

        EditorGUILayout.LabelField("边缘深度", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _use_depth = FindProperty("_use_depth", properties);
        materialEditor.ShaderProperty(_use_depth, "使用边缘深度");
        if (_use_depth.floatValue == 1)
        {
            _Fade_distance = FindProperty("_Fade_distance", properties);
            materialEditor
                .ShaderProperty(_Fade_distance, "深度范围，一般就是1");
            _edge_range = FindProperty("_edge_range", properties);
            materialEditor.ShaderProperty(_edge_range, "范围");
            _edge_hard = FindProperty("_edge_hard", properties);
            materialEditor.ShaderProperty(_edge_hard, "软硬");

            _edge_kill_or_add = FindProperty("_edge_kill_or_add", properties);
            materialEditor
                .ShaderProperty(_edge_kill_or_add, "边缘透明还是叠加");
        }
        EditorGUILayout.EndVertical();


#endregion



#region [溶解]
        EditorGUILayout.LabelField("溶解效果", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _use_dissolve = FindProperty("_use_dissolve", properties);
        materialEditor.ShaderProperty(_use_dissolve, "使用溶解");
        if (_use_dissolve.floatValue == 1)
        {
            _dissolve_tex = FindProperty("_dissolve_tex", properties);
            materialEditor
                .TexturePropertySingleLine(new GUIContent("溶解/遮罩贴图"),
                _dissolve_tex);

            _disslove_uv = FindProperty("_disslove_uv", properties);
            materialEditor.ShaderProperty(_disslove_uv, "溶解uv");
            _disslove_speed = FindProperty("_disslove_speed", properties);
            materialEditor.ShaderProperty(_disslove_speed, "溶解流速");
            _edge_hardness = FindProperty("_edge_hardness", properties);
            materialEditor.ShaderProperty(_edge_hardness, "溶解边缘硬度");
            _dissolve = FindProperty("_dissolve", properties);
            materialEditor.ShaderProperty(_dissolve, "溶解");
            _use_custom1_x_dissolve =
                FindProperty("_use_custom1_x_dissolve", properties);
            materialEditor
                .ShaderProperty(_use_custom1_x_dissolve, "使用custom1_x_溶解");
        }
        EditorGUILayout.EndVertical();
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
            _use_custom1_y_displace =
                FindProperty("_use_custom1_y_displace", properties);
            materialEditor
                .ShaderProperty(_use_custom1_y_displace,
                "shi'y_custom1_y_控制置换强度");
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
            .LabelField("技术谈论Q群:453762906", EditorStyles.miniButton);

        EditorGUILayout.EndVertical();


#endregion

    }
}
