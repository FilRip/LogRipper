using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

using LogRipper.Constants;
using LogRipper.Helpers;

namespace LogRipper.Models;

public abstract partial class RuleViewModelBase : ObservableObject
{
    private Assembly _dll;
    private MethodInfo _mi;
    private Regex _regex;

    [ObservableProperty()]
    [property: XmlElement()]
    private string _text;

    partial void OnTextChanged(string value)
    {
        _dll = null;
        _mi = null;
        _regex = null;
    }

    private StringComparison MyStringComparison
    {
        get
        {
            return CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        }
    }

    [XmlElement()]
    public bool CaseSensitive { get; set; }

    [XmlElement()]
    public Conditions Conditions { get; set; }

    internal virtual bool Execute(string line, DateTime dateline)
    {
        bool result = false;
        if (Conditions == Conditions.CONTAINS)
        {
            result = line.IndexOf(Text, 0, MyStringComparison) >= 0;
        }
        else if (Conditions == Conditions.START_WITH)
        {
            result = line.StartsWith(Text, MyStringComparison);
        }
        else if (Conditions == Conditions.END_WITH)
        {
            result = line.EndsWith(Text, MyStringComparison);
        }
        else if (Conditions == Conditions.REG_EX)
        {
            _regex ??= new Regex(Text);
            result = _regex.Match(line).Success;
        }
        else if (Conditions == Conditions.SCRIPT)
        {
            if (_dll == null)
            {
                CompilerResults compResult = Compiler.Compile(Text);
                if (compResult.Errors.Count == 0)
                {
                    _dll = compResult.CompiledAssembly;
                    Type myClass = _dll.GetType("MyDynamicNameSpace.MyDynamicClass");
                    _mi = myClass.GetMethod("MyDynamicMethod", BindingFlags.Public | BindingFlags.Static);
                }
            }
            if (_mi != null)
            {
                result = (bool)_mi.Invoke(null, new object[] { line, dateline });
            }
        }

        return result;
    }

    internal virtual bool AreSame(RuleViewModelBase baseRule)
    {
        if (baseRule.Conditions != Conditions)
            return false;
        if (baseRule.CaseSensitive != CaseSensitive)
            return false;
        if (baseRule.Text != Text)
            return false;
        return true;
    }

    internal virtual void Refresh()
    {
        OnPropertyChanged(nameof(Conditions));
        OnPropertyChanged(nameof(Text));
        OnPropertyChanged(nameof(CaseSensitive));
    }
}
