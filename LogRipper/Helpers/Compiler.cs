using System;
using System.CodeDom.Compiler;
using System.Text;
using System.Text.RegularExpressions;

using LogRipper.Constants;

using Microsoft.CSharp;

namespace LogRipper.Helpers;

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

    internal static bool TestCondition(Conditions cond, string text, bool showMessageBox = true)
    {
        switch (cond)
        {
            case Conditions.REG_EX:
                try
                {
                    _ = new Regex(text);
                }
                catch (Exception ex)
                {
                    if (showMessageBox)
                        WpfMessageBox.ShowModal(Locale.ERROR_REGEX + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
                    return false;
                }
                break;
            case Conditions.SCRIPT:
                CompilerResults result = Compile(text);
                if (result == null || result.Errors.Count > 0)
                {
                    StringBuilder sb = new();
                    if (result != null && result.Errors.Count > 0)
                    {
                        foreach (CompilerError error in result.Errors)
                            sb.AppendLine(error.ToString());
                    }
                    if (showMessageBox)
                        WpfMessageBox.ShowModal(Locale.ERROR_SCRIPT + Environment.NewLine + sb.ToString(), Locale.TITLE_ERROR);
                    return false;
                }
                break;
            default:
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (showMessageBox)
                        WpfMessageBox.ShowModal(Locale.ERROR_TEXT, Locale.TITLE_ERROR);
                    return false;
                }
                break;
        }
        return true;
    }
}
