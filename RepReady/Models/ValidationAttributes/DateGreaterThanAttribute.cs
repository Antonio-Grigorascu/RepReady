using RepReady.Models;
using System;
using System.ComponentModel.DataAnnotations;

public class DateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (Exercise)validationContext.ObjectInstance;

        if (model.Start >= model.Finish)
        {
            return new ValidationResult("Data de sfârșit a exercițiului trebuie să fie mai mare decât data de început.");
        }

        return ValidationResult.Success;
    }
}