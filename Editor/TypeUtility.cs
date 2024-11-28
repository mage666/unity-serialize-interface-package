#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace STNC.UnityUtilities.Serialization
{
    internal static class TypeUtility
    {
        public static List<Type> GetConcreteTypes(string fieldTypeName)
        {
            if (string.IsNullOrEmpty(fieldTypeName)) return new List<Type>();

            var baseType = TypeUtility.FindBaseType(fieldTypeName);
            return baseType == null ? new List<Type>() : TypeUtility.GetAssignableConcreteTypes(baseType);
        }

        public static string GetCurrentTypeName(SerializedProperty property)
        {
            return property.managedReferenceFullTypename?
                           .Split(',')
                           .FirstOrDefault()?
                           .Split('.')
                           .Last();
        }

        public static object CreateInstanceWithFallback(Type type)
        {
            // Try parameterless constructor first
            var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);
            if (parameterlessConstructor != null)
            {
                return Activator.CreateInstance(type);
            }

            // Try parameterized constructors
            foreach (var constructor in type.GetConstructors())
            {
                var args = constructor.GetParameters()
                                      .Select(param => TypeUtility.GetDefaultValueForType(param.ParameterType))
                                      .ToArray();

                try
                {
                    return constructor.Invoke(args);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create instance of {type.Name} with parameters: {ex.Message}");
                }
            }

            Debug.LogError($"Unable to instantiate type: {type.Name}. No suitable constructor found.");
            return null;
        }

        private static List<Type> GetAssignableConcreteTypes(Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly =>
                                        {
                                            try
                                            {
                                                return assembly.GetTypes();
                                            }
                                            catch (ReflectionTypeLoadException ex)
                                            {
                                                return ex.Types.Where(t => t != null);
                                            }
                                        })
                            .Where(type => baseType.IsAssignableFrom(type) &&
                                           !type.IsAbstract &&
                                           !type.IsInterface)
                            .ToList();
        }

        private static Type FindBaseType(string fieldTypeName)
        {
            var typeParts = fieldTypeName.Split(' ');
            if (typeParts.Length < 2) return null;

            var namespaceAndType = typeParts[1];
            return TypeUtility.FindTypeByName(namespaceAndType);
        }

        private static Type FindTypeByName(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .Select(assembly =>
                                    {
                                        try
                                        {
                                            return assembly.GetType(typeName);
                                        }
                                        catch
                                        {
                                            return null;
                                        }
                                    })
                            .FirstOrDefault(type => type != null);
        }

        private static object GetDefaultValueForType(Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            return type == typeof(string) ? string.Empty :              
                       null;                                            
        }
    }
}
#endif