using System;

namespace LogRipper.Constants;

[Serializable()]
public enum ECurrentSearchMode
{
    None = 0,
    BY_STRING = 1,
    BY_RULES = 2,
}
