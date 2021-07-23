using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace AssetImportTool
{
    public static class SettingsIO 
	{
		/// <summary>
		/// 装入包含父层设置的配置文件
		/// </summary>
		public static Settings NestedLoad(string path)
		{
			var directoryPath = path;

			if (Directory.Exists (path)) {
				// 指定了directory的路径的情况下，转换成文件路径
				directoryPath = Path.Combine(path, Settings.FileName);
			}

			var ignoreTypeList = new HashSet<System.Type> ();

			var result = new Settings ();
			while (true) {
				directoryPath = Path.GetDirectoryName (directoryPath);
				if (string.IsNullOrEmpty (directoryPath)) {
					break;
				}
				var current = Load (directoryPath);
				if (current != null) {
					var ignore = current.settings.Where (o => !o.isEnabled).Select(o => o.Type).ToArray ();
					for (int i = 0; i < ignore.Length; i++) {
						ignoreTypeList.Add (ignore[i]);
					}
					result.Merge (current, ignoreTypeList.ToArray());
				}
			}

			if (!result.HasAny ()) {
				return null;
			}
			return result;
		}

		/// <summary>
		/// 只装入指定路径以下的配置文件
		/// </summary>
		public static Settings Load(string path)
		{
			var filePath = CreateFilePath (path);
			if (!File.Exists (filePath)) {
				return null;
			}

			return LoadToSerialize (filePath);
		}

		/// <summary>
		/// 保存
		/// </summary>
		public static void Save(string path, Settings settings)
		{
			var filePath = CreateFilePath(path);
			DeserializeToSave (settings, filePath);
		}

		/// <summary>
		/// 删除配置文件
		/// </summary>
		public static void Remove(string path)
		{
			var filePath = CreateFilePath (path);
			if (!File.Exists (filePath)) {
				return;
			}
			File.Delete (filePath);
		}

		/// <summary>
		/// 检查配置文件是否存在
		/// </summary>
		public static bool Exist(string path)
		{
			var filePath = CreateFilePath (path);
			return File.Exists (filePath);
		}

		private static string CreateFilePath(string path)
		{
			var directoryPath = "";

			if (Directory.Exists (path)) {
				directoryPath = path;
			} else {
				directoryPath = Path.GetDirectoryName (path);
			}

			var filePath = Path.Combine (directoryPath, Settings.FileName);
			return filePath;
		}

		private static Settings LoadToSerialize(string path)
		{
			var serializer = new XmlSerializer(typeof(Settings));

			using (StreamReader sr = new StreamReader (path, System.Text.Encoding.UTF8)) {
				Settings settings = serializer.Deserialize (sr) as Settings;
				return settings;
			}
		}
		private static void DeserializeToSave(Settings settings, string path)
		{
			var serializer = new XmlSerializer(typeof(Settings));
			using (StreamWriter sw = new StreamWriter (path, false, System.Text.Encoding.UTF8)) {
				serializer.Serialize(sw, settings);
			}
		}
	}
}