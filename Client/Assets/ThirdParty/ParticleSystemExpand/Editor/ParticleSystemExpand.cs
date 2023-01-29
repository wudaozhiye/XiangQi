using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace ExpandParticleSystem
{

    [CustomEditor(typeof(ParticleSystem))]

    public class ParticleSystemExpand : DecoratorEditor
    {
        public enum CustomData
        { 
            none,x,xy,xyz,xyzw,
        }
        #region 按钮样式
        string buttonGUIStyle = "ButtonRight";
        #endregion
        #region 开关
        public bool isOpen_ParticleTransform = false;//是否打开Transform
        public bool isOpen_ParticlePreset = false;//是否打开粒子预设
        public bool isOpen_Name = false;//是否打开命名更改
        public bool isOpen_CustomData = false;//是否打开自定义数据
        #endregion
        private ParticleSystem thisParticleSystem;//自身的粒子系统
        //配置文件路径
        private string pathSetFile = "ParticlePresetClass";
        #region 粒子自定义数据
        private CustomData customData1 = CustomData.xy, customData2 = CustomData.none;
        private bool customDataUV = true;
        #endregion
        public bool isOpen = false;//是否打开插件
        public float Delay;//统一延迟
        public int ParticleCount;
        private string prefixion, postfix;//前缀和后缀
        public ParticleSystemExpand() : base("ParticleSystemInspector") { }
        private void OnEnable()
        {
            thisParticleSystem = target as ParticleSystem;
        }
        public override void OnInspectorGUI()
        {
            using (new GUILayout.VerticalScope("Box"))
            {
                if (GUILayout.Button("       小布的拓展插件", new GUIStyle(buttonGUIStyle)))
                {
                    isOpen = !isOpen;
                }
                if(isOpen)
                {
                    ///重置Transform
                    GUIUpdate_ParticleTransform();
                    ///粒子预设
                    GUIUpdate_ParticlePreset();
                    ///命名更改
                    GUIUpdate_Name();
                    GUIUpdate_CustomData();
                }
            }
            base.OnInspectorGUI();
        }

     
        #region 粒子预设
        /// <summary>
        /// 将这个层级以下的模式改成Hierarchy
        /// </summary>
        private void AllHierarchy()
        {
            var mainP = thisParticleSystem.main;
            mainP.scalingMode = ParticleSystemScalingMode.Hierarchy;

            foreach (Transform t in thisParticleSystem.GetComponentsInChildren<Transform>())
            {
                ParticleSystem p = t.GetComponent<ParticleSystem>();
                var main = p.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }
        }
        /// <summary>
        /// 将这个层级以下的模式改成Hierarchy
        /// </summary>
        private void AllDelay()
        {
            foreach (Transform t in thisParticleSystem.GetComponentsInChildren<Transform>())
            {
                ParticleSystem p = t.GetComponent<ParticleSystem>();
                var main = p.main;
                main.startDelay = Delay;
            }
        }
        private void AddDelay()
        {
            foreach (Transform t in thisParticleSystem.GetComponentsInChildren<Transform>())
            {
                ParticleSystem p = t.GetComponent<ParticleSystem>();
                var main = p.main;
                float subD = main.startDelay.constant;
                main.startDelay = Delay+ subD;
            }
        }
        private void SDelay()
        {
            foreach (Transform t in thisParticleSystem.GetComponentsInChildren<Transform>())
            {
                ParticleSystem p = t.GetComponent<ParticleSystem>();
                var main = p.main;
                float subD = main.startDelay.constant;
                main.startDelay = subD - Delay;
            }
        }
        private void GUIUpdate_ParticlePreset()
        {
       
            using (new GUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.LabelField("", GUILayout.Width(20));
                if (GUILayout.Button("粒子预设", new GUIStyle(buttonGUIStyle)))
                {
                    isOpen_ParticlePreset = !isOpen_ParticlePreset;
                }
            }
            if (isOpen_ParticlePreset)
            {
                if (GUILayout.Button("以下所有粒子层级Hierarchy"))
                {
                    AllHierarchy();
                }
                if (GUILayout.Button("空粒子"))
                {
                    NullParticle();
                }
                if (GUILayout.Button("单个粒子"))
                {
                    OneParticle();
                }
                if (GUILayout.Button("单个模型"))
                {
                    OneParticle(true);
                }
                if (GUILayout.Button("扩散性光刺"))
                {
                    Radiation();
                }
                if (GUILayout.Button("火星四射"))
                {
                    Spark();
                }
                if (GUILayout.Button("移除没有拖尾的拖尾材质"))
                {
                    RemoveTrail();
                }
                using (new GUILayout.HorizontalScope())
                {
                    Delay = EditorGUILayout.FloatField("延迟时间", Delay);
                    if (GUILayout.Button("统一延迟"))
                    {
                        AllDelay();
                    }
                    if (GUILayout.Button("延迟增加"))
                    {
                        AddDelay();
                    }
                    if (GUILayout.Button("延迟减少"))
                    {
                        SDelay();
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    ParticleCount = EditorGUILayout.IntField("粒子最大个数", ParticleCount);
                    if (GUILayout.Button("设置所有子集粒子最大个数限制"))
                    {
                        SetParticleCount(ParticleCount);
                    }
                }
            }
        }
        public void NullParticle()
        {
            thisParticleSystem.Stop();
            ParticleSystem.EmissionModule e = thisParticleSystem.emission;
            e.enabled = false;
            ParticleSystem.ShapeModule s = thisParticleSystem.shape;
            s.enabled = false;
            var main = thisParticleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            var rendererModule = thisParticleSystem.GetComponent<ParticleSystemRenderer>();
            rendererModule.enabled = false;

            main.loop = false;
            main.startLifetime = 1;
            main.startSize = 0;
            main.duration = 1;
            main.startSpeed = 0;
            main.maxParticles = 1;
            thisParticleSystem.transform.localPosition = Vector3.zero;
            thisParticleSystem.transform.localRotation = Quaternion.Euler(Vector3.zero);
            thisParticleSystem.transform.localScale = Vector3.one;
        }
        public void OneParticle(bool isMesh = false)
        {
            thisParticleSystem.Stop();
            ParticleSystem.MainModule main = thisParticleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.loop = false;
            main.startLifetime = 0.3f;
            main.startSize = 1;
            main.duration = 1;
            main.startSpeed = 0;
            main.maxParticles = 1;
            ParticleSystem.ShapeModule s = thisParticleSystem.shape;
            s.enabled = false;

            ParticleSystem.EmissionModule e = thisParticleSystem.emission;
            e.enabled = true;
            e.rateOverTime = 0;
            e.rateOverDistance = 0;
            ParticleSystem.Burst burst = new ParticleSystem.Burst();
            burst.time = 0;
            burst.count = 1;
            burst.cycleCount = 1;
            burst.repeatInterval = 0.01f;
#if !UNITY_2017 && !UNITY_2018
            burst.probability = 1;
#endif
            e.SetBursts(new ParticleSystem.Burst[] { burst });

            var rendererModule = thisParticleSystem.GetComponent<ParticleSystemRenderer>();
            rendererModule.enabled = true;
            if (isMesh)
            {
                rendererModule.renderMode = ParticleSystemRenderMode.Mesh;
                rendererModule.alignment = ParticleSystemRenderSpace.Local;
            }
        }
        public void SetParticleCount(int count)
        {
            foreach (Transform t in thisParticleSystem.GetComponentsInChildren<Transform>())
            {
                ParticleSystem p = t.GetComponent<ParticleSystem>();
                if (p == null)
                {
                    continue;
                }
                var main = p.main;
                if (main.maxParticles == 1000)
                {
                    main.maxParticles = count;
                }
            }
        }
        public void RemoveTrail()
        {
            foreach (Transform t in thisParticleSystem.GetComponentsInChildren<Transform>())
            {
                ParticleSystem p = t.GetComponent<ParticleSystem>();
                if (p != null && !p.trails.enabled)
                {
                    ParticleSystemRenderer m = t.GetComponent<ParticleSystemRenderer>();
                    if (m != null)
                    {
                        m.trailMaterial = null;
                    }
                }

            }
        }

        /// <summary>
        /// 火渐四射预设
        /// </summary>
        public void Radiation()
        {
            thisParticleSystem.Stop();
            //主面板
            ParticleSystem.MainModule main = thisParticleSystem.main;
            main.loop = false;
            var life = main.startLifetime;
            life.mode = ParticleSystemCurveMode.TwoConstants;
            life.constantMin = 0.3f;
            life.constantMax = 0.5f;
            main.startLifetime = life;
            main.startSpeed = 0.01f;
            var size = main.startSize;
            size.mode = ParticleSystemCurveMode.TwoConstants;
            size.constantMin = 0.8f;
            size.constantMax = 1f;
            main.startSize = size;
            main.duration = 1;
            main.maxParticles = 16;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            //发射器
            ParticleSystem.EmissionModule e = thisParticleSystem.emission;
            e.enabled = true;
            e.rateOverTime = 0;
            e.rateOverDistance = 0;
            ParticleSystem.Burst burst = new ParticleSystem.Burst();
            burst.time = 0;
            burst.count = 8;
            burst.cycleCount = 2;
            burst.repeatInterval = 0.1f;
#if !UNITY_2017 && !UNITY_2018
            burst.probability = 1;
#endif
            e.SetBursts(new ParticleSystem.Burst[] { burst });
            //形状
            ParticleSystem.ShapeModule s = thisParticleSystem.shape;
            s.enabled = true;
            ParticleSystem.SizeOverLifetimeModule sizeOverLife = thisParticleSystem.sizeOverLifetime;
            sizeOverLife.enabled = true;
            var sizeMode = sizeOverLife.size;
            sizeMode.mode = ParticleSystemCurveMode.Curve;
            sizeOverLife.size = sizeMode;
            //渲染器
            var rendererModule = thisParticleSystem.GetComponent<ParticleSystemRenderer>();
            rendererModule.enabled = true;
            rendererModule.renderMode = ParticleSystemRenderMode.Stretch;
            rendererModule.lengthScale = -2;
            rendererModule.velocityScale = 2;

        }
        //火星四溅
        public void Spark()
        {

            thisParticleSystem.Stop();
            //主面板
            ParticleSystem.MainModule main = thisParticleSystem.main;
            main.loop = false;
            var life = main.startLifetime;
            life.mode = ParticleSystemCurveMode.TwoConstants;
            life.constantMin = 0.8f;
            life.constantMax = 1.1f;
            main.startLifetime = life;
            var speed = main.startSpeed;
            speed.mode = ParticleSystemCurveMode.TwoConstants;
            speed.constantMax = 18;
            speed.constantMin = 15;
            main.startSpeed = speed;
            var size = main.startSize;
            size.mode = ParticleSystemCurveMode.TwoConstants;
            size.constantMin = 0.8f;
            size.constantMax = 1f;
            main.startSize = size;
            main.duration = 1;
            main.maxParticles = 16;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            //发射器
            ParticleSystem.EmissionModule e = thisParticleSystem.emission;
            e.enabled = true;
            e.rateOverTime = 0;
            e.rateOverDistance = 0;
            ParticleSystem.Burst burst = new ParticleSystem.Burst();
            burst.time = 0;
            burst.count = 8;
            burst.cycleCount = 2;
            burst.repeatInterval = 0.1f;
#if !UNITY_2017 && !UNITY_2018
            burst.probability = 1;
#endif
            e.SetBursts(new ParticleSystem.Burst[] { burst });
            //形状
            ParticleSystem.ShapeModule s = thisParticleSystem.shape;
            s.enabled = true;
            s.shapeType = ParticleSystemShapeType.Cone;
            s.angle = 35;
            ParticleSystem.LimitVelocityOverLifetimeModule lm = thisParticleSystem.limitVelocityOverLifetime;
            lm.enabled = true;
            lm.dampen = 0.3f;
            var drag = lm.drag;
            drag.mode = ParticleSystemCurveMode.TwoConstants;
            drag.constantMin = 0.08f;
            drag.constantMax = 0.15f;
            lm.drag = drag;

            ParticleSystem.NoiseModule noise = thisParticleSystem.noise;
            noise.enabled = true;
            noise.strength = 0.65f;
            noise.frequency = 0.4f;
            noise.scrollSpeed = 0.3f;

            ParticleSystem.SizeOverLifetimeModule sizeOverLife = thisParticleSystem.sizeOverLifetime;
            sizeOverLife.enabled = true;
            var sizeMode = sizeOverLife.size;
            sizeMode.mode = ParticleSystemCurveMode.Curve;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0, 0.5f);
            curve.AddKey(0.42f, 1f);
            curve.AddKey(0.6f, 0.2f);
            curve.AddKey(0.68f, 0.6f);
            curve.AddKey(0.88f, 0.1f);
            curve.AddKey(0.9f, 0.5f);
            curve.AddKey(1f, 0f);
            sizeMode.curve = curve;
            sizeOverLife.size = sizeMode;

        }
        #endregion
        #region Transform
        private void GUIUpdate_ParticleTransform()
        {
            using (new GUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.LabelField("", GUILayout.Width(20));
                if (GUILayout.Button("Transform", new GUIStyle(buttonGUIStyle)))
                {
                    isOpen_ParticleTransform = !isOpen_ParticleTransform;
                }
            }
            if (isOpen_ParticleTransform)
            {
                if (GUILayout.Button("重置"))
                {
                    thisParticleSystem.transform.localPosition = Vector3.zero;
                    thisParticleSystem.transform.localEulerAngles = Vector3.zero;
                    thisParticleSystem.transform.localScale = Vector3.one;
                }
                if (GUILayout.Button("重置位置"))
                {
                    thisParticleSystem.transform.localPosition = Vector3.zero;
                }
                using (new GUILayout.HorizontalScope("Box"))
                {
                    if (GUILayout.Button("重置旋转"))
                    {
                        thisParticleSystem.transform.localEulerAngles = Vector3.zero;
                    }
                    if (GUILayout.Button("旋转90°"))
                    {
                        thisParticleSystem.transform.localEulerAngles = new Vector3(90, 0, 0);
                    }
                    if (GUILayout.Button("旋转-90°"))
                    {
                        thisParticleSystem.transform.localEulerAngles = new Vector3(-90, 0, 0);
                    }
                }
                if (GUILayout.Button("重置大小"))
                {
                    thisParticleSystem.transform.localScale = Vector3.one;
                }
            }
        }
        #endregion
        #region 命名
        private void GUIUpdate_Name()
        {
            using (new GUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.LabelField("", GUILayout.Width(20));
                if (GUILayout.Button("统一更改命名", new GUIStyle(buttonGUIStyle)))
                {
                    isOpen_Name = !isOpen_Name;
                }
            }
            if (isOpen_Name)
            {
                using (new GUILayout.HorizontalScope())
                {
                    prefixion = EditorGUILayout.TextField("前缀", prefixion);
                    if (GUILayout.Button("添加前缀"))
                    {
                        AddPrefixion(prefixion);
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    postfix = EditorGUILayout.TextField("后缀", postfix);
                    if (GUILayout.Button("添加后缀"))
                    {
                        AddPrefixion(postfix, false);
                    }
                   
                }
                if (GUILayout.Button("添加数字后缀"))
                {
                    AddNumberPrefixion();
                }
            }
        }
        private void AddPrefixion(string p,bool front = true)
        {
            Transform[] gameObjects = thisParticleSystem.GetComponentsInChildren<Transform>();
            for (int i = 1; i < gameObjects.Length; i++)
            {
                if (front)
                {
                    gameObjects[i].name = p + gameObjects[i].name;
                }
                else
                {
                    gameObjects[i].name += p; ;
                }
            }
            
        }
        private void AddNumberPrefixion()
        {
            Transform[] gameObjects = thisParticleSystem.GetComponentsInChildren<Transform>();
            for (int i = 1; i < gameObjects.Length; i++)
            {
                    gameObjects[i].name += "_"+i ;
            }

        }
        #endregion
        #region 一键自定义数据
        private void CustomDataSwitch(CustomData customData, List<ParticleSystemVertexStream> Custom,bool isCustom1 = true)
        {
            int custom2 = isCustom1 ? 0 : 4;
            switch (customData)
            {
                case CustomData.x:
                    {
                        Custom.Add(ParticleSystemVertexStream.Custom1X + custom2);
                        break;
                    }
                case CustomData.xy:
                    {
                        Custom.Add(ParticleSystemVertexStream.Custom1XY + custom2);
                        break;
                    }
                case CustomData.xyz:
                    {
                        Custom.Add(ParticleSystemVertexStream.Custom1XYZ + custom2);
                        break;
                    }
                case CustomData.xyzw:
                    {
                        Custom.Add(ParticleSystemVertexStream.Custom1XYZW + custom2);
                        break;
                    }
            }
        }
        private void GUIUpdate_CustomData()
        {
            using (new GUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.LabelField("", GUILayout.Width(20));
                if (GUILayout.Button("一键自定义数据", new GUIStyle(buttonGUIStyle)))
                {
                    isOpen_CustomData = !isOpen_CustomData;
                }
            }
            if (isOpen_CustomData)
            {
                var rendererModule = thisParticleSystem.GetComponent<ParticleSystemRenderer>();
                rendererModule.enabled = true;
                //原有的顶点数据
                List<ParticleSystemVertexStream> vertexData = new List<ParticleSystemVertexStream>(new ParticleSystemVertexStream[]
                    {
                        ParticleSystemVertexStream.Position,
                        ParticleSystemVertexStream.Normal,
                        ParticleSystemVertexStream.Color,
                        ParticleSystemVertexStream.UV,
                    });
                customDataUV = EditorGUILayout.Toggle("是否占掉UV0通道", customDataUV);
                using (new GUILayout.HorizontalScope("Box"))
                {
                    EditorGUILayout.LabelField("customData1 : ");
                    customData1 = (CustomData)EditorGUILayout.EnumPopup(customData1);

                }
                using (new GUILayout.HorizontalScope("Box"))
                {
                    EditorGUILayout.LabelField("customData2 : ");
                    customData2 = (CustomData)EditorGUILayout.EnumPopup(customData2);

                }
                if (GUILayout.Button("一键开启"))
                {
                    List<ParticleSystemVertexStream> Customs = new List<ParticleSystemVertexStream>(vertexData);
                    if(customDataUV)
                    {
                        Customs.Add(ParticleSystemVertexStream.UV2);
                    }
                    CustomDataSwitch(customData1, Customs);
                    CustomDataSwitch(customData2, Customs, false);
                    rendererModule.SetActiveVertexStreams(Customs);
                }
            }
        }
        #endregion

    }
}

