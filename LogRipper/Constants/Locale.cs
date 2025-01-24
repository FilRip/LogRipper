using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using LogRipper.Helpers;

namespace LogRipper.Constants;

internal static class Locale
{
#pragma warning disable S3993 // Custom attributes should be marked with "System.AttributeUsageAttribute"
    private sealed class ReadTranslationAttribute : Attribute { }
#pragma warning restore S3993 // Custom attributes should be marked with "System.AttributeUsageAttribute"

    #region Fields

    internal static string Language { get; private set; } = "fr-FR";
    internal static List<string> ListConditions { get; private set; }
    private static string[] _languageFile;

    #endregion

    #region Translation
    [ReadTranslation()]
    public static string LBL_CONTAINS { get; private set; }
    [ReadTranslation()]
    public static string LBL_START_WITH { get; private set; }
    [ReadTranslation()]
    public static string LBL_END_WITH { get; private set; }
    [ReadTranslation()]
    public static string LBL_REG_EX { get; private set; }
    [ReadTranslation()]
    public static string LBL_SCRIPT_CSHARP { get; private set; }
    [ReadTranslation()]
    public static string MENU_FILE { get; private set; }
    [ReadTranslation()]
    public static string MENU_OPEN { get; private set; }
    [ReadTranslation()]
    public static string MENU_MERGE { get; private set; }
    [ReadTranslation()]
    public static string MENU_EXPORT_HTML { get; private set; }
    [ReadTranslation()]
    public static string MENU_EXIT { get; private set; }
    [ReadTranslation()]
    public static string MENU_RULES { get; private set; }
    [ReadTranslation()]
    public static string MENU_ADD_RULE { get; private set; }
    [ReadTranslation()]
    public static string MENU_MANAGE_RULES { get; private set; }
    [ReadTranslation()]
    public static string MENU_SAVE_RULES { get; private set; }
    [ReadTranslation()]
    public static string MENU_LOAD_RULES { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENCODING { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENC_DEFAULT { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENC_ASCII { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENC_UTF7 { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENC_UTF8 { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENC_UNICODE { get; private set; }
    [ReadTranslation()]
    public static string MENU_EDIT { get; private set; }
    [ReadTranslation()]
    public static string MENU_COPY { get; private set; }
    [ReadTranslation()]
    public static string MENU_SEARCH { get; private set; }
    [ReadTranslation()]
    public static string MENU_GOTO_LINE { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW_NUMLINE { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW_FILENAME { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW_SEARCHRESULT { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENC_UTF32 { get; private set; }
    [ReadTranslation()]
    public static string BTN_ADD_RULE { get; private set; }
    [ReadTranslation()]
    public static string BTN_CANCEL_RULE { get; private set; }
    [ReadTranslation()]
    public static string BTN_EDIT_RULE { get; private set; }
    [ReadTranslation()]
    public static string BTN_OK { get; private set; }
    [ReadTranslation()]
    public static string BTN_CANCEL { get; private set; }
    [ReadTranslation()]
    public static string LBL_ASK_DATEFORMAT { get; private set; }
    [ReadTranslation()]
    public static string LBL_MERGE_FILENAME { get; private set; }
    [ReadTranslation()]
    public static string LBL_DEFAULT_TEXT_COLOR { get; private set; }
    [ReadTranslation()]
    public static string LBL_DEFAULT_BACK_COLOR { get; private set; }
    [ReadTranslation()]
    public static string LBL_LIST_RULES { get; private set; }
    [ReadTranslation()]
    public static string BTN_DEL_RULE { get; private set; }
    [ReadTranslation()]
    public static string BTN_SEE_RULE_RESULT { get; private set; }
    [ReadTranslation()]
    public static string TITLE_NEW_RULE { get; private set; } = "TITLE_NEW_RULE";
    [ReadTranslation()]
    public static string TITLE_EDIT_RULE { get; private set; } = "TITLE_EDIT_RULE";
    [ReadTranslation()]
    public static string LBL_RULE_CONDITION { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_TEXT { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_TEXT_COLOR { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_BACK_COLOR { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_CASE_SENSITIVE { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_HIDE_LINE { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_SCRIPT_CS { get; private set; }
    [ReadTranslation()]
    public static string HELP_SCRIPT_CS { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_REGEX { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_REVERSE { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_DESCRIPTION { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_PRIORITY { get; private set; }
    [ReadTranslation()]
    public static string TITLE_LIST_RULES { get; private set; } = "TITLE_LIST_RULES";
    [ReadTranslation()]
    public static string TITLE_MERGE_FILE { get; private set; } = "TITLE_MERGE_FILE";
    [ReadTranslation()]
    public static string LBL_SEARCH_RESULT { get; private set; }
    [ReadTranslation()]
    public static string LBL_RESULT { get; private set; }
    [ReadTranslation()]
    public static string MENU_SETTINGS { get; private set; }
    [ReadTranslation()]
    public static string MENU_OPTIONS { get; private set; }
    [ReadTranslation()]
    public static string TITLE_SEARCH { get; private set; } = "TITLE_SEARCH";
    [ReadTranslation()]
    public static string LBL_SEARCH_TEXT { get; private set; }
    [ReadTranslation()]
    public static string TITLE_GOTO { get; private set; } = "TITLE_GOTO";
    [ReadTranslation()]
    public static string LBL_GOTO_LINE { get; private set; }
    [ReadTranslation()]
    public static string ERROR_SAVE_RULES { get; private set; }
    [ReadTranslation()]
    public static string ERROR_LOAD_RULES { get; private set; }
    [ReadTranslation()]
    public static string TITLE_ERROR { get; private set; }
    [ReadTranslation()]
    public static string ERROR_EMPTY_FILE { get; private set; }
    [ReadTranslation()]
    public static string ERROR_DATEFORMAT { get; private set; }
    [ReadTranslation()]
    public static string ASK_CONFIRM_DEL { get; private set; }
    [ReadTranslation()]
    public static string TITLE_CONFIRM_DEL { get; private set; }
    [ReadTranslation()]
    public static string LBL_SEARCH_CASE_SENSITIVE { get; private set; }
    [ReadTranslation()]
    public static string ERROR_READING_FILE { get; private set; }
    [ReadTranslation()]
    public static string ERROR_FILE_ALREADY_LOADED { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENABLE_AUTO_RELOAD { get; private set; }
    [ReadTranslation()]
    public static string MENU_ENABLE_AUTO_SCROLL { get; private set; }
    [ReadTranslation()]
    public static string OPTION_LANGUAGE { get; private set; }
    [ReadTranslation()]
    public static string RESET_WINDOW_POS { get; private set; }
    [ReadTranslation()]
    public static string OPTION_THEME { get; private set; }
    [ReadTranslation()]
    public static string THEME_LIGHT { get; private set; }
    [ReadTranslation()]
    public static string THEME_DARK { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW_MARGIN { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW_TOOLBAR { get; private set; }
    [ReadTranslation()]
    public static string TOOLBAR_LIST_FILES { get; private set; }
    [ReadTranslation()]
    public static string TOOLBAR_LIST_RULES { get; private set; }
    [ReadTranslation()]
    public static string MENU_SAVE_STATE { get; private set; }
    [ReadTranslation()]
    public static string MENU_LOAD_STATE { get; private set; }
    [ReadTranslation()]
    public static string ERROR_NO_CONDITION { get; private set; }
    [ReadTranslation()]
    public static string ERROR_REGEX { get; private set; }
    [ReadTranslation()]
    public static string ERROR_SCRIPT { get; private set; }
    [ReadTranslation()]
    public static string ERROR_TEXT { get; private set; }
    [ReadTranslation()]
    public static string ERROR_SELECT_RULE { get; private set; }
    [ReadTranslation()]
    public static string PREVIEW_COLOR { get; private set; }
    [ReadTranslation()]
    public static string EXPLORER_OPEN_WITH { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_ADD_EXPLORER { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_REMOVE_EXPLORER { get; private set; }
    [ReadTranslation()]
    public static string MENU_ABOUT { get; private set; }
    [ReadTranslation()]
    public static string TITLE_SETTINGS { get; private set; } = "TITLE_SETTINGS";
    [ReadTranslation()]
    public static string ABOUT { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_CATEGORY { get; private set; }
    [ReadTranslation()]
    public static string OPTIONS_DEFAULT_RULES { get; private set; }
    [ReadTranslation()]
    public static string OPTIONS_FONT_SIZE { get; private set; }
    [ReadTranslation()]
    public static string OPTIONS_SPACE_SIZE { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_AUTO_SHOW_MARGIN { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_AUTO_SHOW_TOOLBAR { get; private set; }
    [ReadTranslation()]
    public static string TOOLBAR_LIST_CATEGORY { get; private set; }
    [ReadTranslation()]
    public static string RELOAD_FILE { get; private set; }
    [ReadTranslation()]
    public static string REMOVE_FILE { get; private set; }
    [ReadTranslation()]
    public static string CONFIRM_REMOVE_FILE { get; private set; }
    [ReadTranslation()]
    public static string LBL_DATE_FILTER { get; private set; }
    [ReadTranslation()]
    public static string LBL_DATE_START_FILTER { get; private set; }
    [ReadTranslation()]
    public static string LBL_DATE_END_FILTER { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW_DATE_FILTER { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_AUTO_SHOW_DATE_FILTER { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_AUTO_FOLLOW_MARGIN { get; private set; }
    [ReadTranslation()]
    public static string HIDE_ALL_OTHERS_LINES { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_BOLD { get; private set; }
    [ReadTranslation()]
    public static string LBL_RULE_ITALIC { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_WRAP { get; private set; }
    [ReadTranslation()]
    public static string CONCATENATION_AND { get; private set; }
    [ReadTranslation()]
    public static string CONCATENATION_OR { get; private set; }
    [ReadTranslation()]
    public static string MENU_RELOAD_FILES { get; private set; }
    [ReadTranslation()]
    public static string ERROR_SAVE_FILE { get; private set; }
    [ReadTranslation()]
    public static string MENU_SAVE_SEARCH_RESULT { get; private set; }
    [ReadTranslation()]
    public static string MENU_MERGE_RULES { get; private set; }
    [ReadTranslation()]
    public static string ERROR_NO_RULE_FOUND { get; private set; }
    [ReadTranslation()]
    public static string LBL_CONCATENATION_RULE { get; private set; }
    [ReadTranslation()]
    public static string ADD_SUB_RULE { get; private set; }
    [ReadTranslation()]
    public static string REMOVE_SUB_RULE { get; private set; }
    [ReadTranslation()]
    public static string EDIT_SUB_RULE { get; private set; }
    [ReadTranslation()]
    public static string LBL_SUB_RULES { get; private set; }
    [ReadTranslation()]
    public static string LBL_FONT_PROPERTIES { get; private set; }
    [ReadTranslation()]
    public static string HELP_PRIORITY { get; private set; }
    [ReadTranslation()]
    public static string LBL_NEW_VERSION { get; private set; }
    [ReadTranslation()]
    public static string LBL_ERROR_EXTRACT_NEW_VERSION { get; private set; }
    [ReadTranslation()]
    public static string LBL_ERROR_INSTALL_NEW_VERSION { get; private set; }
    [ReadTranslation()]
    public static string LBL_ERROR_DOWNLOAD_NEW_VERSION { get; private set; }
    [ReadTranslation()]
    public static string LBL_RESTART_APP_NEW_VERSION { get; private set; }
    [ReadTranslation()]
    public static string LBL_VERSION_COMPARE { get; private set; }
    [ReadTranslation()]
    public static string MENU_CHECK_NEW_VERSION { get; private set; }
    [ReadTranslation()]
    public static string ERROR_CHECK_NEW_VERSION { get; private set; }
    [ReadTranslation()]
    public static string NO_DATEFORMAT_IN_FILE { get; private set; }
    [ReadTranslation()]
    public static string HIDE_LINE_BEFORE { get; private set; }
    [ReadTranslation()]
    public static string HIDE_LINE_AFTER { get; private set; }
    [ReadTranslation()]
    public static string PLEASE_WAIT { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW_HIDE_LINE_BA { get; private set; }
    [ReadTranslation()]
    public static string LBL_GROUP_LINE { get; private set; }
    [ReadTranslation()]
    public static string MENU_HIDE_SELECTED { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_SHOW_SYSTRAY { get; private set; }
    [ReadTranslation()]
    public static string SETTINGS_MINIMIZE_SYSTRAY { get; private set; }
    [ReadTranslation()]
    public static string MENU_SHOW_MAINWINDOW { get; private set; }
    [ReadTranslation()]
    public static string CLOSE_TAB { get; private set; }
    [ReadTranslation()]
    public static string CLOSE_ALL_TABS { get; private set; }
    [ReadTranslation()]
    public static string CLOSE_OTHER_TABS { get; private set; }
    [ReadTranslation()]
    public static string LBL_NOT_CONTAINS { get; private set; }
    [ReadTranslation()]
    public static string ASK_EDIT_FILE_NOW { get; private set; }
    [ReadTranslation()]
    public static string ASK_MERGE_OR_REPLACE { get; private set; }
    [ReadTranslation()]
    public static string PREVIEW_FIRST_LINE { get; private set; }
    [ReadTranslation()]
    public static string INVALID_DATE_FORMAT { get; private set; }
    [ReadTranslation()]
    public static string REGISTER_BETA_CHANNEL { get; private set; }
    [ReadTranslation()]
    public static string READ_CONSOLE { get; private set; }
    [ReadTranslation()]
    public static string CHOICE_PROCESS { get; private set; }
    [ReadTranslation()]
    public static string ERROR_ATTACH_CONSOLE { get; private set; }
    [ReadTranslation()]
    public static string NOT_DURING_READ_CONSOLE_MODE { get; private set; }
    [ReadTranslation()]
    public static string TITLE_DELETE { get; private set; }
    [ReadTranslation()]
    public static string ASK_DELETE_LINES { get; private set; }
    #endregion

    #region Methods

    internal static void Init()
    {
        Language = Properties.Settings.Default.Language;
        string filename = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "Languages", Language + ".ini");
        if (!File.Exists(filename))
            filename = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "Languages", "en-us.ini");
        if (File.Exists(filename))
        {
            _languageFile = File.ReadAllLines(filename);

            foreach (PropertyInfo pi in typeof(Locale).GetProperties(BindingFlags.Public | BindingFlags.Static).Where(pi => pi.GetCustomAttribute<ReadTranslationAttribute>() != null))
                pi.SetValue(null, GetTranslation(pi.Name));
        }
        else
        {
            WpfMessageBox.ShowModal("No language file found. Check your install of LogRipper, or files in subfolder 'Languages'", "Error");
            Environment.Exit(2);
        }
        ABOUT = ABOUT.Replace(@"\r\n", Environment.NewLine);
        ABOUT = ABOUT.Replace(@"\r", Environment.NewLine);
        ABOUT = ABOUT.Replace(@"\n", Environment.NewLine);
        ListConditions =
        [
            LBL_CONTAINS,
            LBL_NOT_CONTAINS,
            LBL_START_WITH,
            LBL_END_WITH,
            LBL_REG_EX,
            LBL_SCRIPT_CSHARP,
        ];
    }

    private static string GetTranslation(string label)
    {
        if (_languageFile?.Length > 0)
        {
            string ret = Array.Find(_languageFile, line => line.StartsWith(label + "="));
            if (!string.IsNullOrWhiteSpace(ret))
                return ret.Substring(ret.IndexOf("=") + 1);
        }
        return label;
    }

    #endregion
}
