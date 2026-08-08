using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WineCellar.Validation;

public class NotaValidationAttribute : ValidationAttribute
{
    public NotaValidationAttribute()
        : base("Informe uma nota válida de 0 a 10.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is not string texto || string.IsNullOrWhiteSpace(texto))
            return false;

        var normalizado = texto.Replace(',', '.');

        if (!double.TryParse(normalizado, NumberStyles.Float, CultureInfo.InvariantCulture, out var nota))
            return false;

        return nota is >= 0 and <= 10;
    }
}