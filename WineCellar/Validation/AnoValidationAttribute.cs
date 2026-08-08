using System.ComponentModel.DataAnnotations;

namespace WineCellar.Validation;

public class AnoValidationAttribute : ValidationAttribute
{
    public AnoValidationAttribute() : base("Informe um ano entre 1900 e o ano atual.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is not int ano)
            return false;

        return ano >= 1900 && ano <= DateTime.Now.Year;
    }
}