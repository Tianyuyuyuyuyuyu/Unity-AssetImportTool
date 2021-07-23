using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AssetImportTool
{
	public interface IAssetImporterExtension
	{
		/// <summary>
		/// 返回该导入处理器处理的资源类型
		/// 返回null时可以处理所有的导入类型（做一些通用处理）
		/// </summary>
		System.Type GetTargetImporterType ();
		/// <summary>
		/// AssetImporter 应用该处理器
		/// </summary>
		void Apply (AssetImporter importer, string assetPath, Property[] properties);
		/// <summary>
		/// 资源导入完成后调用
		/// </summary>
		void OnPostprocess (string assetPath, Property[] properties);
		/// <summary>
		/// 资源删除完成后调用
		/// </summary>
		void OnRemoveprocess(string assetPath, Property[] properties);
	}
}