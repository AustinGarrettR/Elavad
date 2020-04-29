//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

namespace JBooth.MicroSplat
{
   #if __MICROSPLAT__
   [InitializeOnLoad]
   public class MicroSplatSnow : FeatureDescriptor
   {
      const string sSnowDefine = "__MICROSPLAT_SNOW__";
      static MicroSplatSnow()
      {
         MicroSplatDefines.InitDefine(sSnowDefine);
      }
      [PostProcessSceneAttribute (0)]
      public static void OnPostprocessScene()
      { 
         MicroSplatDefines.InitDefine(sSnowDefine);
      }

      public override string ModuleName()
      {
         return "Snow";
      }

      public enum SnowDefineFeature
      {
         _SNOW,
         _SNOWSIMPLE,
         _SNOWNORMALNOISE,
         _SNOWDISTANCERESAMPLE,
         _PERTEXSNOWSTRENGTH,
         _USEGLOBALSNOWLEVEL,
         _USEGLOBALSNOWHEIGHT,
         _SNOWSSS,
#if __MICROSPLAT_TEXTURECLUSTERS__
         _SNOWSTOCHASTIC,
#endif
         kNumFeatures,
      }

      public enum SnowMode
      {
         None,
         Simple,
         Rich
      }

      static TextAsset properties_snow;
      static TextAsset properties_snow_normalNoise;
      static TextAsset properties_snow_distanceResample;
      static TextAsset func_snow;
      static TextAsset cbuffer_snow;

      public SnowMode snow = SnowMode.None;
      public bool snowNormalNoise = false;
      public bool snowDistanceResample = false;
      public bool perTexSnow = false;
      public bool globalLevel = false;
      public bool globalHeight = false;
      public bool snowSSS = false;
#if __MICROSPLAT_TEXTURECLUSTERS__
      public bool snowStochastic = false;
#endif



      GUIContent CShaderSnow = new GUIContent("Global Snow", "Enabled Global Snow. Simple mode matches the CPU side calculations, Rich snow has more detail, but may not match the CPU snow exactly");
      GUIContent CShaderNormalNoise = new GUIContent("Snow Normal Noise", "Blends in a low res normal map to break up tiling in the snow");
      GUIContent CShaderSnowDistanceResample = new GUIContent("Snow Distance Resample", "When enabled, snow texture is resamples and blended in with itself at different UV scale in the distance");
      GUIContent CDistanceNoise = new GUIContent("Noise", "A mostly greyscale linear texture with the center around 0.5");
#if __MICROSPLAT_TEXTURECLUSTERS__
      GUIContent CShaderSnowStochastic = new GUIContent("Snow Stochastic", "Use stochastic sampling for snow");
#endif
      GUIContent CShaderSnowSSS = new GUIContent ("Snow Subsurface Scattering", "Use Subsurface Scattering on snow");

      // Can we template these somehow?
      static Dictionary<SnowDefineFeature, string> sFeatureNames = new Dictionary<SnowDefineFeature, string>();
      public static string GetFeatureName(SnowDefineFeature feature)
      {
         string ret;
         if (sFeatureNames.TryGetValue(feature, out ret))
         {
            return ret;
         }
         string fn = System.Enum.GetName(typeof(SnowDefineFeature), feature);
         sFeatureNames[feature] = fn;
         return fn;
      }

      public static bool HasFeature(string[] keywords, SnowDefineFeature feature)
      {
         string f = GetFeatureName(feature);
         for (int i = 0; i < keywords.Length; ++i)
         {
            if (keywords[i] == f)
               return true;
         }
         return false;
      }

      public static bool HasFeature (string [] keywords, string f)
      {
         for (int i = 0; i < keywords.Length; ++i)
         {
            if (keywords [i] == f)
               return true;
         }
         return false;
      }


      public override string GetVersion()
      {
         return "3.3";
      }

