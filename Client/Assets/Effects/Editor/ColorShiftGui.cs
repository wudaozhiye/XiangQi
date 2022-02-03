using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColorShiftGui : ShaderGUI
{
    MaterialProperty _ZWrite;

    MaterialProperty _ZTest;

    MaterialProperty _Cull;

    MaterialProperty _mask;

    MaterialProperty _Base_alpha;

    MaterialProperty _shift;

    MaterialProperty _scale;

    MaterialProperty _mask2;

    MaterialProperty _noise_mask_uv;

    MaterialProperty _custom1_x_control_shift;

    MaterialProperty _custom1_zw_move_uv;

    MaterialProperty _X_speed;

    MaterialProperty _Y_speed;

    MaterialProperty _speed;

    public override void OnGUI(
        MaterialEditor materialEditor,
        MaterialProperty[] properties
    )
    {
        EditorGUILayout.LabelField("色相偏移", EditorStyles.miniButton);


#region [基础贴图]

        EditorGUILayout.LabelField("基础部分", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _mask = FindProperty("_mask", properties);
        materialEditor.TextureProperty(_mask, "基础遮罩");

        _Base_alpha = FindProperty("_Base_alpha", properties);
        materialEditor.ShaderProperty(_Base_alpha, "基础透明度");

        _shift = FindProperty("_shift", properties);
        materialEditor.ShaderProperty(_shift, "色相分离强度");

        _scale = FindProperty("_scale", properties);
        materialEditor.ShaderProperty(_scale, "色相分离强度常数（一般为0.1）");

        _custom1_x_control_shift =
            FindProperty("_custom1_x_control_shift", properties);
        materialEditor
            .ShaderProperty(_custom1_x_control_shift,
            "使用custom1_x控制分离强度");
        EditorGUILayout.EndVertical();
#endregion



#region [遮罩贴图]

        EditorGUILayout.LabelField("遮罩部分", EditorStyles.miniButton);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _mask2 = FindProperty("_mask2", properties);

        materialEditor
            .TexturePropertySingleLine(new GUIContent("噪波贴图"), _mask2);

        _noise_mask_uv = FindProperty("_noise_mask_uv", properties);
        materialEditor.ShaderProperty(_noise_mask_uv, "噪波贴图uv");

        _X_speed = FindProperty("_X_speed", properties);
        materialEditor.ShaderProperty(_X_speed, "x方向位移");

        _Y_speed = FindProperty("_Y_speed", properties);
        materialEditor.ShaderProperty(_Y_speed, "Y方向位移");

        _speed = FindProperty("_speed", properties);
        materialEditor.ShaderProperty(_speed, "位移速度");

        _custom1_zw_move_uv = FindProperty("_custom1_zw_move_uv", properties);
        materialEditor
            .ShaderProperty(_custom1_zw_move_uv, "使用custom1_zw控制噪波位移");

        EditorGUILayout.EndVertical();
#endregion



#region [额外设置]
        EditorGUILayout.LabelField("额外设置", EditorStyles.miniButton);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _ZWrite = FindProperty("_ZWrite", properties);
        materialEditor.ShaderProperty(_ZWrite, "ZWrite");

        _ZTest = FindProperty("_ZTest", properties);
        materialEditor.ShaderProperty(_ZTest, "ZTest");

        _Cull = FindProperty("_Cull", properties);
        materialEditor.ShaderProperty(_Cull, "Cull");

        // float _SaveValue01x=showdark?1:0;
        // float _SaveValue01y=showdark?1:0;
        // Vector4 _SaveValue01=new Vector4(_SaveValue01x,0,0,0);
        // _SaveValue.vectorValue=_SaveValue01;
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
