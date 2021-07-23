using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Reflection;

namespace AssetImportTool
{
	/// <summary>
	/// 用于为每个文件夹设置资产导入器设置的 UI
	/// </summary>
	public class InspectorEditor : IDrawDirectoryInspector
	{
		private static string HeaderMessage =
			"您可以为每个文件夹设置资产导入" + System.Environment.NewLine +
			System.Environment.NewLine +
			"红色设置是从父文件夹继承的设置" + System.Environment.NewLine +
			System.Environment.NewLine +
			"如果不需要，可以禁用它。";

		/// <summary>
		/// 设定
		/// </summary>
		private Settings m_Settings;
		/// <summary>
		/// 父文件夹设定
		/// </summary>
		private Settings m_ParentSettings;
		/// <summary>
		/// 目录路径
		/// </summary>
		private string m_DirectoryPath;
		/// <summary>
		/// 选中的导入器 类型索引
		/// </summary>
		private int m_SelectImporterIndex;
		/// <summary>
		/// 滚动位置
		/// </summary>
		private Vector2 m_ScrollPosition;

		#region IDrawDirectoryInspector
		/// <summary>
		/// OnEnable时调用
		/// </summary>
		public void OnEnable()
		{
			m_DirectoryPath = Utility.GetSelectedFolderPath ();
			m_Settings = LoadSettings (m_DirectoryPath);
			m_ParentSettings = LoadParentSettings (m_DirectoryPath);
		}
		/// <summary>
		/// OnInspectorGUI时调用
		/// </summary>
		public void OnInspectorGUI(Object target)
		{
			if (string.IsNullOrEmpty (m_DirectoryPath)) {
				EditorGUILayout.HelpBox ("选择文件夹", MessageType.Warning, true);
				return;
			}

			if (m_Settings == null) {
				m_Settings = new Settings ();
			}
				
			OnDrawHeaderMenu ();
			OnDrawAddSettingMenu ();

			m_ScrollPosition  = EditorGUILayout.BeginScrollView (m_ScrollPosition);

			List<Setting> deleteSetting = new List<Setting> ();
			ButtonParam button = new ButtonParam (){
				name = "删除"
			};
			for (int i = 0; i < m_Settings.settings.Count; i++) {
				var setting = m_Settings.settings [i];
				Setting parentSetting = null;
				if (m_ParentSettings != null) {
					parentSetting = m_ParentSettings.GetSetting (setting.Type);
				}

				button.onClick = () => {
					deleteSetting.Add(setting);
				};
				OnDrawSetting (setting, parentSetting, button);
				EditorGUILayout.Space ();
			}

			if (m_ParentSettings != null) {
				for (int i = 0; i < m_ParentSettings.settings.Count; i++) {
					var setting = m_ParentSettings.settings [i];
					if (m_Settings.Has (setting.Type)) {
						continue;
					}
					OnDrawSetting (null, setting);
					EditorGUILayout.Space ();
				}
			}

			for (int i = 0; i < deleteSetting.Count; i++) {
				m_Settings.settings.Remove (deleteSetting[i]);
			}

			EditorGUILayout.EndScrollView ();

		}
#endregion // IDrawDirectoryInspector

