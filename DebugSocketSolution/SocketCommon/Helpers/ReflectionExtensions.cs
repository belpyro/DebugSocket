using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SocketCommon.Wrappers.Tree;

namespace SocketCommon.Helpers
{
    public static class ReflectionExtensions
    {
        public static MemberInfoWrapper ConvertToWrapper(this FieldInfo info, bool isSimple = true)
        {
            return new MemberInfoWrapper()
            {
                Name = info.Name,
                ItemType =
                    (info.FieldType.IsClass || info.FieldType.IsValueType) && !isSimple
                        ? info.FieldType.GetInterfaces().Contains(typeof (IEnumerable))
                            ? MemberType.Collection
                            : MemberType.Type
                        : MemberType.Field,
                TypeName = info.FieldType.Name,
                IsStatic = info.IsStatic
            };
        }
        
        public static MemberInfoWrapper ConvertToWrapper(this PropertyInfo info, bool isSimple = true)
        {
            return new MemberInfoWrapper()
            {
                Name = info.Name,
                TypeName = info.PropertyType.Name,
                ItemType =
                    (info.PropertyType.IsClass || info.PropertyType.IsValueType) && !isSimple
                        ? info.PropertyType.GetInterfaces().Contains(typeof (IEnumerable))
                            ? MemberType.Collection
                            : MemberType.Type
                        : MemberType.Property,
                IsStatic = info.GetGetMethod().IsStatic
            };
        }


        public static MethodInfoWrapper ConvertToWrapper(this MethodInfo info)
        {
            return new MethodInfoWrapper()
            {
                Name = info.Name,
                IsStatic = info.IsStatic,
                Parameters = info.GetParameters().Any() ? info.GetParameters().Select(x => new MemberInfoWrapper()
                {
                    Name = x.Name,
                    TypeName = x.ParameterType.Name
                }).ToList() : null
            };
        }
    }
}
