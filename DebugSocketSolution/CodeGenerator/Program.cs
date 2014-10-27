using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using UnityEngine;
using FileStream = System.IO.FileStream;

namespace CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var unit = new CodeCompileUnit();

            var codeNamespace = new CodeNamespace("SocketServer");

            var codeType = new CodeTypeDeclaration("EventSubscriber")
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Static
            };

            codeNamespace.Types.Add(codeType);

            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));

            unit.Namespaces.Add(codeNamespace);

            var methodAdd = new CodeMemberMethod { Name = "Subscribe" };
            
            var methodRemove = new CodeMemberMethod { Name = "UnSubscribe" };

            var prm = new CodeParameterDeclarationExpression(typeof(string), "methodName");

            methodAdd.Parameters.Add(prm);
            methodRemove.Parameters.Add(prm);
            
            methodAdd.Attributes = MemberAttributes.Final | MemberAttributes.Public | MemberAttributes.Static;
            methodRemove.Attributes = MemberAttributes.Final | MemberAttributes.Public | MemberAttributes.Static;

            codeType.Members.Add(methodAdd);
            codeType.Members.Add(methodRemove);

            //statements

            var fields =
                typeof(GameEvents).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public |
                                              BindingFlags.Static);

            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.IsPrimitive) continue;

                var condition =
                 new CodeConditionStatement(new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(prm.Name), "Equals",
                     new CodePrimitiveExpression(fieldInfo.Name)),
                     new CodeExpressionStatement(
                         new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(GameEvents)), fieldInfo.Name), "Add", new CodeMethodReferenceExpression(null, fieldInfo.Name)))

                     );

                methodAdd.Statements.Add(condition);
                
                condition =
                 new CodeConditionStatement(new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(prm.Name), "Equals",
                     new CodePrimitiveExpression(fieldInfo.Name)),
                     new CodeExpressionStatement(
                         new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(GameEvents)), fieldInfo.Name), "Remove", new CodeMethodReferenceExpression(null, fieldInfo.Name)))

                     );

                methodRemove.Statements.Add(condition);


                var executedMethod = new CodeMemberMethod {Name = fieldInfo.Name, Attributes = MemberAttributes.Static};

                codeType.Members.Add(executedMethod);

                var ga = fieldInfo.FieldType.GetGenericArguments();

                int x = 1;

                foreach (var type in ga)
                {
                    executedMethod.Parameters.Add(new CodeParameterDeclarationExpression(type, ga.Count() <= 1 ? "data" : string.Format("data{0}", x)));
                    x ++;
                }

                var executedStatemnt =
                    new CodeExpressionStatement(
                        new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Log",
                            new CodePrimitiveExpression(string.Format("Event {0} was invoked!!!", executedMethod.Name))));
                
                executedMethod.Statements.Add(executedStatemnt);
            }

            var provider = new CSharpCodeProvider();

            using (var fs = new StreamWriter(@"e:\EventSubscriber.cs"))
            {
                provider.GenerateCodeFromCompileUnit(unit, new IndentedTextWriter(fs), new CodeGeneratorOptions());
            }

        }
    }
}