      public override void DrawFeatureGUI(MicroSplatKeywords keywords)
      {
         snow = (SnowMode)EditorGUILayout.EnumPopup(CShaderSnow, snow);
         if (snow != SnowMode.None)
         {
            EditorGUI.indentLevel++;
            snowSSS = EditorGUILayout.Toggle (CShaderSnowSSS, snowSSS);
            snowNormalNoise = EditorGUILayout.Toggle(CShaderNormalNoise, snowNormalNoise);
            snowDistanceResample = EditorGUILayout.Toggle(CShaderSnowDistanceResample, snowDistanceResample);
#if __MICROSPLAT_TEXTURECLUSTERS__
            snowStochastic = EditorGUILayout.Toggle(CShaderSnowStochastic, snowStochastic);
#endif
            EditorGUI.indentLevel--;
         }
      }

      static GUIContent CDiffTex = new GUIContent("Diffuse/Height", "Diffuse with height map in alpha for snow");
      static GUIContent CNormTex = new GUIContent("NormalSAO", "Normal, smoothness, and ao for snow");
      static GUIContent CHeightClear = new GUIContent("Height Clearing", "Causes snow to melt on higher parts of the texture");
      static GUIContent CErosionClearing = new GUIContent("Erosion Clearing", "Causes snow to clear on cliff edges and based on AO");
      static GUIContent CHeightRange = new GUIContent("Height Range", "Start and end height for snow coverage");
      static GUIContent CAngleRange = new GUIContent("Angle Range", "Causes snow to not appears at certain angles");
      static GUIContent CCrystals = new GUIContent("Crystals", "Blend between soft and icy snow");
      static GUIContent CMelt = new GUIContent("Melt", "Creates an area of wetness around the snow edge");
      static GUIContent CUpVector = new GUIContent("Snow Up Vector", "Direction snow came from");
      static GUIContent CSSSTint = new GUIContent ("Subsurface Tint", "Snow SSS Tint color");
      static GUIContent CSSSThickness = new GUIContent ("Subsurface Thickness Mult", "Multiplier for subsurface thickness");

