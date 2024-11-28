#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace STNC.UnityUtilities.Serialization
{
    [CustomPropertyDrawer(typeof(SerializeInterfaceAttribute))]
    public class SerializeInterfaceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            const string UXML_PATH   = "Packages/com.stnc.serializeinterface/Ui/Layout.uxml";
            var          visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            if (visualTree == null)
            {
                Debug.LogError($"UXML file not found at {UXML_PATH}. Ensure the file exists.");
                return new VisualElement();
            }

            var root = visualTree.CloneTree();
            
            const string USS_PATH    = "Packages/com.stnc.serializeinterface/Ui/Layout.uss";
            var          styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }
            else
            {
                Debug.LogError($"USS file not found at {USS_PATH}. Ensure the file exists.");
            }


            var container = root.Q<VisualElement>("elementsContainer");
            if (container == null)
            {
                Debug.LogError("VisualElement 'elementsContainer' not found in UXML.");
                return root;
            }


            new PropertyDropdownRenderer(property, container).Render();

            return root;
        }
    }
}
#endif