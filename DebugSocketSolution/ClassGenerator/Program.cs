using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using SocketCommon.Attributes;

namespace ClassGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("UnityEngine") || x.FullName.Contains("Assembly-CSharp"))
                .SelectMany(x => x.GetTypes())
                .Where(x => (x.IsClass || x.IsValueType) && !x.IsEnum && !x.IsPrimitive).Where(x => !x.FullName.Contains('+') && x.Name.Length > 3)
                .Where(x => !x.IsSerializable).Where(x => x.BaseType != typeof(Attribute)).OrderBy(x => x.FullName).ToList();

            //hack for loading assembly
            var m = typeof(FlightGlobals);

            foreach (var t in types)
            {
                CodeCompileUnit unit = new CodeCompileUnit();

                var nspace = new CodeNamespace("Wrappers");

                unit.Namespaces.Add(nspace);

                CodeTypeDeclaration wrapper = new CodeTypeDeclaration(string.Format("{0}_Wrapper", t.Name))
                {
                    IsClass = true,
                    Attributes = MemberAttributes.Public,
                };

                wrapper.CustomAttributes.Add(
                    new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));

                nspace.Types.Add(wrapper);

                foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    if ((field.FieldType.IsClass || field.FieldType.IsValueType) && !field.FieldType.IsPrimitive && field.FieldType != typeof(string)
                        && !field.FieldType.IsSerializable)
                    {
                        var codeField = new CodeMemberField(string.Format("{0}_Wrapper", field.FieldType.Name), field.Name)
                        {
                            Attributes = MemberAttributes.Public
                        };
                        wrapper.Members.Add(codeField);

                        if (field.FieldType == t)
                        {
                            wrapper.CustomAttributes.Add(
                                new CodeAttributeDeclaration(new CodeTypeReference(typeof(InstanceNameAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(field.Name))));
                        }
                    }
                    else
                    {
                        var codeField = new CodeMemberField(field.FieldType, field.Name)
                     {
                         Attributes = MemberAttributes.Public
                     };
                        wrapper.Members.Add(codeField);
                    }
                }

                foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    if ((prop.PropertyType.IsClass || prop.PropertyType.IsValueType) && !prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string) && !prop.PropertyType.IsSerializable)
                    {
                        var field = new CodeMemberField(string.Format("{0}_Wrapper", prop.PropertyType.Name),
                            prop.Name)
                        {
                            Attributes = MemberAttributes.Public
                        };
                        wrapper.Members.Add(field);

                        if (prop.PropertyType == t)
                        {
                            wrapper.CustomAttributes.Add(
                                new CodeAttributeDeclaration(new CodeTypeReference(typeof(InstanceNameAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(prop.Name))));
                        }
                    }
                    else
                    {
                        var codeField = new CodeMemberField(prop.PropertyType, prop.Name)
                        {
                            Attributes = MemberAttributes.Public
                        };

                        wrapper.Members.Add(codeField);
                    }

                }

                try
                {
                    using (var fStream = new StreamWriter(System.IO.File.Create(string.Format(@"e:\{0}.cs", wrapper.Name))))
                    {
                        var provider = new CSharpCodeProvider();
                        provider.GenerateCodeFromCompileUnit(unit, fStream, null);
                    }
                }
                catch (Exception)
                {
                    continue;
                }

            }


        }

        static CodeMemberProperty CreateProperty(string field, string name, Type type)
        {
            CodeMemberProperty property = new CodeMemberProperty()
            {
                Name = name,
                Type = new CodeTypeReference(type),
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };

            property.SetStatements.Add(
              new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, field),
                    new CodePropertySetValueReferenceExpression()));

            property.GetStatements.Add(
              new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(null, field)));

            return property;
        }
    }
}