      public override void DrawShaderGUI(MicroSplatShaderGUI shaderGUI, MicroSplatKeywords keywords, Material mat, MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (snow != SnowMode.None && mat.HasProperty("_SnowParams") && MicroSplatUtilities.DrawRollup("Snow"))
         {
            var snowDiff = shaderGUI.FindProp("_SnowDiff", props);
            var snowNorm = shaderGUI.FindProp("_SnowNormal", props);
            materialEditor.TexturePropertySingleLine(CDiffTex, snowDiff);
            materialEditor.TexturePropertySingleLine(CNormTex, snowNorm);
            MicroSplatUtilities.EnforceDefaultTexture(snowDiff, "microsplat_def_snow_diff");
            MicroSplatUtilities.EnforceDefaultTexture(snowNorm, "microsplat_def_snow_normsao");

            if (mat.HasProperty("_SnowUVScales"))
            {
               Vector4 snowUV = shaderGUI.FindProp("_SnowUVScales", props).vectorValue;
               EditorGUI.BeginChangeCheck();
               EditorGUILayout.BeginHorizontal();
               EditorGUILayout.PrefixLabel("UV Scale");
               snowUV.x = EditorGUILayout.FloatField(snowUV.x);
               snowUV.y = EditorGUILayout.FloatField(snowUV.y);
               EditorGUILayout.EndHorizontal();
               if (EditorGUI.EndChangeCheck())
               {
                  shaderGUI.FindProp("_SnowUVScales", props).vectorValue = snowUV;
                  EditorUtility.SetDirty(mat);
               }
            }
            if (mat.HasProperty("_SnowSSSTint"))
            {
               var prop = shaderGUI.FindProp ("_SnowSSSTint", props);
               EditorGUI.BeginChangeCheck ();
               Color c = prop.colorValue;
               c = EditorGUILayout.ColorField (CSSSTint, c);
               c.a = EditorGUILayout.Slider (CSSSThickness, c.a, 0, 1);

               if (EditorGUI.EndChangeCheck ())
               {
                  prop.colorValue = c;
                  EditorUtility.SetDirty (mat);
               }
            }


            // influence, erosion, crystal, melt
            Vector4 p1 = shaderGUI.FindProp("_SnowParams", props).vectorValue;
            Vector4 hr = shaderGUI.FindProp("_SnowHeightAngleRange", props).vectorValue;

            EditorGUILayout.BeginHorizontal();
            bool oldEnabled = GUI.enabled;
            if (globalLevel)
            {
               GUI.enabled = false;
            }
            materialEditor.ShaderProperty(shaderGUI.FindProp("_SnowAmount", props), "Amount");
            GUI.enabled = oldEnabled;
            globalLevel = DrawGlobalToggle(GetFeatureName(SnowDefineFeature._USEGLOBALSNOWLEVEL), keywords);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            if (snow == SnowMode.Rich)
            {
               p1.x = EditorGUILayout.Slider(CHeightClear, p1.x, 0, 1);
            }
            p1.y = EditorGUILayout.Slider(CErosionClearing, p1.y, 0, 1);

            EditorGUILayout.BeginHorizontal();
            oldEnabled = GUI.enabled;
            if (globalHeight)
            {
               GUI.enabled = false;
            }
            EditorGUILayout.PrefixLabel(CHeightRange);
            hr.x = EditorGUILayout.FloatField(hr.x); 
            hr.y = EditorGUILayout.FloatField(hr.y);
            GUI.enabled = oldEnabled;
            globalHeight = DrawGlobalToggle(GetFeatureName(SnowDefineFeature._USEGLOBALSNOWHEIGHT), keywords);
            EditorGUILayout.EndHorizontal();


            hr.z = 1.0f - hr.z;
            hr.w = 1.0f - hr.w;
            EditorGUILayout.MinMaxSlider(CAngleRange, ref hr.w, ref hr.z, 0.0f, 1.0f);
            hr.z = 1.0f - hr.z;
            hr.w = 1.0f - hr.w;

            p1.z = EditorGUILayout.FloatField(CCrystals, p1.z);
            p1.w = EditorGUILayout.Slider(CMelt, p1.w, 0, 0.6f);

            if (EditorGUI.EndChangeCheck())
            {
               shaderGUI.FindProp("_SnowParams", props).vectorValue = p1;
               shaderGUI.FindProp("_SnowHeightAngleRange", props).vectorValue = hr;
            }

            Vector4 up = mat.GetVector("_SnowUpVector");
            EditorGUI.BeginChangeCheck();
            Vector3 newUp = EditorGUILayout.Vector3Field(CUpVector, new Vector3(up.x, up.y, up.z));
            if (EditorGUI.EndChangeCheck())
            {
               newUp.Normalize();
               mat.SetVector("_SnowUpVector", new Vector4(newUp.x, newUp.y, newUp.z, 0));
               EditorUtility.SetDirty(mat);
            }
               
            if (snowNormalNoise)
            {
               if (mat.HasProperty("_SnowNormalNoise"))
               {
                  var texProp = shaderGUI.FindProp("_SnowNormalNoise", props);
                  materialEditor.TexturePropertySingleLine(CDistanceNoise, texProp);
                  MicroSplatUtilities.EnforceDefaultTexture(texProp, "microsplat_def_snow_normalnoise");


                  Vector4 scaleStr = shaderGUI.FindProp("_SnowNormalNoiseScaleStrength", props).vectorValue;
                  Vector4 newScaleStr = scaleStr;
                  newScaleStr.x = EditorGUILayout.FloatField("Noise UV Scale", scaleStr.x);
                  newScaleStr.y = EditorGUILayout.FloatField("Noise Strength", scaleStr.y);
                  if (newScaleStr != scaleStr)
                  {
                     shaderGUI.FindProp("_SnowNormalNoiseScaleStrength", props).vectorValue = newScaleStr;
                  }
               }
            }
            if (snowDistanceResample)
            {
               if (mat.HasProperty("_SnowDistanceResampleScaleStrengthFade"))
               {
                  Vector4 scaleStr = shaderGUI.FindProp("_SnowDistanceResampleScaleStrengthFade", props).vectorValue;
                  Vector4 newScaleStr = scaleStr;
                  newScaleStr.x = EditorGUILayout.FloatField("Resample UV Scale", scaleStr.x);
                  newScaleStr.y = EditorGUILayout.FloatField("Resample Strength", scaleStr.y);
                  newScaleStr.z = EditorGUILayout.FloatField("Resample Fade Start", scaleStr.z);
                  newScaleStr.w = EditorGUILayout.FloatField("Resample Fade End", scaleStr.w);
                  if (newScaleStr != scaleStr)
                  {
                     shaderGUI.FindProp("_SnowDistanceResampleScaleStrengthFade", props).vectorValue = newScaleStr;
                  }
               }
            }

#if __MICROSPLAT_TEXTURECLUSTERS__
            if (snowStochastic)
            {
               if (mat.HasProperty("_SnowStochasticScale"))
               {
                  materialEditor.RangeProperty(shaderGUI.FindProp("_SnowStochasticScale", props), "Stochastic Scale");
                  materialEditor.RangeProperty(shaderGUI.FindProp("_SnowStochasticContrast", props), "Stochastic Contrast");
               }
            }
#endif

            if (mat.HasProperty("_TessDisplaceSnowMultiplier"))
            {
               materialEditor.RangeProperty (shaderGUI.FindProp ("_TessDisplaceSnowMultiplier", props), "Displacement Multiplier");
            }
         }
      }

