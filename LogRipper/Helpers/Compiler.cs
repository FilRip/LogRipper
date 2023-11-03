using System.CodeDom.Compiler;
using System.Text;

using Microsoft.CSharp;

namespace LogRipper.Helpers
{
    internal static class Compiler
    {
        internal static CompilerResults Compile(string script)
        {
            CSharpCodeProvider compilator = new();
            CompilerParameters options = new()
            {
                GenerateInMemory = true,
                CompilerOptions = "/t:library",
                IncludeDebugInformation = false,
            };
            options.CompilerOptions = " -debug- -optimize+";
            StringBuilder sb = new("namespace MyDynamicNameSpace");
            sb.AppendLine("{");
            sb.AppendLine("public static class MyDynamicClass");
            sb.AppendLine("{");
            sb.AppendLine("public static bool MyDynamicMethod(string line, DateTime dateline)");
            sb.AppendLine("{");
            sb.AppendLine(script);
            sb.AppendLine("return false;");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("}");
            return compilator.CompileAssemblyFromSource(options, sb.ToString());
        }
    }
}
