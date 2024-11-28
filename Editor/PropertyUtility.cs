#if UNITY_EDITOR
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace STNC.UnityUtilities.Serialization
{
    public static class PropertyUtility
    {
        public static DropdownField CreateDropdown(SerializedProperty property)
        {
            string propertyDisplayName = GetPropertyDisplayName(property);
            var dropdown = new DropdownField(propertyDisplayName);

            var concreteTypes = TypeUtility.GetConcreteTypes(property.managedReferenceFieldTypename);

            dropdown.choices = concreteTypes.Select(type => type.Name).ToList();

            // Set the initial value of the dropdown
            string currentType = TypeUtility.GetCurrentTypeName(property);
            dropdown.value = currentType ?? dropdown.choices.FirstOrDefault();

            // Handle value change
            dropdown.RegisterValueChangedCallback(evt =>
            {
                var selectedType = concreteTypes.FirstOrDefault(type => type.Name == evt.newValue);
                if (selectedType != null)
                {
                    property.managedReferenceValue = TypeUtility.CreateInstanceWithFallback(selectedType);
                    property.serializedObject.ApplyModifiedProperties();
                }
                property.serializedObject.Update();
            });

            return dropdown;
        }

        public static void RenderSubProperties(SerializedProperty property, VisualElement container)
        {
            var subContainer = container.Q<VisualElement>("subPropertiesContainer") ?? CreateSubContainer(container);

            subContainer.Clear();

            if (property.managedReferenceValue != null)
            {
                RenderChildProperties(property, subContainer);
            }
        }

        private static VisualElement CreateSubContainer(VisualElement container)
        {
            var subContainer = new VisualElement { name = "subPropertiesContainer" };
            container.Add(subContainer);
            return subContainer;
        }

        private static void RenderChildProperties(SerializedProperty property, VisualElement subContainer)
        {
            var iterator = property.Copy();
            var depth = iterator.depth;

            if (iterator.Next(true))
            {
                do
                {
                    if (iterator.depth <= depth) break;

                    var field = new PropertyField(iterator);
                    field.Bind(property.serializedObject);
                    subContainer.Add(field);
                } while (iterator.Next(false));
            }
        }

        private static string GetPropertyDisplayName(SerializedProperty property)
        {
            if (!IsPropertyList(property)) return property.displayName;

            int index = GetIndexFromPropertyPath(property.propertyPath);
            return $"[{index}]";
        }

        private static bool IsPropertyList(SerializedProperty property)
        {
            return property.name == "data";
        }

        private static int GetIndexFromPropertyPath(string propertyPath)
        {
            const string PATTERN = @"\[(\d+)\]";
            var match = Regex.Match(propertyPath, PATTERN);
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }
    }
}
#endif

