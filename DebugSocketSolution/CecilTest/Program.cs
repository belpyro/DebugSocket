﻿using System;
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
            var assembly = AssemblyDefinition.ReadAssembly(@"e:\KSP_Dev\KSP_Data\Managed\UnityEngine.dll");

            //var logMethod = typeof(Debug).GetMethod("Log", new[] { typeof(object) });

            //var refrToLog = assembly.MainModule.Import(logMethod);

            var attrCtor = assembly.MainModule.Import(typeof (SerializableAttribute).GetConstructor(Type.EmptyTypes));

            //var kspMenuType =
            //    assembly.MainModule.Types.FirstOrDefault(
            //        x => x.Name.Equals("MainMenu", StringComparison.InvariantCultureIgnoreCase));

            foreach (var typeDefinition in assembly.MainModule.Types)
            {
                typeDefinition.CustomAttributes.Insert(0, new CustomAttribute(attrCtor));
            }
            //if (kspMenuType != null)
            //{
            //    foreach (var methodDefinition in kspMenuType.Methods)
            //    {
            //        methodDefinition.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, "Injected"));
            //        methodDefinition.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, refrToLog));
            //    }
            //}

            assembly.Write(@"e:\KSP_Dev\KSP_Data\Managed\UnityEngine.dll");

        }
    }
}