      public override string[] Pack()
      {
         List<string> features = new List<string>();
         if (snow != SnowMode.None)
         {
            features.Add(GetFeatureName(SnowDefineFeature._SNOW));
            if (snow == SnowMode.Simple)
            {
               features.Add(GetFeatureName(SnowDefineFeature._SNOWSIMPLE));
            }
            if (snowNormalNoise)
            {
               features.Add(GetFeatureName(SnowDefineFeature._SNOWNORMALNOISE));
            }
            if (snowDistanceResample)
            {
               features.Add(GetFeatureName(SnowDefineFeature._SNOWDISTANCERESAMPLE));
            }
            if (perTexSnow)
            {
               features.Add(GetFeatureName(SnowDefineFeature._PERTEXSNOWSTRENGTH));
            }
            if (globalLevel)
            {
               features.Add(GetFeatureName(SnowDefineFeature._USEGLOBALSNOWLEVEL));
            }
            if (globalHeight)
            {
               features.Add(GetFeatureName(SnowDefineFeature._USEGLOBALSNOWHEIGHT));
            }

            if (snowSSS)
            {
               features.Add (GetFeatureName (SnowDefineFeature._SNOWSSS));
            }
#if __MICROSPLAT_TEXTURECLUSTERS__
            if (snowStochastic)
            {
               features.Add(GetFeatureName(SnowDefineFeature._SNOWSTOCHASTIC));
            }
#endif

         }
         return features.ToArray();
      }

      public override void Unpack(string[] keywords)
      {
         snow = HasFeature(keywords, SnowDefineFeature._SNOW) ? SnowMode.Rich : SnowMode.None;
         if (HasFeature(keywords, SnowDefineFeature._SNOWSIMPLE))
            snow = SnowMode.Simple;
         if (snow != SnowMode.None)
         {
            snowNormalNoise = HasFeature(keywords, SnowDefineFeature._SNOWNORMALNOISE);
            snowDistanceResample = HasFeature(keywords, SnowDefineFeature._SNOWDISTANCERESAMPLE);
            perTexSnow = HasFeature(keywords, SnowDefineFeature._PERTEXSNOWSTRENGTH) && snow == SnowMode.Rich;
            globalLevel = HasFeature(keywords, SnowDefineFeature._USEGLOBALSNOWLEVEL);
            globalHeight = HasFeature(keywords, SnowDefineFeature._USEGLOBALSNOWHEIGHT);
            snowSSS = HasFeature (keywords, SnowDefineFeature._SNOWSSS);
#if __MICROSPLAT_TEXTURECLUSTERS__
            snowStochastic = HasFeature(keywords, SnowDefineFeature._SNOWSTOCHASTIC);
#endif
         }
      }

