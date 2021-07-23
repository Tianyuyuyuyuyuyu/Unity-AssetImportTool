using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetImportTool
{
    /// <summary>
    /// DirectoryInspector绘制Inspector扩展接口
    /// </summary>
    public interface IDrawDirectoryInspector
    {
        /// <summary>
        /// OnEnable时调用
        /// </summary>
        void OnEnable();
        /// <summary>
        /// OnInspectorGUI时调用
        /// </summary>
        void OnInspectorGUI(Object target);
    }
}