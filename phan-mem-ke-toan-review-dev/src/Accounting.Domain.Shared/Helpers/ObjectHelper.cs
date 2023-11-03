using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Accounting.Helpers
{
    public static class ObjectHelper
    {
        public static T CreateInstance<T>() where T : class
        {
            return Activator.CreateInstance<T>();
        }
        public static object CreateInstance(string typeName)
        {
            Type type = FindType(typeName);
            return Activator.CreateInstance(type);
        }
        public static PropertyInfo[] GetPublicPropertyNames<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public);            
        }
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            var instanceType = instance.GetType();      
            var pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new Exception($"No property '{propertyName}' found on the instance.");
            if (!pi.CanWrite)
                throw new Exception($"The property '{propertyName}' on the instance does not have a setter.");
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, Array.Empty<object>());
        }
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value == null)
                return null;

            var sourceType = value.GetType(); 

            var destinationConverter = TypeDescriptor.GetConverter(destinationType);
            if (destinationConverter.CanConvertFrom(value.GetType()))
                return destinationConverter.ConvertFrom(null, culture, value);

            var sourceConverter = TypeDescriptor.GetConverter(sourceType);
            if (sourceConverter.CanConvertTo(destinationType))                      
                return sourceConverter.ConvertTo(null, culture, value, destinationType);

            if (destinationType.IsEnum && value is int)
                return Enum.ToObject(destinationType, (int)value);

            /*if(value is double) { return Convert.ToDecimal(value); }*/

            if (!destinationType.IsInstanceOfType(value))
                return Convert.ChangeType(value, destinationType, culture);  

            return value;
        }
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }
        private static Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
        private static Type FindType(string typeName)
        {
            Assembly[] assemblies = GetAssemblies();
            return assemblies.Where(p => p.GetType().FullName.Contains(typeName))
                    .Select(p => p.GetType()).FirstOrDefault();
        }
        public static string NewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
