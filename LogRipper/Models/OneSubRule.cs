using System;
using System.Xml.Serialization;

using LogRipper.Constants;

namespace LogRipper.Models
{
    [XmlRoot()]
    public class OneSubRule : RuleViewModelBase
    {
        [XmlElement()]
        public Concatenation Concatenation { get; set; }

        internal override bool AreSame(RuleViewModelBase baseRule)
        {
            OneSubRule rule = (OneSubRule)baseRule;
            if (!base.AreSame(rule))
                return false;
            if (rule.Concatenation != Concatenation)
                return false;
            return true;
        }

        public override string ToString()
        {
            return (" " + (Concatenation == Concatenation.AND ? Locale.CONCATENATION_AND : Locale.CONCATENATION_OR)) + " " + ConditionsManager.ConditionEnumToString(Conditions) + Text.Split(Environment.NewLine.ToCharArray())[0];
        }

        internal override void Refresh()
        {
            base.Refresh();
            OnPropertyChanged(nameof(Concatenation));
        }
    }
}
