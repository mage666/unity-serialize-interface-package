using System;
using UnityEngine;

namespace STNC.UnityUtilities.Serialization
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = false)]
    public class SerializeInterfaceAttribute : PropertyAttribute
    {
    }
}