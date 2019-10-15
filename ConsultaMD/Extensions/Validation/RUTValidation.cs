using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ConsultaMD.Extensions.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RUT : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object value)
        {
            var parts = Extensions.RUT.Unformat(value.ToString());
            return parts != null && parts.Value.rut > 900_000 && parts.Value.rut < 30_000_000;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-rut", errorMessage);
        }

        private bool MergeAttribute(
        IDictionary<string, string> attributes,
        string key,
        string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}
