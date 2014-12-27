using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CecilTest
{

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using UnityEngine;

    class Program
    {
        static void Main(string[] args)
        {
            var assembly = AssemblyDefinition.ReadAssembly(@"e:\KSP_Dev_25\KSP_Data\Managed\Assembly-CSharp.dll.old");

            var logMethod = typeof(Debug).GetMethod("Log", new[] { typeof(object) });

            var refrToLog = assembly.MainModule.Import(logMethod);

            //var attrCtor = assembly.MainModule.Import(typeof (SerializableAttribute).GetConstructor(Type.EmptyTypes));

            var kspMenuType =
                assembly.MainModule.Types.FirstOrDefault(
                    x => x.Name.Equals("EditorLogic", StringComparison.InvariantCultureIgnoreCase));

            //foreach (var typeDefinition in assembly.MainModule.Types)
            //{
            //    typeDefinition.CustomAttributes.Insert(0, new CustomAttribute(attrCtor));
            //}
            if (kspMenuType != null)
            {
                //var definition = kspMenuType.Methods.FirstOrDefault(x => x.Name.Equals("Start"));

                //if (definition != null)
                //{
                //    definition.IsPublic = true;
                //    definition.IsVirtual = true;
                //}
                foreach (var methodDefinition in kspMenuType.Methods.Where(x => !x.IsConstructor && !x.Name.Equals("Update") && !x.Name.Equals("OnGUI")
                    && !x.Name.Equals("UpdatePartMode") && !x.Name.Equals("mouseOverModalArea") && !x.Name.Equals("get_SelectedPart")))
                {
                    methodDefinition.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, string.Format("Called {0}",methodDefinition.Name)));
                    methodDefinition.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, refrToLog));
                }
            }

            assembly.Write(@"e:\KSP_Dev_25\KSP_Data\Managed\Assembly-CSharp.dll");

        }
    }
}
