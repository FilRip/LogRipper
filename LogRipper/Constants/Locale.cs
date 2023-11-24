using System;
using System.Collections.Generic;
using System.IO;

using LogRipper.Helpers;

namespace LogRipper.Constants;

internal static class Locale
{
    #region Fields

    internal static string Language { get; set; } = "fr-FR";
    internal static List<string> ListConditions { get; set; }
    private static string[] _languageFile;

    #endregion

    #region Translation
    public static string LBL_CONTAINS { get; set; }
    public static string LBL_START_WITH { get; set; }
    public static string LBL_END_WITH { get; set; }
    public static string LBL_REG_EX { get; set; }
    public static string LBL_SCRIPT_CSHARP { get; set; }
    public static string MENU_FILE { get; set; }
    public static string MENU_OPEN { get; set; }
    public static string MENU_MERGE { get; set; }
    public static string MENU_EXPORT_HTML { get; set; }
    public static string MENU_EXIT { get; set; }
    public static string MENU_RULES { get; set; }
    public static string MENU_ADD_RULE { get; set; }
    public static string MENU_MANAGE_RULES { get; set; }
    public static string MENU_SAVE_RULES { get; set; }
    public static string MENU_LOAD_RULES { get; set; }
    public static string MENU_ENCODING { get; set; }
    public static string MENU_ENC_DEFAULT { get; set; }
    public static string MENU_ENC_ASCII { get; set; }
    public static string MENU_ENC_UTF7 { get; set; }
    public static string MENU_ENC_UTF8 { get; set; }
    public static string MENU_ENC_UNICODE { get; set; }
    public static string MENU_EDIT { get; set; }
    public static string MENU_COPY { get; set; }
    public static string MENU_SEARCH { get; set; }
    public static string MENU_GOTO_LINE { get; set; }
    public static string MENU_SHOW { get; set; }
    public static string MENU_SHOW_NUMLINE { get; set; }
    public static string MENU_SHOW_FILENAME { get; set; }
    public static string MENU_SHOW_SEARCHRESULT { get; set; }
    public static string MENU_ENC_UTF32 { get; set; }
    public static string BTN_ADD_RULE { get; set; }
    public static string BTN_CANCEL_RULE { get; set; }
    public static string BTN_EDIT_RULE { get; set; }
    public static string BTN_OK { get; set; }
    public static string BTN_CANCEL { get; set; }
    public static string LBL_ASK_DATEFORMAT { get; set; }
    public static string LBL_MERGE_FILENAME { get; set; }
    public static string LBL_DEFAULT_TEXT_COLOR { get; set; }
    public static string LBL_DEFAULT_BACK_COLOR { get; set; }
    public static string LBL_LIST_RULES { get; set; }
    public static string BTN_DEL_RULE { get; set; }
    public static string BTN_SEE_RULE_RESULT { get; set; }
    public static string TITLE_NEW_RULE { get; set; } = "TITLE_NEW_RULE";
    public static string TITLE_EDIT_RULE { get; set; } = "TITLE_EDIT_RULE";
    public static string LBL_RULE_CONDITION { get; set; }
    public static string LBL_RULE_TEXT { get; set; }
    public static string LBL_RULE_TEXT_COLOR { get; set; }
    public static string LBL_RULE_BACK_COLOR { get; set; }
    public static string LBL_RULE_CASE_SENSITIVE { get; set; }
    public static string LBL_RULE_HIDE_LINE { get; set; }
    public static string LBL_RULE_SCRIPT_CS { get; set; }
    public static string HELP_SCRIPT_CS { get; set; }
    public static string LBL_RULE_REGEX { get; set; }
    public static string LBL_RULE_REVERSE { get; set; }
    public static string LBL_RULE_DESCRIPTION { get; set; }
    public static string LBL_RULE_PRIORITY { get; set; }
    public static string TITLE_LIST_RULES { get; set; } = "TITLE_LIST_RULES";
    public static string TITLE_MERGE_FILE { get; set; } = "TITLE_MERGE_FILE";
    public static string LBL_SEARCH_RESULT { get; set; }
    public static string LBL_RESULT { get; set; }
    public static string MENU_SETTINGS { get; set; }
    public static string MENU_OPTIONS { get; set; }
    public static string TITLE_SEARCH { get; set; } = "TITLE_SEARCH";
    public static string LBL_SEARCH_TEXT { get; set; }
    public static string TITLE_GOTO { get; set; } = "TITLE_GOTO";
    public static string LBL_GOTO_LINE { get; set; }
    public static string ERROR_SAVE_RULES { get; set; }
    public static string ERROR_LOAD_RULES { get; set; }
    public static string TITLE_ERROR { get; set; }
    public static string ERROR_EMPTY_FILE { get; set; }
    public static string ERROR_DATEFORMAT { get; set; }
    public static string ASK_CONFIRM_DEL { get; set; }
    public static string TITLE_CONFIRM_DEL { get; set; }
    public static string LBL_SEARCH_CASE_SENSITIVE { get; set; }
    public static string ERROR_READING_FILE { get; set; }
    public static string ERROR_FILE_ALREADY_LOADED { get; set; }
    public static string MENU_ENABLE_AUTO_RELOAD { get; set; }
    public static string MENU_ENABLE_AUTO_SCROLL { get; set; }
    public static string OPTION_LANGUAGE { get; set; }
    public static string RESET_WINDOW_POS { get; set; }
    public static string OPTION_THEME { get; set; }
    public static string THEME_LIGHT { get; set; }
    public static string THEME_DARK { get; set; }
    public static string MENU_SHOW_MARGIN { get; set; }
    public static string MENU_SHOW_TOOLBAR { get; set; }
    public static string TOOLBAR_LIST_FILES { get; set; }
    public static string TOOLBAR_LIST_RULES { get; set; }
    public static string MENU_SAVE_STATE { get; set; }
    public static string MENU_LOAD_STATE { get; set; }
    public static string ERROR_NO_CONDITION { get; set; }
    public static string ERROR_REGEX { get; set; }
    public static string ERROR_SCRIPT { get; set; }
    public static string ERROR_TEXT { get; set; }
    public static string ERROR_SELECT_RULE { get; set; }
    public static string PREVIEW_COLOR { get; set; }
    public static string EXPLORER_OPEN_WITH { get; set; }
    public static string SETTINGS_ADD_EXPLORER { get; set; }
    public static string SETTINGS_REMOVE_EXPLORER { get; set; }
    public static string MENU_ABOUT { get; set; }
    public static string TITLE_SETTINGS { get; set; } = "TITLE_SETTINGS";
    public static string ABOUT { get; set; }
    public static string LBL_RULE_CATEGORY { get; set; }
    public static string OPTIONS_DEFAULT_RULES { get; set; }
    public static string OPTIONS_FONT_SIZE { get; set; }
    public static string OPTIONS_SPACE_SIZE { get; set; }
    public static string SETTINGS_AUTO_SHOW_MARGIN { get; set; }
    public static string SETTINGS_AUTO_SHOW_TOOLBAR { get; set; }
    public static string TOOLBAR_LIST_CATEGORY { get; set; }
    public static string RELOAD_FILE { get; set; }
    public static string REMOVE_FILE { get; set; }
    public static string CONFIRM_REMOVE_FILE { get; set; }
    public static string LBL_DATE_FILTER { get; set; }
    public static string LBL_DATE_START_FILTER { get; set; }
    public static string LBL_DATE_END_FILTER { get; set; }
    public static string MENU_SHOW_DATE_FILTER { get; set; }
    public static string SETTINGS_AUTO_SHOW_DATE_FILTER { get; set; }
    public static string SETTINGS_AUTO_FOLLOW_MARGIN { get; set; }
    public static string HIDE_ALL_OTHERS_LINES { get; set; }
    public static string LBL_RULE_BOLD { get; set; }
    public static string LBL_RULE_ITALIC { get; set; }
    public static string SETTINGS_WRAP { get; set; }
    public static string CONCATENATION_AND { get; set; }
    public static string CONCATENATION_OR { get; set; }
    public static string MENU_RELOAD_FILES { get; set; }
    public static string ERROR_SAVE_FILE { get; set; }
    public static string MENU_SAVE_SEARCH_RESULT { get; set; }
    public static string MENU_MERGE_RULES { get; set; }
    public static string ERROR_NO_RULE_FOUND { get; set; }
    public static string LBL_CONCATENATION_RULE { get; set; }
    public static string ADD_SUB_RULE { get; set; }
    public static string REMOVE_SUB_RULE { get; set; }
    public static string EDIT_SUB_RULE { get; set; }
    public static string LBL_SUB_RULES { get; set; }
    public static string LBL_FONT_PROPERTIES { get; set; }
    public static string HELP_PRIORITY { get; set; }
    public static string LBL_NEW_VERSION { get; set; }
    public static string LBL_ERROR_EXTRACT_NEW_VERSION { get; set; }
    public static string LBL_ERROR_INSTALL_NEW_VERSION { get; set; }
    public static string LBL_ERROR_DOWNLOAD_NEW_VERSION { get; set; }
    public static string LBL_RESTART_APP_NEW_VERSION { get; set; }
    public static string LBL_VERSION_COMPARE { get; set; }
    public static string MENU_CHECK_NEW_VERSION { get; set; }
    public static string ERROR_CHECK_NEW_VERSION { get; set; }
    public static string NO_DATEFORMAT_IN_FILE { get; set; }
    public static string HIDE_LINE_BEFORE { get; set; }
    public static string HIDE_LINE_AFTER { get; set; }
    public static string PLEASE_WAIT { get; set; }
    public static string MENU_SHOW_HIDE_LINE_BA { get; set; }
    public static string LBL_GROUP_LINE { get; set; }
    public static string MENU_HIDE_SELECTED { get; set; }
    public static string SETTINGS_SHOW_SYSTRAY { get; set; }
    public static string SETTINGS_MINIMIZE_SYSTRAY { get; set; }
    public static string MENU_SHOW_MAINWINDOW { get; set; }
    public static string CLOSE_TAB { get; set; }
    public static string CLOSE_ALL_TABS { get; set; }
    public static string CLOSE_OTHER_TABS { get; set; }
    public static string LBL_NOT_CONTAINS { get; set; }
    public static string ASK_EDIT_FILE_NOW { get; set; }
    public static string ASK_MERGE_OR_REPLACE { get; set; }
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
            LBL_CONTAINS = GetTranslation(nameof(LBL_CONTAINS));
            LBL_START_WITH = GetTranslation(nameof(LBL_START_WITH));
            LBL_END_WITH = GetTranslation(nameof(LBL_END_WITH));
            LBL_REG_EX = GetTranslation(nameof(LBL_REG_EX));
            LBL_SCRIPT_CSHARP = GetTranslation(nameof(LBL_SCRIPT_CSHARP));
            MENU_FILE = GetTranslation(nameof(MENU_FILE));
            MENU_OPEN = GetTranslation(nameof(MENU_OPEN));
            MENU_MERGE = GetTranslation(nameof(MENU_MERGE));
            MENU_EXPORT_HTML = GetTranslation(nameof(MENU_EXPORT_HTML));
            MENU_EXIT = GetTranslation(nameof(MENU_EXIT));
            MENU_RULES = GetTranslation(nameof(MENU_RULES));
            MENU_ADD_RULE = GetTranslation(nameof(MENU_ADD_RULE));
            MENU_MANAGE_RULES = GetTranslation(nameof(MENU_MANAGE_RULES));
            MENU_SAVE_RULES = GetTranslation(nameof(MENU_SAVE_RULES));
            MENU_LOAD_RULES = GetTranslation(nameof(MENU_LOAD_RULES));
            MENU_ENCODING = GetTranslation(nameof(MENU_ENCODING));
            MENU_ENC_DEFAULT = GetTranslation(nameof(MENU_ENC_DEFAULT));
            MENU_ENC_ASCII = GetTranslation(nameof(MENU_ENC_ASCII));
            MENU_ENC_UTF7 = GetTranslation(nameof(MENU_ENC_UTF7));
            MENU_ENC_UTF8 = GetTranslation(nameof(MENU_ENC_UTF8));
            MENU_ENC_UNICODE = GetTranslation(nameof(MENU_ENC_UNICODE));
            MENU_EDIT = GetTranslation(nameof(MENU_EDIT));
            MENU_COPY = GetTranslation(nameof(MENU_COPY));
            MENU_SEARCH = GetTranslation(nameof(MENU_SEARCH));
            MENU_GOTO_LINE = GetTranslation(nameof(MENU_GOTO_LINE));
            MENU_SHOW = GetTranslation(nameof(MENU_SHOW));
            MENU_SHOW_NUMLINE = GetTranslation(nameof(MENU_SHOW_NUMLINE));
            MENU_SHOW_FILENAME = GetTranslation(nameof(MENU_SHOW_FILENAME));
            MENU_SHOW_SEARCHRESULT = GetTranslation(nameof(MENU_SHOW_SEARCHRESULT));
            MENU_ENC_UTF32 = GetTranslation(nameof(MENU_ENC_UTF32));
            BTN_ADD_RULE = GetTranslation(nameof(BTN_ADD_RULE));
            BTN_CANCEL_RULE = GetTranslation(nameof(BTN_CANCEL_RULE));
            BTN_EDIT_RULE = GetTranslation(nameof(BTN_EDIT_RULE));
            BTN_OK = GetTranslation(nameof(BTN_OK));
            BTN_CANCEL = GetTranslation(nameof(BTN_CANCEL));
            LBL_ASK_DATEFORMAT = GetTranslation(nameof(LBL_ASK_DATEFORMAT));
            LBL_MERGE_FILENAME = GetTranslation(nameof(LBL_MERGE_FILENAME));
            LBL_DEFAULT_TEXT_COLOR = GetTranslation(nameof(LBL_DEFAULT_TEXT_COLOR));
            LBL_DEFAULT_BACK_COLOR = GetTranslation(nameof(LBL_DEFAULT_BACK_COLOR));
            LBL_LIST_RULES = GetTranslation(nameof(LBL_LIST_RULES));
            BTN_DEL_RULE = GetTranslation(nameof(BTN_DEL_RULE));
            BTN_SEE_RULE_RESULT = GetTranslation(nameof(BTN_SEE_RULE_RESULT));
            TITLE_NEW_RULE = GetTranslation(nameof(TITLE_NEW_RULE));
            TITLE_EDIT_RULE = GetTranslation(nameof(TITLE_EDIT_RULE));
            LBL_RULE_CONDITION = GetTranslation(nameof(LBL_RULE_CONDITION));
            LBL_RULE_TEXT = GetTranslation(nameof(LBL_RULE_TEXT));
            LBL_RULE_TEXT_COLOR = GetTranslation(nameof(LBL_RULE_TEXT_COLOR));
            LBL_RULE_BACK_COLOR = GetTranslation(nameof(LBL_RULE_BACK_COLOR));
            LBL_RULE_CASE_SENSITIVE = GetTranslation(nameof(LBL_RULE_CASE_SENSITIVE));
            LBL_RULE_HIDE_LINE = GetTranslation(nameof(LBL_RULE_HIDE_LINE));
            LBL_RULE_SCRIPT_CS = GetTranslation(nameof(LBL_RULE_SCRIPT_CS));
            LBL_RULE_REGEX = GetTranslation(nameof(LBL_RULE_REGEX));
            LBL_RULE_REVERSE = GetTranslation(nameof(LBL_RULE_REVERSE));
            LBL_RULE_DESCRIPTION = GetTranslation(nameof(LBL_RULE_DESCRIPTION));
            LBL_RULE_PRIORITY = GetTranslation(nameof(LBL_RULE_PRIORITY));
            TITLE_LIST_RULES = GetTranslation(nameof(TITLE_LIST_RULES));
            TITLE_MERGE_FILE = GetTranslation(nameof(TITLE_MERGE_FILE));
            LBL_SEARCH_RESULT = GetTranslation(nameof(LBL_SEARCH_RESULT));
            LBL_RESULT = GetTranslation(nameof(LBL_RESULT));
            MENU_SETTINGS = GetTranslation(nameof(MENU_SETTINGS));
            MENU_OPTIONS = GetTranslation(nameof(MENU_OPTIONS));
            TITLE_SEARCH = GetTranslation(nameof(TITLE_SEARCH));
            LBL_SEARCH_TEXT = GetTranslation(nameof(LBL_SEARCH_TEXT));
            TITLE_GOTO = GetTranslation(nameof(TITLE_GOTO));
            LBL_GOTO_LINE = GetTranslation(nameof(LBL_GOTO_LINE));
            ERROR_SAVE_RULES = GetTranslation(nameof(ERROR_SAVE_RULES));
            ERROR_LOAD_RULES = GetTranslation(nameof(ERROR_LOAD_RULES));
            TITLE_ERROR = GetTranslation(nameof(TITLE_ERROR));
            ERROR_DATEFORMAT = GetTranslation(nameof(ERROR_DATEFORMAT));
            ASK_CONFIRM_DEL = GetTranslation(nameof(ASK_CONFIRM_DEL));
            TITLE_CONFIRM_DEL = GetTranslation(nameof(TITLE_CONFIRM_DEL));
            ERROR_EMPTY_FILE = GetTranslation(nameof(ERROR_EMPTY_FILE));
            LBL_SEARCH_CASE_SENSITIVE = GetTranslation(nameof(LBL_SEARCH_CASE_SENSITIVE));
            ERROR_READING_FILE = GetTranslation(nameof(ERROR_READING_FILE));
            ERROR_FILE_ALREADY_LOADED = GetTranslation(nameof(ERROR_FILE_ALREADY_LOADED));
            MENU_ENABLE_AUTO_RELOAD = GetTranslation(nameof(MENU_ENABLE_AUTO_RELOAD));
            MENU_ENABLE_AUTO_SCROLL = GetTranslation(nameof(MENU_ENABLE_AUTO_SCROLL));
            OPTION_LANGUAGE = GetTranslation(nameof(OPTION_LANGUAGE));
            RESET_WINDOW_POS = GetTranslation(nameof(RESET_WINDOW_POS));
            OPTION_THEME = GetTranslation(nameof(OPTION_THEME));
            THEME_LIGHT = GetTranslation(nameof(THEME_LIGHT));
            THEME_DARK = GetTranslation(nameof(THEME_DARK));
            MENU_SHOW_MARGIN = GetTranslation(nameof(MENU_SHOW_MARGIN));
            MENU_SHOW_TOOLBAR = GetTranslation(nameof(MENU_SHOW_TOOLBAR));
            TOOLBAR_LIST_FILES = GetTranslation(nameof(TOOLBAR_LIST_FILES));
            TOOLBAR_LIST_RULES = GetTranslation(nameof(TOOLBAR_LIST_RULES));
            MENU_SAVE_STATE = GetTranslation(nameof(MENU_SAVE_STATE));
            MENU_LOAD_STATE = GetTranslation(nameof(MENU_LOAD_STATE));
            ERROR_NO_CONDITION = GetTranslation(nameof(ERROR_NO_CONDITION));
            ERROR_REGEX = GetTranslation(nameof(ERROR_REGEX));
            ERROR_SCRIPT = GetTranslation(nameof(ERROR_SCRIPT));
            ERROR_TEXT = GetTranslation(nameof(ERROR_TEXT));
            ERROR_SELECT_RULE = GetTranslation(nameof(ERROR_SELECT_RULE));
            PREVIEW_COLOR = GetTranslation(nameof(PREVIEW_COLOR));
            EXPLORER_OPEN_WITH = GetTranslation(nameof(EXPLORER_OPEN_WITH));
            SETTINGS_ADD_EXPLORER = GetTranslation(nameof(SETTINGS_ADD_EXPLORER));
            SETTINGS_REMOVE_EXPLORER = GetTranslation(nameof(SETTINGS_REMOVE_EXPLORER));
            MENU_ABOUT = GetTranslation(nameof(MENU_ABOUT));
            ABOUT = GetTranslation(nameof(ABOUT));
            LBL_RULE_CATEGORY = GetTranslation(nameof(LBL_RULE_CATEGORY));
            OPTIONS_DEFAULT_RULES = GetTranslation(nameof(OPTIONS_DEFAULT_RULES));
            OPTIONS_FONT_SIZE = GetTranslation(nameof(OPTIONS_FONT_SIZE));
            OPTIONS_SPACE_SIZE = GetTranslation(nameof(OPTIONS_SPACE_SIZE));
            SETTINGS_AUTO_SHOW_MARGIN = GetTranslation(nameof(SETTINGS_AUTO_SHOW_MARGIN));
            SETTINGS_AUTO_SHOW_TOOLBAR = GetTranslation(nameof(SETTINGS_AUTO_SHOW_TOOLBAR));
            TOOLBAR_LIST_CATEGORY = GetTranslation(nameof(TOOLBAR_LIST_CATEGORY));
            RELOAD_FILE = GetTranslation(nameof(RELOAD_FILE));
            REMOVE_FILE = GetTranslation(nameof(REMOVE_FILE));
            CONFIRM_REMOVE_FILE = GetTranslation(nameof(CONFIRM_REMOVE_FILE));
            LBL_DATE_FILTER = GetTranslation(nameof(LBL_DATE_FILTER));
            LBL_DATE_START_FILTER = GetTranslation(nameof(LBL_DATE_START_FILTER));
            LBL_DATE_END_FILTER = GetTranslation(nameof(LBL_DATE_END_FILTER));
            MENU_SHOW_DATE_FILTER = GetTranslation(nameof(MENU_SHOW_DATE_FILTER));
            SETTINGS_AUTO_SHOW_DATE_FILTER = GetTranslation(nameof(SETTINGS_AUTO_SHOW_DATE_FILTER));
            SETTINGS_AUTO_FOLLOW_MARGIN = GetTranslation(nameof(SETTINGS_AUTO_FOLLOW_MARGIN));
            HIDE_ALL_OTHERS_LINES = GetTranslation(nameof(HIDE_ALL_OTHERS_LINES));
            LBL_RULE_BOLD = GetTranslation(nameof(LBL_RULE_BOLD));
            LBL_RULE_ITALIC = GetTranslation(nameof(LBL_RULE_ITALIC));
            SETTINGS_WRAP = GetTranslation(nameof(SETTINGS_WRAP));
            CONCATENATION_AND = GetTranslation(nameof(CONCATENATION_AND));
            CONCATENATION_OR = GetTranslation(nameof(CONCATENATION_OR));
            MENU_RELOAD_FILES = GetTranslation(nameof(MENU_RELOAD_FILES));
            ERROR_SAVE_FILE = GetTranslation(nameof(ERROR_SAVE_FILE));
            MENU_SAVE_SEARCH_RESULT = GetTranslation(nameof(MENU_SAVE_SEARCH_RESULT));
            MENU_MERGE_RULES = GetTranslation(nameof(MENU_MERGE_RULES));
            ERROR_NO_RULE_FOUND = GetTranslation(nameof(ERROR_NO_RULE_FOUND));
            ADD_SUB_RULE = GetTranslation(nameof(ADD_SUB_RULE));
            REMOVE_SUB_RULE = GetTranslation(nameof(REMOVE_SUB_RULE));
            EDIT_SUB_RULE = GetTranslation(nameof(EDIT_SUB_RULE));
            LBL_SUB_RULES = GetTranslation(nameof(LBL_SUB_RULES));
            LBL_FONT_PROPERTIES = GetTranslation(nameof(LBL_FONT_PROPERTIES));
            HELP_SCRIPT_CS = GetTranslation(nameof(HELP_SCRIPT_CS));
            HELP_PRIORITY = GetTranslation(nameof(HELP_PRIORITY));
            LBL_NEW_VERSION = GetTranslation(nameof(LBL_NEW_VERSION));
            LBL_ERROR_EXTRACT_NEW_VERSION = GetTranslation(nameof(LBL_ERROR_EXTRACT_NEW_VERSION));
            LBL_ERROR_DOWNLOAD_NEW_VERSION = GetTranslation(nameof(LBL_ERROR_DOWNLOAD_NEW_VERSION));
            LBL_ERROR_INSTALL_NEW_VERSION = GetTranslation(nameof(LBL_ERROR_INSTALL_NEW_VERSION));
            LBL_RESTART_APP_NEW_VERSION = GetTranslation(nameof(LBL_RESTART_APP_NEW_VERSION));
            LBL_VERSION_COMPARE = GetTranslation(nameof(LBL_VERSION_COMPARE));
            MENU_CHECK_NEW_VERSION = GetTranslation(nameof(MENU_CHECK_NEW_VERSION));
            ERROR_CHECK_NEW_VERSION = GetTranslation(nameof(ERROR_CHECK_NEW_VERSION));
            NO_DATEFORMAT_IN_FILE = GetTranslation(nameof(NO_DATEFORMAT_IN_FILE));
            HIDE_LINE_BEFORE = GetTranslation(nameof(HIDE_LINE_BEFORE));
            HIDE_LINE_AFTER = GetTranslation(nameof(HIDE_LINE_AFTER));
            TITLE_SETTINGS = GetTranslation(nameof(TITLE_SETTINGS));
            PLEASE_WAIT = GetTranslation(nameof(PLEASE_WAIT));
            MENU_SHOW_HIDE_LINE_BA = GetTranslation(nameof(MENU_SHOW_HIDE_LINE_BA));
            LBL_GROUP_LINE = GetTranslation(nameof(LBL_GROUP_LINE));
            MENU_HIDE_SELECTED = GetTranslation(nameof(MENU_HIDE_SELECTED));
            SETTINGS_SHOW_SYSTRAY = GetTranslation(nameof(SETTINGS_SHOW_SYSTRAY));
            SETTINGS_MINIMIZE_SYSTRAY = GetTranslation(nameof(SETTINGS_MINIMIZE_SYSTRAY));
            MENU_SHOW_MAINWINDOW = GetTranslation(nameof(MENU_SHOW_MAINWINDOW));
            CLOSE_TAB = GetTranslation(nameof(CLOSE_TAB));
            CLOSE_ALL_TABS = GetTranslation(nameof(CLOSE_ALL_TABS));
            CLOSE_OTHER_TABS = GetTranslation(nameof(CLOSE_OTHER_TABS));
            LBL_NOT_CONTAINS = GetTranslation(nameof(LBL_NOT_CONTAINS));
            ASK_EDIT_FILE_NOW = GetTranslation(nameof(ASK_EDIT_FILE_NOW));
            ASK_MERGE_OR_REPLACE = GetTranslation(nameof(ASK_MERGE_OR_REPLACE));
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
