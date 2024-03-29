﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Extensions.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class CellPhoneAttribute : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object value)
        {
            var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            var phone = phoneNumberUtil.Parse((string)value, "CL");
            var valid = phoneNumberUtil.IsPossibleNumberForType(phone, PhoneNumbers.PhoneNumberType.MOBILE);
            return valid;
        }
        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context?.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context?.ModelMetadata.GetDisplayName());
            MergeAttribute(context?.Attributes, "data-val-cell", errorMessage);
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
