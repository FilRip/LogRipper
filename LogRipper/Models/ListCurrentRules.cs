using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace LogRipper.Models
{
    internal class ListCurrentRules
    {
        private ObservableCollection<OneRule> _listRules;

        public event EventHandler AddRuleEvent;
        public event EventHandler RemoveRuleEvent;
        public event EventHandler EditRuleEvent;

        internal ListCurrentRules()
        {
            _listRules = new();
        }

        public ObservableCollection<OneRule> ListRules
        {
            get { return _listRules; }
        }

        private IEnumerable<OneRule> ListActiveRules
        {
            get { return _listRules.Where(r => r.Active); }
        }

        internal void SetRules(ObservableCollection<OneRule> rules)
        {
            _listRules = rules;
            AddRuleEvent?.Invoke(rules[0], EventArgs.Empty);
        }

        internal void AddRule(OneRule rule)
        {
            if (_listRules.Any(r => r.AreSame(rule)))
                return;
            _listRules.Add(rule);
            AddRuleEvent?.Invoke(rule, EventArgs.Empty);
        }

        internal void RemoveRule(OneRule rule)
        {
            if (_listRules.Contains(rule))
            {
                _listRules.Remove(rule);
                RemoveRuleEvent?.Invoke(rule, EventArgs.Empty);
            }
        }

        internal void EditRule(OneRule rule)
        {
            EditRuleEvent?.Invoke(rule, EventArgs.Empty);
        }

        internal bool ExecuteRules(string line, DateTime dateline)
        {
            if (!string.IsNullOrWhiteSpace(line))
                return ListActiveRules.Any(r => r.Execute(line, dateline));
            return false;
        }

        internal SolidColorBrush ExecuteRulesForeground(string line, DateTime dateline)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;
            SolidColorBrush foreground = null;
            OneRule rule = ListActiveRules.OrderBy(r => r.Priority).LastOrDefault(r => r.Execute(line, dateline));
            if (rule != null)
                foreground = rule.ForegroundBrush;

            return foreground;
        }

        internal SolidColorBrush ExecuteRulesBackground(string line, DateTime dateline)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;
            SolidColorBrush background = null;
            OneRule rule = ListActiveRules.OrderBy(r => r.Priority).LastOrDefault(r => r.Execute(line, dateline));
            if (rule != null)
                background = rule.BackgroundBrush;

            return background;
        }

        internal bool ExecuteRulesBold(string line, DateTime dateline)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;
            OneRule rule = ListActiveRules.OrderBy(r => r.Priority).LastOrDefault(r => r.Execute(line, dateline));
            if (rule != null)
                return rule.Bold;

            return false;
        }

        internal bool ExecuteRulesItalic(string line, DateTime dateline)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;
            OneRule rule = ListActiveRules.OrderBy(r => r.Priority).LastOrDefault(r => r.Execute(line, dateline));
            if (rule != null)
                return rule.Italic;

            return false;
        }

        internal bool ExecuteRulesHideLine(string line, DateTime dateline)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;
            OneRule rule = ListActiveRules.OrderBy(r => r.Priority).LastOrDefault(r => r.HideLine && r.Execute(line, dateline));
            if (rule != null)
                return true;
            return false;
        }

        internal bool RuleExist(OneRule rule)
        {
            return _listRules.Any(r => r.AreSame(rule));
        }
    }
}
