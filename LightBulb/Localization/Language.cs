using System.ComponentModel.DataAnnotations;

namespace LightBulb.Localization;

public enum Language
{
    System,
    English,
    Ukrainian,
    German,
    French,
    Spanish,

    [Display(Name = "Simplified Chinese")]
    ChineseSimplified,
}