		private void OnDrawHeaderMenu()
		{
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.HelpBox (HeaderMessage, MessageType.Info, true);

			EditorGUILayout.BeginHorizontal ();
			{
				GUI.enabled = SettingsIO.Exist (m_DirectoryPath);
				if (GUILayout.Button ("删除设置")) {
					SettingsIO.Remove (m_DirectoryPath);
					m_Settings = null;
					EditorGUIUtility.ExitGUI ();
				}
				GUI.enabled = true;

				GUI.enabled = m_Settings != null;
				if (GUILayout.Button ("保存设置")) {
					SettingsIO.Save (m_DirectoryPath, m_Settings);
				}
				GUI.enabled = true;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}


		private void OnDrawSetting(Setting setting, Setting parentSetting, ButtonParam button = null)
		{
			if (setting == null && parentSetting == null) {
				return;
			}
			
			var type = (setting != null ? setting.Type : parentSetting.Type);
			var fieldList = type.GetFields (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty);
			var nameList = fieldList.Select (o => o.Name).ToArray ();

			EditorGUILayout.BeginVertical (GUI.skin.box);

			if (setting != null) {
				EditorGUILayout.BeginHorizontal ();
				setting.isEnabled = EditorGUILayout.ToggleLeft (setting.Type.Name, setting.isEnabled);
				if (button != null) {
					if (GUILayout.Button (button.name)) {
						if (button.onClick != null) {
							button.onClick ();
						}
					}
				}
				EditorGUILayout.EndHorizontal();
				if (!setting.isEnabled) {
					string msg = "禁用父文件夹设置" + System.Environment.NewLine;
					msg += "如果想继续沿用父文件夹设置、请按删除按钮";
					EditorGUILayout.HelpBox (msg, MessageType.None, true);
					EditorGUILayout.EndVertical ();
					return;
				}

				List<Property> deleteProperties = new List<Property> ();
				ButtonParam buttonParam = new ButtonParam () {
					name = "删除",
					backgroundColor = Color.red
				};
				for (int i = 0; i < setting.properties.Count; i++) {
					var property = setting.properties [i];

					buttonParam.onClick = () => {
						deleteProperties.Add (property);
					};

					OnDrawProperty (property, fieldList, nameList, buttonParam);
				}

				for (int i = 0; i < deleteProperties.Count; i++) {
					setting.properties.Remove (deleteProperties [i]);
				}

				if (GUILayout.Button ("添加设定")) {
					setting.properties.Add (new Property ());
				}
			}

			if (parentSetting != null) {
				var targetProperties = parentSetting.properties
					.Where (
						o =>{
							if (setting != null) {
								if (setting.HasProperty (o.name)) {
									return false;
								}
							}

							if(!o.isEnabled){
								return false;
							}

							return true;
						})
					.ToArray();

				if (targetProperties.Length <= 0) {
					EditorGUILayout.EndVertical ();
					return;
				}

				GUI.enabled = false;
				var prevColor = GUI.backgroundColor;
				GUI.backgroundColor = Color.red;

				if (setting == null) {
					parentSetting.isEnabled = EditorGUILayout.ToggleLeft ("※" + parentSetting.Type.Name, parentSetting.isEnabled);
					EditorGUILayout.HelpBox ("父文件夹设置已应用", MessageType.None, true);
				}
					
				ButtonParam buttonParam = new ButtonParam () {
					name = "复制",
					backgroundColor = prevColor
				};
				for (int i = 0; i < targetProperties.Length; i++) {
					var property = targetProperties [i];

					if (setting != null) {
						if (setting.HasProperty (property.name)) {
							continue;
						}
					}

					if (!property.isEnabled) {
						continue;
					}

					buttonParam.onClick = () => {
						var currentSetting = m_Settings.GetSetting(parentSetting.Type);
						if(currentSetting == null){
							currentSetting = new Setting();
							currentSetting.Type = parentSetting.Type;
							m_Settings.settings.Add(currentSetting);
						}
						currentSetting.properties.Add(property.Copy());
					};

					OnDrawProperty (property, fieldList, nameList, buttonParam);
				}
				GUI.backgroundColor = prevColor;
				GUI.enabled = true;
			}

			EditorGUILayout.EndVertical ();
		}

		private void OnDrawProperty(Property property, FieldInfo[] fieldList, string[] propertyList, ButtonParam button)
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			EditorGUILayout.BeginHorizontal ();
			{
				var toggleLabel = property.isEnabled ? "有效" : "无效";
				property.isEnabled = EditorGUILayout.ToggleLeft (toggleLabel, property.isEnabled);

				if (button != null) {
					var prevEnabled = GUI.enabled;
					GUI.enabled = true;

					var prevColor = GUI.backgroundColor;
					GUI.backgroundColor = button.backgroundColor;
					if (GUILayout.Button (button.name)) {
						if (button.onClick != null) {
							button.onClick ();
						}
					}
					GUI.backgroundColor = prevColor;
					GUI.enabled = prevEnabled;
				}
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUI.BeginChangeCheck ();
			int index = System.Array.IndexOf (propertyList, property.name);
			index = EditorGUILayout.Popup ("设置", index, propertyList);
			if (EditorGUI.EndChangeCheck ()) {
				property.name = propertyList [index];
				property.value = "";
			}

			if (index < 0 || index >= fieldList.Length) {
				EditorGUILayout.EndVertical ();
				return;
			}

			var field = fieldList [index];
			var fieldType = field.FieldType;

			if (string.IsNullOrEmpty (property.value)) {
				if (fieldType == typeof(string)) {
					property.value = "";
				} else {
					property.value = System.Activator.CreateInstance (fieldType).ToString ();
				}
			}

			while (true) {

				if (fieldType == typeof(int)) {
					property.value = EditorGUILayout.IntField ("值", int.Parse (property.value)).ToString();
					break;
				}

				if (fieldType == typeof(float)) {
					property.value = EditorGUILayout.FloatField ("值", float.Parse(property.value)).ToString();
					break;
				}

				if (fieldType == typeof(bool)) {
					property.value = EditorGUILayout.Toggle ("值", bool.Parse(property.value)).ToString();
					break;
				}

				if (fieldType == typeof(string)) {
					property.value = EditorGUILayout.TextField ("值", property.value).ToString();
					break;
				}

				if (fieldType.IsEnum) {
					property.value = EditorGUILayout.EnumPopup ("值", System.Enum.Parse (fieldType, property.value) as System.Enum).ToString ();
					break;
				}

				var prevColor = GUI.color;
				GUI.color = Color.red;
				EditorGUILayout.LabelField ("无法设置值，因为类型未定义");
				GUI.color = prevColor;

				break;
			}

			EditorGUILayout.EndVertical ();
		}

		private void OnDrawAddSettingMenu()
		{
			var importerTypeList = Utility.GetInterfaces<IAssetImporterExtension> ();

			// 省去已添加的
			importerTypeList = importerTypeList.Where(o => !m_Settings.Has(o)).ToArray();

			var nameList = importerTypeList.Select (o => o.Name).ToArray ();

			EditorGUILayout.BeginHorizontal (GUI.skin.box);
			GUI.enabled = importerTypeList.Length > 0;

			m_SelectImporterIndex = EditorGUILayout.IntPopup ("添加导入器", m_SelectImporterIndex, nameList, null);

			if (GUILayout.Button ("追加")) {
				var setting = new Setting ();
				setting.Type = importerTypeList [m_SelectImporterIndex];
				m_Settings.settings.Add (setting);
				m_SelectImporterIndex = 0;
			}

			GUI.enabled = true;
			EditorGUILayout.EndHorizontal ();

		}

		private Settings LoadSettings(string selectPath)
		{
			return SettingsIO.Load (selectPath);
		}

		private Settings LoadParentSettings(string selectPath)
		{
			var directoryPath = "";

			if (Directory.Exists (selectPath)) {
				directoryPath = selectPath;
			} else {
				directoryPath = Path.GetDirectoryName (selectPath);
			}

			directoryPath = Path.GetDirectoryName (directoryPath);

			return SettingsIO.NestedLoad (directoryPath);
		}


		private class ButtonParam
		{
			public string name;
			public Color backgroundColor;
			public System.Action onClick;
		}
	}
}