using System;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Threading;
using System.Diagnostics;

// declare the interface
public interface IFactorial
{
    int myfactorial();
}

public class SampleFactorialFromEmission
{
    // emit the assembly using op codes
    private Assembly EmitAssembly(int theValue)
    {
        // create assembly name
        AssemblyName assemblyName = new AssemblyName();
        assemblyName.Name = "FactorialAssembly";

        // create assembly with one module
        AssemblyBuilder newAssembly = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder newModule = newAssembly.DefineDynamicModule("MFactorial");

        // define a public class named "CFactorial" in the assembly
        TypeBuilder myType = newModule.DefineType("CFactorial", TypeAttributes.Public);

        // Mark the class as implementing IFactorial.
        myType.AddInterfaceImplementation(typeof(IFactorial));

        // define myfactorial method by passing an array that defines
        // the types of the parameters, the type of the return type,
        // the name of the method, and the method attributes.

        Type[] paramTypes = new Type[0];
        Type returnType = typeof(Int32);
        MethodBuilder simpleMethod = myType.DefineMethod("myfactorial", MethodAttributes.Public | MethodAttributes.Virtual, returnType, paramTypes);

        // obtain an ILGenerator to emit the IL
        ILGenerator generator = simpleMethod.GetILGenerator();

        // Ldc_I4 pushes a supplied value of type int32
        // onto the evaluation stack as an int32.
        // push 1 onto the evaluation stack.
        // foreach i less than theValue,
        // push i onto the stack as a constant
        // multiply the two values at the top of the stack.
        // The result multiplication is pushed onto the evaluation
        // stack.
        generator.Emit(OpCodes.Ldc_I4, 1);

        for (Int32 i = 1; i <= theValue; ++i)
        {
            generator.Emit(OpCodes.Ldc_I4, i);
            generator.Emit(OpCodes.Mul);
        }

        // emit the return value on the top of the evaluation stack.
        // Ret returns from method, possibly returning a value.

        generator.Emit(OpCodes.Ret);

        // encapsulate information about the method and
        // provide access to the method metadata
        MethodInfo factorialInfo = typeof(IFactorial).GetMethod("myfactorial");

        // specify the method implementation.
        // pass in the MethodBuilder that was returned
        // by calling DefineMethod and the methodInfo just created
        myType.DefineMethodOverride(simpleMethod, factorialInfo);

        // create the type and return new on-the-fly assembly
        myType.CreateType();
        return newAssembly;
    }

    // check if the interface is null, generate assembly
    // otherwise it is already there, where it is to be...
    public double DoFactorial(int theValue)
    {
        if (thesample == null)
        {
            GenerateCode(theValue);
        }

        // call the method through the interface
        return (thesample.myfactorial());
    }

    // emit the assembly, create an instance and
    // get the interface IFactorial
    public void GenerateCode(int theValue)
    {
        Assembly theAssembly = EmitAssembly(theValue);
        thesample = (IFactorial)theAssembly.CreateInstance("CFactorial");
    }

    // private member data
    IFactorial thesample = null;
}

class Faktorial
{
    [STAThread]
    static void Main(string[] args)
    {
        Int32 aValue = 5;
        SampleFactorialFromEmission t = new SampleFactorialFromEmission();
        double result = t.DoFactorial(aValue);
        Console.WriteLine("Factorial of " + aValue + " is " + result);
    }
}