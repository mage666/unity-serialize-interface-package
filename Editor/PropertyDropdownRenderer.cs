#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

namespace STNC.UnityUtilities.Serialization
{
    public class PropertyDropdownRenderer
    {
        private readonly SerializedProperty _property;
        private readonly VisualElement      _container;

        public PropertyDropdownRenderer(SerializedProperty property, VisualElement container)
        {
            _property  = property;
            _container = container;
        }

        public void Render()
        {
            var dropdown = PropertyUtility.CreateDropdown(_property);
            _container.Add(dropdown);

            PropertyUtility.RenderSubProperties(_property, _container);
        }
    }

}
#endif
