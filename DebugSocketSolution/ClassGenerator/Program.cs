using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KSP.IO;
using Microsoft.CSharp;
using FileAccess = System.IO.FileAccess;
using FileMode = System.IO.FileMode;
using FileStream = System.IO.FileStream;

namespace ClassGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Unity") || x.FullName.Contains("Assembly-CSharp"))
                .SelectMany(x => x.GetTypes())
                .Where(x => (x.IsClass || x.IsValueType) && !x.IsEnum && !x.IsPrimitive).Where(x => !x.FullName.Contains('+'))
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
                    new CodeAttributeDeclaration(new CodeTypeReference(typeof (SerializableAttribute))));

                nspace.Types.Add(wrapper);

                foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if ((field.FieldType.IsClass || field.FieldType.IsValueType) && !field.FieldType.IsPrimitive && field.FieldType != typeof(string))
                    {
                        var codeField = new CodeMemberField(string.Format("{0}_Wrapper", field.FieldType.Name), string.Format("_{0}_wrapped", field.Name))
                        {
                            Attributes = MemberAttributes.Public
                        };
                        wrapper.Members.Add(codeField);
                    }
                    else
                    {
                        var codeField = new CodeMemberField(field.FieldType, string.Format("_{0}_wrapped", field.Name))
                     {
                         Attributes = MemberAttributes.Public
                     };
                        wrapper.Members.Add(codeField);
                    }
                }

                foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if ((prop.PropertyType.IsClass || prop.PropertyType.IsValueType) && !prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string))
                    {
                        var field = new CodeMemberField(string.Format("{0}_Wrapper", prop.PropertyType.Name),
                            string.Format("_{0}", prop.Name.ToLower()))
                        {
                            Attributes = MemberAttributes.Private
                        };
                        wrapper.Members.Add(field);

                        var property = new CodeMemberProperty()
                        {
                            Name = prop.Name,
                            Type = field.Type,
                            Attributes = MemberAttributes.Public
                        };

                        property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(null, field.Name), new CodePropertySetValueReferenceExpression()));

                        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, field.Name)));
                    }
                    else
                    {
                        var codeField = new CodeMemberField(prop.PropertyType, string.Format("_{0}", prop.Name.ToLower()))
                        {
                            Attributes = MemberAttributes.Private
                        };

                        wrapper.Members.Add(codeField);

                        wrapper.Members.Add(CreateProperty(string.Format("_{0}", prop.Name.ToLower()), prop.Name, prop.PropertyType));
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
                Attributes = MemberAttributes.Public
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
