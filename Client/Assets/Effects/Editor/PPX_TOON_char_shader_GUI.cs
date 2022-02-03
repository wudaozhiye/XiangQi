using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PPX_TOON_char_shader_GUI : ShaderGUI
{
    MaterialProperty _ASEOutlineWidth;

    MaterialProperty _ASEOutlineColor;

    MaterialProperty _ASEOutalpha;

    MaterialProperty _Cutoff;

    MaterialProperty _shadow_clip;

    MaterialProperty _shadow_edge;

    MaterialProperty _normal_map;

    MaterialProperty _BaseTex;

    MaterialProperty _light;

    MaterialProperty _drak;

    MaterialProperty _use_second_tex = null;

    MaterialProperty _BuffTex;

    MaterialProperty _BuffTex_switch;

    MaterialProperty _BuffTex_switch_edge_hardness;

    MaterialProperty _BuffTex_switch_dissolve;

    MaterialProperty _use_matcat = null;

    MaterialProperty _matcap;

    MaterialProperty _special_buff_switch;

    MaterialProperty _special_buff_switch_edge_hardness;

    MaterialProperty _special_buff_dissolve;

    MaterialProperty _use_frensel = null;

    MaterialProperty _frensel_range;

    MaterialProperty _frensel_hard;

    MaterialProperty _frensel_power;

    MaterialProperty _frensel_color;

    MaterialProperty _use_emissive = null;

    MaterialProperty _Emissive_Tex;

    MaterialProperty _Emissve_color;

    MaterialProperty _Emissve_power;

    MaterialProperty _use_dissolve = null;

    MaterialProperty _dissolve;

    MaterialProperty _dissolve_edge_dissolve;

    MaterialProperty _edge_width;

    MaterialProperty _edge_clip;

    MaterialProperty _dissolve_Emissve_color;

    MaterialProperty _dissolve_Emissve_power;

    // // MaterialProperty _numdis;
    // MaterialProperty _num;
    // MaterialProperty _numDistort;
    // MaterialProperty _numDissolve;
    // enum Channel
    // {
    //     R,
    //     G,
    //     B,
    //     A
    // }
    // string[] ChannelNames = System.Enum.GetNames(typeof (Channel));
    // string[] ChannelNames2 = System.Enum.GetNames(typeof (Channel));
    // string[] ChannelNames3 = System.Enum.GetNames(typeof (Channel));
    // string[] ChannelNames4 = System.Enum.GetNames(typeof (Channel));
    // MaterialProperty _distort_mask;
    // MaterialProperty _choose_distort_mask;
    // MaterialProperty _numDistortMask;
    // MaterialProperty _SaveValue;
    MaterialEditor m_MaterialEditor;

    Material m_Material;

    public override void OnGUI(
        MaterialEditor materialEditor,
        MaterialProperty[] properties
    )
    {
        EditorGUILayout
            .LabelField("皮皮虾，最好的伙伴！！！", EditorStyles.miniButton);


#region [基础光照]
        EditorGUILayout.LabelField("描边设置", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _ASEOutlineWidth = FindProperty("_ASEOutlineWidth", properties);
        materialEditor.ShaderProperty(_ASEOutlineWidth, "描边宽度");

        _ASEOutlineColor = FindProperty("_ASEOutlineColor", properties);
        materialEditor.ShaderProperty(_ASEOutlineColor, "描边颜色");

        // //
        // _ASEOutalpha = FindProperty("_ASEOutalpha", properties);
        // materialEditor
        //     .ShaderProperty(_ASEOutalpha, "描边透明度,使用溶解时需要设为负数");
        //
        EditorGUILayout.EndVertical();
#endregion



#region [基础光照]
        EditorGUILayout.LabelField("基础光照", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _BaseTex = FindProperty("_BaseTex", properties);
        materialEditor.ShaderProperty(_BaseTex, "基础贴图");

        _normal_map = FindProperty("_normal_map", properties);
        materialEditor.ShaderProperty(_normal_map, "法线贴图");

        _shadow_clip = FindProperty("_shadow_clip", properties);
        materialEditor.ShaderProperty(_shadow_clip, "阴影范围");

        _shadow_edge = FindProperty("_shadow_edge", properties);
        materialEditor.ShaderProperty(_shadow_edge, "阴影软硬程度");

        _light = FindProperty("_light", properties);
        materialEditor.ShaderProperty(_light, "亮面叠加颜色");

        _drak = FindProperty("_drak", properties);
        materialEditor.ShaderProperty(_drak, "暗面叠加颜色");
        EditorGUILayout.EndVertical();
#endregion



#region [变身]

        EditorGUILayout.LabelField("变身效果", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _use_second_tex = FindProperty("_use_second_tex", properties);
        materialEditor.ShaderProperty(_use_second_tex, "使用第二张贴图");

        if (_use_second_tex.floatValue == 1)
        {
            _BuffTex = FindProperty("_BuffTex", properties);
            materialEditor.ShaderProperty(_BuffTex, "第二张贴图");

            _BuffTex_switch = FindProperty("_BuffTex_switch", properties);
            materialEditor.ShaderProperty(_BuffTex_switch, "第二纹理切换贴图");

            _BuffTex_switch_edge_hardness =
                FindProperty("_BuffTex_switch_edge_hardness", properties);
            materialEditor
                .ShaderProperty(_BuffTex_switch_edge_hardness, "切换软硬程度");

            _BuffTex_switch_dissolve =
                FindProperty("_BuffTex_switch_dissolve", properties);
            materialEditor.ShaderProperty(_BuffTex_switch_dissolve, "切换纹理");
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("特殊buff效果", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _use_matcat = FindProperty("_use_matcat", properties);
        materialEditor.ShaderProperty(_use_matcat, "使用matcap贴图");

        if (_use_matcat.floatValue == 1)
        {
            _matcap = FindProperty("_matcap", properties);
            materialEditor.ShaderProperty(_matcap, "matcap贴图");

            _special_buff_switch =
                FindProperty("_special_buff_switch", properties);
            materialEditor
                .ShaderProperty(_special_buff_switch, "matcap贴图切换贴图");

            _special_buff_switch_edge_hardness =
                FindProperty("_special_buff_switch_edge_hardness", properties);
            materialEditor
                .ShaderProperty(_special_buff_switch_edge_hardness,
                "切换软硬程度");

            _special_buff_dissolve =
                FindProperty("_special_buff_dissolve", properties);
            materialEditor.ShaderProperty(_special_buff_dissolve, "切换纹理");
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("边缘光效果", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _use_frensel = FindProperty("_use_frensel", properties);
        materialEditor.ShaderProperty(_use_frensel, "使用边缘光");

        if (_use_frensel.floatValue == 1)
        {
            _frensel_range = FindProperty("_frensel_range", properties);
            materialEditor.ShaderProperty(_frensel_range, "边缘光范围");

            _frensel_hard = FindProperty("_frensel_hard", properties);
            materialEditor.ShaderProperty(_frensel_hard, "边缘光软硬");

            _frensel_power = FindProperty("_frensel_power", properties);
            materialEditor.ShaderProperty(_frensel_power, "边缘光亮度");

            _frensel_color = FindProperty("_frensel_color", properties);
            materialEditor.ShaderProperty(_frensel_color, "边缘光颜色");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout
            .LabelField("自发光效果追加，强度为负数时可以变为暗纹",
            EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _use_emissive = FindProperty("_use_emissive", properties);
        materialEditor.ShaderProperty(_use_emissive, "追加自发光");
        if (_use_emissive.floatValue == 1)
        {
            _Emissive_Tex = FindProperty("_Emissive_Tex", properties);
            materialEditor.ShaderProperty(_Emissive_Tex, "自发光贴图");
            _Emissve_color = FindProperty("_Emissve_color", properties);
            materialEditor.ShaderProperty(_Emissve_color, "自发光颜色");
            _Emissve_power = FindProperty("_Emissve_power", properties);
            materialEditor.ShaderProperty(_Emissve_power, "自发光强度");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("溶解效果", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _use_dissolve = FindProperty("_use_dissolve", properties);
        materialEditor.ShaderProperty(_use_dissolve, "使用溶解");

        _ASEOutalpha = FindProperty("_ASEOutalpha", properties);

        if (_use_dissolve.floatValue == 1)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout
                .LabelField("注意！使用溶解时这个参数需要设为负数",
                EditorStyles.miniButton);
            _ASEOutalpha = FindProperty("_ASEOutalpha", properties);
            materialEditor.ShaderProperty(_ASEOutalpha, "描边透明度");
            EditorGUILayout.EndVertical();

            //
            // _ASEOutalpha = FindProperty("_ASEOutalpha", properties);
            // // materialEditor.ShaderProperty(_ASEOutalpha, "描边透明度,使用溶解时需要设为负数");
            // // _ASEOutalpha.set(-0.1);
            // // _ASEOutalpha.vectorValue =  Vector4(-1, 0, 0, 0);
            // _ASEOutalpha.floatValue = (-1);
            //
            _dissolve = FindProperty("_dissolve", properties);
            materialEditor.ShaderProperty(_dissolve, "溶解贴图");

            _dissolve_edge_dissolve =
                FindProperty("_dissolve_edge_dissolve", properties);
            materialEditor.ShaderProperty(_dissolve_edge_dissolve, "溶解程度");

            _edge_width = FindProperty("_edge_width", properties);
            materialEditor.ShaderProperty(_edge_width, "溶解亮边软硬");
            _edge_clip = FindProperty("_edge_clip", properties);
            materialEditor.ShaderProperty(_edge_clip, "溶解亮边宽度");
            _dissolve_Emissve_power =
                FindProperty("_dissolve_Emissve_power", properties);
            materialEditor
                .ShaderProperty(_dissolve_Emissve_power, "溶解亮边强度");

            _dissolve_Emissve_color =
                FindProperty("_dissolve_Emissve_color", properties);
            materialEditor
                .ShaderProperty(_dissolve_Emissve_color, "溶解亮边颜色");
            // EditorGUILayout.EndVertical();
        }

        // else
        // {
        //     _ASEOutalpha.floatValue = (0);
        // }
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

        //////////////////
        /////////////////
    }
}
