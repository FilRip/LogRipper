using System;
using System.Collections.Generic;
using System.IO;

namespace LogRipper.Constants
{
    internal static class Locale
    {
        #region Fields

        internal static string Language { get; set; } = "fr-FR";
        internal static List<string> ListConditions { get; set; }
        private static string[] _languageFile;

        #endregion

        #region Translation
        public static string LBL_CONTAINS { get; set; } = "Contains";
        public static string LBL_START_WITH { get; set; } = "Start with";
        public static string LBL_END_WITH { get; set; } = "End with";
        public static string LBL_REG_EX { get; set; } = "Regular expression";
        public static string LBL_SCRIPT_CSHARP { get; set; } = "Script C#";
        public static string MENU_FILE { get; set; } = "_File";
        public static string MENU_OPEN { get; set; } = "_Open";
        public static string MENU_MERGE { get; set; } = "_Merge";
        public static string MENU_EXPORT_HTML { get; set; } = "_Export to HTML";
        public static string MENU_EXIT { get; set; } = "_Quit";
        public static string MENU_RULES { get; set; } = "_Rules";
        public static string MENU_ADD_RULE { get; set; } = "_Add a rule";
        public static string MENU_MANAGE_RULES { get; set; } = "_Manage rules";
        public static string MENU_SAVE_RULES { get; set; } = "_Save rules";
        public static string MENU_LOAD_RULES { get; set; } = "_Load rules";
        public static string MENU_ENCODING { get; set; } = "_Encoding";
        public static string MENU_ENC_DEFAULT { get; set; } = "_Default";
        public static string MENU_ENC_ASCII { get; set; } = "_ASCII";
        public static string MENU_ENC_UTF7 { get; set; } = "UTF-_7";
        public static string MENU_ENC_UTF8 { get; set; } = "UTF-_8";
        public static string MENU_ENC_UNICODE { get; set; } = "_Unicode";
        public static string MENU_EDIT { get; set; } = "_Edit";
        public static string MENU_COPY { get; set; } = "_Copy selection to clipboard";
        public static string MENU_SEARCH { get; set; } = "_Search...";
        public static string MENU_GOTO_LINE { get; set; } = "_Goto line";
        public static string MENU_SHOW { get; set; } = "_View";
        public static string MENU_SHOW_NUMLINE { get; set; } = "View line _number";
        public static string MENU_SHOW_FILENAME { get; set; } = "View _file name";
        public static string MENU_SHOW_SEARCHRESULT { get; set; } = "View search _result";
        public static string MENU_ENC_UTF32 { get; set; } = "UTF-_32";
        public static string BTN_ADD_RULE { get; set; } = "Add";
        public static string BTN_CANCEL_RULE { get; set; } = "Cancel";
        public static string BTN_EDIT_RULE { get; set; } = "Modify";
        public static string BTN_OK { get; set; } = "OK";
        public static string BTN_CANCEL { get; set; } = "Cancel";
        public static string LBL_ASK_DATEFORMAT { get; set; } = "DateTime format to use : ";
        public static string LBL_MERGE_FILENAME { get; set; } = "Filename to add : ";
        public static string LBL_DEFAULT_TEXT_COLOR { get; set; } = "Default color of text : ";
        public static string LBL_DEFAULT_BACK_COLOR { get; set; } = "Default color of background : ";
        public static string LBL_LIST_RULES { get; set; } = "List rules : ";
        public static string BTN_DEL_RULE { get; set; } = "Delete";
        public static string BTN_SEE_RULE_RESULT { get; set; } = "See lines of this rule";
        public static string TITLE_NEW_RULE { get; set; } = "New rule";
        public static string TITLE_EDIT_RULE { get; set; } = "Modify rule";
        public static string LBL_RULE_CONDITION { get; set; } = "Conditions : ";
        public static string LBL_RULE_TEXT { get; set; } = "Text : ";
        public static string LBL_RULE_TEXT_COLOR { get; set; } = "Color of text : ";
        public static string LBL_RULE_BACK_COLOR { get; set; } = "Background color : ";
        public static string LBL_RULE_CASE_SENSITIVE { get; set; } = "Case sensitive";
        public static string LBL_RULE_HIDE_LINE { get; set; } = "Hide lines : ";
        public static string LBL_RULE_SCRIPT_CS { get; set; } = "C# Script : ";
        public static string HELP_SCRIPT_CS { get; set; } = "You must return a boolean : true to apply this rule. You can access the current line by using 'line' variable that is already set in input";
        public static string LBL_RULE_REGEX { get; set; } = "Regular expression : ";
        public static string LBL_RULE_REVERSE { get; set; } = "Reserve the test of this rule : ";
        public static string LBL_RULE_DESCRIPTION { get; set; } = "Description : ";
        public static string LBL_RULE_PRIORITY { get; set; } = "Priority : ";
        public static string TITLE_LIST_RULES { get; set; } = "Manage rules";
        public static string TITLE_MERGE_FILE { get; set; } = "Merge with another file";
        public static string LBL_SEARCH_RESULT { get; set; } = "Search result for : ";
        public static string LBL_RESULT { get; set; } = "{0} (occurence(s) found)";
        public static string MENU_SETTINGS { get; set; } = "_Settings";
        public static string MENU_OPTIONS { get; set; } = "_Options";
        public static string TITLE_SEARCH { get; set; } = "Search";
        public static string LBL_SEARCH_TEXT { get; set; } = "String to search : ";
        public static string TITLE_GOTO { get; set; } = "Go to line number";
        public static string LBL_GOTO_LINE { get; set; } = "Number of line to reach : ";
        public static string ERROR_SAVE_RULES { get; set; } = "Error when try to save list of rules";
        public static string ERROR_LOAD_RULES { get; set; } = "Error when try to load list of rules";
        public static string TITLE_ERROR { get; set; } = "Error";
        public static string ERROR_EMPTY_FILE { get; set; } = "The content of file seems empty";
        public static string ERROR_DATEFORMAT { get; set; } = "Error during try to parse date at start of line";
        public static string ASK_CONFIRM_DEL { get; set; } = "Are you sur you want to delete this rule : ";
        public static string TITLE_CONFIRM_DEL { get; set; } = "Confirm";
        public static string LBL_SEARCH_CASE_SENSITIVE { get; set; } = "Case sensitive";
        public static string ERROR_READING_FILE { get; set; } = "Unable to read the file\r\n{0}";
        public static string ERROR_FILE_ALREADY_LOADED { get; set; } = "This file is already loaded";
        public static string MENU_ENABLE_AUTO_RELOAD { get; set; } = "Enable auto relad file when change";
        public static string MENU_ENABLE_AUTO_SCROLL { get; set; } = "Enable auto scroll to end of file";
        public static string OPTION_LANGUAGE { get; set; } = "Select current language : ";
        public static string RESET_WINDOW_POS { get; set; } = "Reset saved position of windows";
        public static string OPTION_THEME { get; set; } = "Select your theme : ";
        public static string THEME_LIGHT { get; set; } = "Light";
        public static string THEME_DARK { get; set; } = "Dark";
        public static string MENU_SHOW_MARGIN { get; set; } = "Show preview in right margin";
        public static string MENU_SHOW_TOOLBAR { get; set; } = "Show quick toolbar";
        public static string TOOLBAR_LIST_FILES { get; set; } = "Loaded file(s) : ";
        public static string TOOLBAR_LIST_RULES { get; set; } = "Loaded rule(s) : ";
        public static string MENU_SAVE_STATE { get; set; } = "Save full current state";
        public static string MENU_LOAD_STATE { get; set; } = "Load a full state";
        public static string ERROR_NO_CONDITION { get; set; } = "You must choice a condition";
        public static string ERROR_REGEX { get; set; } = "Your regular expression doesn't seems to be correct";
        public static string ERROR_SCRIPT { get; set; } = "You C# Script seems to have error";
        public static string ERROR_TEXT { get; set; } = "You don't have type some text for condition";
        public static string ERROR_SELECT_RULE { get; set; } = "You must select one or more rules first";
        public static string PREVIEW_COLOR { get; set; } = "Preview colors";
        public static string EXPLORER_OPEN_WITH { get; set; } = "Open with LogRipper";
        public static string SETTINGS_ADD_EXPLORER { get; set; } = "Add LogRipper to contextal menu of file explorer";
        public static string SETTINGS_REMOVE_EXPLORER { get; set; } = "Remove LogRipper from contextual menu of file explorer";
        public static string MENU_ABOUT { get; set; } = "About...";
        public static string TITLE_SETTINGS { get { return MENU_SETTINGS.Replace("_", ""); } }
        public static string ABOUT { get; set; } = @"This software is under CC BY NC licence.\r\nCreative Commons Attribution No Commerciale.\r\nhttps://creativecommons.org/licenses/by-nc/4.0/deed.frIt can't be sold.\r\n\r\nVisit official website : http://www.github.com/FilRip/LogRipper\r\n\r\nSource code can be reused, except in a paid software.\r\n\r\nThis software use the following librairies :\r\n- ModernWpfUi,\r\n- Extended.Wpf.ToolKit,\r\n- SharpZipLib.";
        public static string LBL_RULE_CATEGORY { get; set; } = "Category : ";
        public static string OPTIONS_DEFAULT_RULES { get; set; } = "List of rules to load at start : ";
        public static string OPTIONS_FONT_SIZE { get; set; } = "Default font size of lines : ";
        public static string OPTIONS_SPACE_SIZE { get; set; } = "Default space between lines : ";
        public static string SETTINGS_AUTO_SHOW_MARGIN { get; set; } = "Auto show right margin at start";
        public static string SETTINGS_AUTO_SHOW_TOOLBAR { get; set; } = "Auto show toolbar at start";
        public static string TOOLBAR_LIST_CATEGORY { get; set; } = "Category(ies) present :";
        public static string RELOAD_FILE { get; set; } = "Reload the file now";
        public static string REMOVE_FILE { get; set; } = "Remove this file from list";
        public static string CONFIRM_REMOVE_FILE { get; set; } = "Are you sure you want to remove this file ?";
        public static string LBL_DATE_FILTER { get; set; } = "Filter by date : ";
        public static string LBL_DATE_START_FILTER { get; set; } = "From : ";
        public static string LBL_DATE_END_FILTER { get; set; } = "To : ";
        public static string MENU_SHOW_DATE_FILTER { get; set; } = "Show filter by date";
        public static string SETTINGS_AUTO_SHOW_DATE_FILTER { get; set; } = "Auto show filter by date at start";
        public static string SETTINGS_AUTO_FOLLOW_MARGIN { get; set; } = "Auto follow the main content in the margin";
        public static string HIDE_ALL_OTHERS_LINES { get; set; } = "Hide all others lines";
        public static string LBL_RULE_BOLD { get; set; } = "Set bold";
        public static string LBL_RULE_ITALIC { get; set; } = "Set italic";
        public static string SETTINGS_WRAP { get; set; } = "Wrap lines";
        public static string CONCATENATION_AND { get; set; } = "AND";
        public static string CONCATENATION_OR { get; set; } = "OR";
        public static string MENU_RELOAD_FILES { get; set; } = "Reload all files now";
        public static string ERROR_SAVE_FILE { get; set; } = "Error during trying to save the file";
        public static string MENU_SAVE_SEARCH_RESULT { get; set; } = "Save this search result";
        public static string MENU_MERGE_RULES { get; set; } = "Merge with another list of saved rules";
        public static string ERROR_NO_RULE_FOUND { get; set; } = "No rules found in this file";
        public static string LBL_CONCATENATION_RULE { get; set; } = "Concatenation operator : ";
        public static string ADD_SUB_RULE { get; set; } = "Add condition";
        public static string REMOVE_SUB_RULE { get; set; } = "Remove condition";
        public static string EDIT_SUB_RULE { get; set; } = "Modify condition";
        public static string LBL_SUB_RULES { get; set; } = "More Conditions :";
        public static string LBL_FONT_PROPERTIES { get; set; } = "Font properties :";
        public static string HELP_PRIORITY { get; set; } = "Greater value for most priority than other. Negative value available";
        public static string LBL_NEW_VERSION { get; set; } = "A new version ({0}) is available. Do you want to install it now ?";
        public static string LBL_ERROR_EXTRACT_NEW_VERSION { get; set; } = "Error during extraction of zip file containing the new version";
        public static string LBL_ERROR_INSTALL_NEW_VERSION { get; set; } = "Error during installing the new version";
        public static string LBL_ERROR_DOWNLOAD_NEW_VERSION { get; set; } = "Error during downloading the new version";
        public static string LBL_RESTART_APP_NEW_VERSION { get; set; } = "New version has been installed. You must restart to change take effect. Do you want to restart now ?";
        public static string LBL_VERSION_COMPARE { get; set; } = "Your version : {0}\r\nAvailable version : {1}";
        public static string MENU_CHECK_NEW_VERSION { get; set; } = "Check for update now";
        public static string ERROR_CHECK_NEW_VERSION { get; set; } = "Unable to check if new version is available\r\nBe sure you're connected to Internet";
        #endregion

        #region Methods

        internal static void Init()
        {
            Language = Properties.Settings.Default.Language;
            string filename = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "Languages", Language + ".ini");
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
            }
            ABOUT = ABOUT.Replace(@"\r\n", Environment.NewLine);
            ABOUT = ABOUT.Replace(@"\r", Environment.NewLine);
            ListConditions = new List<string>()
            {
                LBL_CONTAINS,
                LBL_START_WITH,
                LBL_END_WITH,
                LBL_REG_EX,
                LBL_SCRIPT_CSHARP,
            };
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
}
