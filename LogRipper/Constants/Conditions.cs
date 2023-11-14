using System;

namespace LogRipper.Constants;

[Serializable()]
public enum Conditions
{
    CONTAINS = 0,
    START_WITH = 1,
    END_WITH = 2,
    REG_EX = 3,
    SCRIPT = 4,
}

internal static class ConditionsManager
{
    internal static Conditions ConditionStringToEnum(string condition)
    {
        if (condition == Locale.LBL_CONTAINS)
            return Conditions.CONTAINS;
        else if (condition == Locale.LBL_START_WITH)
            return Conditions.START_WITH;
        else if (condition == Locale.LBL_END_WITH)
            return Conditions.END_WITH;
        else if (condition == Locale.LBL_REG_EX)
            return Conditions.REG_EX;
        else if (condition == Locale.LBL_SCRIPT_CSHARP)
            return Conditions.SCRIPT;
        else
            throw new NotImplementedException();
    }

    internal static string ConditionEnumToString(Conditions conditions)
    {
        return conditions switch
        {
            Conditions.CONTAINS => Locale.LBL_CONTAINS,
            Conditions.START_WITH => Locale.LBL_START_WITH,
            Conditions.END_WITH => Locale.LBL_END_WITH,
            Conditions.REG_EX => Locale.LBL_REG_EX,
            Conditions.SCRIPT => Locale.LBL_SCRIPT_CSHARP,
            _ => throw new NotImplementedException(),
        };
    }
}
