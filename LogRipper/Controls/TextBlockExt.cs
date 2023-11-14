using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace LogRipper.Controls;

internal static class TextBlockExt
{
    internal static readonly Regex _regex = new(@"http[s]?://[^\s ]+", RegexOptions.Compiled);

    public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.RegisterAttached("FormattedText",
        typeof(string), typeof(TextBlockExt), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, FormattedTextPropertyChanged));

    public static void SetFormattedText(DependencyObject textBlock, string value)
    {
        textBlock.SetValue(FormattedTextProperty, value);
    }

    public static string GetFormattedText(DependencyObject textBlock)
    {
        return (string)textBlock.GetValue(FormattedTextProperty);
    }

    internal static void FormattedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock textBlock)
            return;

        string formattedText = (string)e.NewValue ?? string.Empty;
        string fullText = $"<Span xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">{formattedText}</Span>";

        textBlock.Inlines.Clear();
        using XmlReader xmlReader1 = XmlReader.Create(new StringReader(fullText));
        try
        {
            Span result = (Span)XamlReader.Load(xmlReader1);
            RecognizeHyperlinks(result);
            textBlock.Inlines.Add(result);
        }
        catch
        {
            formattedText = System.Security.SecurityElement.Escape(formattedText);
            fullText = $"<Span xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">{formattedText}</Span>";

            using XmlReader xmlReader2 = XmlReader.Create(new StringReader(fullText));
            try
            {
                dynamic result = (Span)XamlReader.Load(xmlReader2);
                textBlock.Inlines.Add(result);
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }

    internal static void RecognizeHyperlinks(Inline originalInline)
    {
        if (originalInline is not Span span)
            return;

        Dictionary<Inline, List<Inline>> replacements = new();
        List<Inline> startInlines = new(span.Inlines);
        foreach (Inline i in startInlines)
        {
            switch (i)
            {
                case Hyperlink _:
                    continue;
                case Run run:
                    {
                        if (!_regex.IsMatch(run.Text))
                            continue;
                        List<Inline> newLines = GetHyperlinks(run);
                        replacements.Add(run, newLines);
                        break;
                    }
                default:
                    RecognizeHyperlinks(i);
                    break;
            }
        }

        if (!replacements.Any())
            return;

        List<Inline> currentInlines = new(span.Inlines);
        span.Inlines.Clear();
        foreach (Inline i in currentInlines)
        {
            if (replacements.ContainsKey(i))
                span.Inlines.AddRange(replacements[i]);
            else
                span.Inlines.Add(i);
        }
    }

    internal static List<Inline> GetHyperlinks(Run run)
    {
        List<Inline> result = new();
        string currentText = run.Text;
        do
        {
            if (!_regex.IsMatch(currentText))
            {
                if (!string.IsNullOrEmpty(currentText)) result.Add(new Run(currentText));
                break;
            }

            Match match = _regex.Match(currentText);

            if (match.Index > 0)
                result.Add(new Run(currentText.Substring(0, match.Index)));

            Hyperlink hyperLink = new()
            {
                NavigateUri = new Uri(match.Value),
            };
            hyperLink.Inlines.Add(match.Value);
            hyperLink.RequestNavigate += HyperLink_RequestNavigate;
            result.Add(hyperLink);

            currentText = currentText.Substring(match.Index + match.Length);
        } while (true);

        return result;
    }

    internal static void HyperLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(e.Uri.ToString());
        }
        catch (Exception) { /* Ignore errors */ }
    }
}
