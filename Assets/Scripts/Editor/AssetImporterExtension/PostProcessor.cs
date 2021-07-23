using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AssetImportTool
{
    /// <summary>
    /// 资产导入器
    /// </summary>
    public class PostProcessor : AssetPostprocessor
	{
		/// <summary>
		/// 导入类型以及对应类型实例字典
		/// </summary>
		private static Dictionary<System.Type, IAssetImporterExtension> m_ImporterCache = new Dictionary<System.Type, IAssetImporterExtension>();

		/// <summary>
		/// 应用
		/// </summary>
		private void Apply(AssetImporter assetImporter, string assetPath)
		{
			var settings = SettingsIO.NestedLoad (assetPath);
			if (settings == null) {
				return;
			}

			for (int i = 0; i < settings.settings.Count; i++) {
				var setting = settings.settings [i];

				var importer = GetImporterInstance (setting.Type);

				if (importer == null) {
					Debug.LogError (string.Format("资源路径({0}) -- > 类型({0})并未实现IAssetImporterExtension接口", assetPath, setting.Type));
					continue;
				}

				if (CanExecute(assetImporter, importer)) {
					var properties = setting.properties.Where (o => o.isEnabled).ToArray ();
					importer.Apply (assetImporter, assetPath, properties);
				}
			}
		}

		/// <summary>
		/// 导入完成后处理
		/// </summary>
		private void OnPostprocessImpl(AssetImporter assetImporter, string assetPath)
		{
			var settings = SettingsIO.NestedLoad (assetPath);
			if (settings == null) {
				return;
			}

			for (int i = 0; i < settings.settings.Count; i++) {
				var setting = settings.settings [i];

				var importer = GetImporterInstance (setting.Type);

				if (importer == null) {
					Debug.LogError ("定义了没有继承IAssetImporter Extension的Importer");
					continue;
				}

				if (CanExecute(assetImporter, importer)) {
					var properties = setting.properties.Where (o => o.isEnabled).ToArray ();
					importer.OnPostprocess (assetPath, properties);
				}
			}
		}

		/// <summary>
		/// 所有资源导入完成 后处理
		/// </summary>
		private static void OnPostprocessAllAssetsImpl(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			for (int n = 0; n < importedAssets.Length; n++) {
				var assetPath = importedAssets [n];

				var settings = SettingsIO.NestedLoad (assetPath);
				if (settings == null) {
					return;
				}

				for (int i = 0; i < settings.settings.Count; i++) {
					var setting = settings.settings [i];

					var importer = GetImporterInstance (setting.Type);

					if (importer == null) {
						Debug.LogError ("定义了没有继承IAssetImporter Extension的Importer");
						continue;
					}

					var properties = setting.properties.Where (o => o.isEnabled).ToArray ();
					importer.OnPostprocess (assetPath, properties);
				}
			}

			for (int n = 0; n < deletedAssets.Length; n++) {
				var assetPath = deletedAssets [n];

				var settings = SettingsIO.NestedLoad (assetPath);
				if (settings == null) {
					return;
				}

				for (int i = 0; i < settings.settings.Count; i++) {
					var setting = settings.settings [i];

					var importer = GetImporterInstance (setting.Type);

					if (importer == null) {
						Debug.LogError ("定义了没有继承IAssetImporter Extension的Importer");
						continue;
					}

					var properties = setting.properties.Where (o => o.isEnabled).ToArray ();
					importer.OnRemoveprocess (assetPath, properties);
				}
			}
		}

		/// <summary>
		/// 获得导入器实例
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IAssetImporterExtension GetImporterInstance(System.Type type)
		{
			IAssetImporterExtension ret;
			if (m_ImporterCache.TryGetValue (type, out ret)) {
				return ret;
			}

			var importer = System.Activator.CreateInstance(type);
			ret = importer as IAssetImporterExtension;
			if (ret != null) {
				m_ImporterCache.Add (type, ret);
			}

			return ret;
		}

		/// <summary>
		/// 是否可以执行
		/// </summary>
		/// <param name="assetImporte"></param>
		/// <param name="importer"></param>
		/// <returns></returns>
		private bool CanExecute(AssetImporter assetImporte, IAssetImporterExtension importer)
		{
			var targetImporterType = importer.GetTargetImporterType ();
			if (targetImporterType == null) {
				return true;
			}

			return assetImporter.GetType () == targetImporterType;
		}

		/// <summary>
		/// 预处理Texture
		/// </summary>
		private void OnPreprocessTexture()
		{
			Apply (assetImporter, assetPath);
		}

		/// <summary>
		/// 预处理Animation
		/// </summary>
		private void OnPreprocessAnimation()
		{
			Apply (assetImporter, assetPath);
		}

		private void OnPreprocessAudio()
		{
			Apply (assetImporter, assetPath);
		}

		/// <summary>
		/// 预处理Model
		/// </summary>
		private void OnPreprocessModel()
		{
			Apply (assetImporter, assetPath);
		}

		/// <summary>
		/// 预处理SpeedTree
		/// </summary>
		private void OnPreprocessSpeedTree()
		{
			Apply (assetImporter, assetPath);
		}

		/// <summary>
		/// 预处理Asset
		/// </summary>
		private void OnPreprocessAsset()
		{
			Apply (assetImporter, assetPath);
		}

		/// <summary>
		/// 所有资源后处理完成
		/// </summary>
		/// <param name="importedAssets"></param>
		/// <param name="deletedAssets"></param>
		/// <param name="movedAssets"></param>
		/// <param name="movedFromAssetPaths"></param>
		private static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			OnPostprocessAllAssetsImpl (importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
		}

		/// <summary>
		/// 后处理模型
		/// </summary>
		/// <param name="g"></param>
		void OnPostprocessModel(GameObject g)
		{
			// for skeleton animations.
			List<AnimationClip> animationClipList = new List<AnimationClip>(AnimationUtility.GetAnimationClips(g));
			if (animationClipList.Count == 0)
			{
				AnimationClip[] objectList = UnityEngine.Object.FindObjectsOfType(typeof(AnimationClip)) as AnimationClip[];
				animationClipList.AddRange(objectList);
			}

			foreach (AnimationClip theAnimation in animationClipList)
			{
				try
				{
					//去除scale曲线
					/*foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(theAnimation))
					{
						string name = theCurveBinding.propertyName.ToLower();
						if (name.Contains("scale"))
						{
							AnimationUtility.SetEditorCurve(theAnimation, theCurveBinding, null);
						}
					}*/

					//浮点数精度压缩到f3
					AnimationClipCurveData[] curves = null;
					curves = AnimationUtility.GetAllCurves(theAnimation);
					Keyframe key;
					Keyframe[] keyFrames;
					for (int ii = 0; ii < curves.Length; ++ii)
					{
						AnimationClipCurveData curveDate = curves[ii];
						if (curveDate.curve == null || curveDate.curve.keys == null)
						{
							//Debug.LogWarning(string.Format("AnimationClipCurveData {0} don't have curve; Animation name {1} ", curveDate, animationPath));
							continue;
						}
						keyFrames = curveDate.curve.keys;
						for (int i = 0; i < keyFrames.Length; i++)
						{
							key = keyFrames[i];
							key.value = float.Parse(key.value.ToString("f3"));
							key.inTangent = float.Parse(key.inTangent.ToString("f3"));
							key.outTangent = float.Parse(key.outTangent.ToString("f3"));
							keyFrames[i] = key;
						}
						curveDate.curve.keys = keyFrames;
						theAnimation.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
					}
				}
				catch (System.Exception e)
				{
					Debug.LogError(string.Format("CompressAnimationClip Failed !!! animationPath : {0} error: {1}", assetPath, e));
				}
			}

			try
			{
				MeshFilter[] sceneMeshs = g.GetComponentsInChildren<MeshFilter>();

				List<Color> c = null;
				List<Color32> c32 = null;
				for (int i = 0; i < sceneMeshs.Length; i++)
				{
					sceneMeshs[i].sharedMesh.SetColors(c);
					sceneMeshs[i].sharedMesh.SetColors(c32);
					sceneMeshs[i].sharedMesh.colors = null;
					sceneMeshs[i].sharedMesh.colors32 = null;
				}
            }
            catch (System.Exception e)
            {
				Debug.LogError(string.Format("清除顶点颜色失败!!! model path : {0} error: {1}", assetPath, e));
			}
		}
	}
}