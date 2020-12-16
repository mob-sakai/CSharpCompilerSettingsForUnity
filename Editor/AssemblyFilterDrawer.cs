using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    [CustomPropertyDrawer(typeof(AssemblyFilter))]
    internal class AssemblyFilterDrawer : PropertyDrawer
    {
        private bool _isInitialized;
        private SerializedProperty _includedAssemblies;
        private SerializedProperty _predefinedAssemblies;
        private ReorderableList _roIncludedAssemblies;

        private void Initialize(SerializedProperty property)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _predefinedAssemblies = property.FindPropertyRelative("m_PredefinedAssemblies");
            _includedAssemblies = property.FindPropertyRelative("m_IncludedAssemblies");

            _roIncludedAssemblies = new ReorderableList(property.serializedObject, _includedAssemblies, true, true, true, true);
            _roIncludedAssemblies.drawHeaderCallback = rect =>
            {
                EditorGUI.PrefixLabel(rect, new GUIContent(_includedAssemblies.displayName));

                rect.x += rect.width - 110;
                rect.width = 110;
                EditorGUI.LabelField(rect, "* Prefix '!' to exclude.", EditorStyles.miniLabel);
            };
            _roIncludedAssemblies.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            _roIncludedAssemblies.drawElementCallback = (rect, index, active, focused) =>
            {
                var sp = _includedAssemblies.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, sp, GUIContent.none);
            };
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            return _includedAssemblies.arraySize * (EditorGUIUtility.singleLineHeight + 2) + 47;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var p = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            _roIncludedAssemblies.DoList(new Rect(p.x - 3, p.y, p.width + 6, p.height));

            EditorGUI.PropertyField(new Rect(p.x, p.y + p.height - 16, p.width, 16), _predefinedAssemblies);

            EditorGUI.EndProperty();
        }
    }
}
