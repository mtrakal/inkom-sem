using System;
using System.Runtime;
using System.Reflection;
using System.Reflection.Emit;

public class reflectionEmit
{
    public static void Main()
    {
        AppDomain ad = AppDomain.CurrentDomain;
        AssemblyName am = new AssemblyName();
        am.Name = "TestAsm";
        AssemblyBuilder ab = ad.DefineDynamicAssembly(am, AssemblyBuilderAccess.Save);
        ModuleBuilder mb = ab.DefineDynamicModule("testmod", "TestAsm.exe");
        TypeBuilder tb = mb.DefineType("mytype", TypeAttributes.Public);
        MethodBuilder metb = tb.DefineMethod("hi", MethodAttributes.Public |
        MethodAttributes.Static, null, null);
        ab.SetEntryPoint(metb);

        ILGenerator il = metb.GetILGenerator();
        il.EmitWriteLine("Hello World");
        il.Emit(OpCodes.Ret);
        tb.CreateType();
        ab.Save("TestAsm.exe");
    }
    public static int Pricti(int a)
    {
        return 1 + 1;
    }
}