      public override void InitCompiler(string[] paths)
      {
         for (int i = 0; i < paths.Length; ++i)
         {
            string p = paths[i];
            if (p.EndsWith("microsplat_properties_snow.txt"))
            {
               properties_snow = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_properties_snow_normal_noise.txt"))
            {
               properties_snow_normalNoise = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_properties_snow_distance_resample.txt"))
            {
               properties_snow_distanceResample = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_func_snow.txt"))
            {
               func_snow = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_cbuffer_snow.txt"))
            {
               cbuffer_snow = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
         }
      }

      public override void WriteProperties(string[] features, System.Text.StringBuilder sb)
      {
         if (snow != SnowMode.None)
         {
            sb.Append(properties_snow.text);
            if (HasFeature(features, SnowDefineFeature._SNOWNORMALNOISE))
            {
               sb.Append(properties_snow_normalNoise.text);
            }
            if (HasFeature(features, SnowDefineFeature._SNOWDISTANCERESAMPLE))
            {
               sb.Append(properties_snow_distanceResample.text);
            }

#if __MICROSPLAT_TEXTURECLUSTERS__
            if (snowStochastic)
            {
               sb.AppendLine("      _SnowStochasticScale(\"Stochastic Scale\", Range(0.5, 1.5)) = 1");
               sb.AppendLine("      _SnowStochasticContrast(\"Stochastic Contrast\", Range(0.01, 0.99)) = 0.3");
            }
#endif
            if (snowSSS)
            {
               sb.AppendLine("      _SnowSSSTint(\"Subsurface Tint\", Color) = (1,1,1,0.5)");
            }
            if (HasFeature(features, "_TESSDISTANCE"))
            {
               sb.AppendLine ("      _TessDisplaceSnowMultiplier(\"Snow Displacement Multiplier\", Range(0, 2)) = 1");
            }
         }
      }

      public override void WritePerMaterialCBuffer (string[] features, System.Text.StringBuilder sb)
      {
         if (snow != SnowMode.None)
         {
            sb.AppendLine(cbuffer_snow.text);
         }
      }

      public override void WriteFunctions(System.Text.StringBuilder sb)
      {
         if (snow != SnowMode.None)
         {
            sb.AppendLine(func_snow.text);
         }
      }

      public override void ComputeSampleCounts(string[] features, ref int arraySampleCount, ref int textureSampleCount, ref int maxSamples, ref int tessellationSamples, ref int depTexReadLevel)
      {
         if (snow != SnowMode.None)
         {
            textureSampleCount += 2;
            if (snowDistanceResample)
            {
               textureSampleCount += 2;
            }
            if (snowNormalNoise)
            {
               textureSampleCount += 1;
            }
#if __MICROSPLAT_TEXTURECLUSTERS__
            if (snowStochastic)
            {
               textureSampleCount += 4;
            }
#endif
         }
      }

      static GUIContent CPerTexSnow = new GUIContent("Snow Strength", "Maximum amount of snow for this surface");
      public override void DrawPerTextureGUI(int index, MicroSplatKeywords keywords, Material mat, MicroSplatPropData propData)
      {
         InitPropData(8, propData, new Color(1.0f, 0, 0, 0)); //snow strength
         if (snow == SnowMode.Rich)
         {
            perTexSnow = DrawPerTexFloatSlider(index, 8, GetFeatureName(SnowDefineFeature._PERTEXSNOWSTRENGTH),
               keywords, propData, Channel.R,  CPerTexSnow, 0, 1);
         }
      }
   }   
   #endif

}