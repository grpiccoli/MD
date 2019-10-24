using ConsultaMD.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class LocalizedValidationMetadataProvider : IValidationMetadataProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly Type _resourceType;
        public LocalizedValidationMetadataProvider()
        {
        }
        public LocalizedValidationMetadataProvider(string baseName, Type resourceType)
        {
            _resourceType = resourceType;
            _resourceManager = new ResourceManager(baseName,
                resourceType.GetTypeInfo().Assembly);
        }
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context != null && context.Key.ModelType.GetTypeInfo().IsValueType &&
                !context.ValidationMetadata.ValidatorMetadata
                .Where(m => m.GetType() == typeof(RequiredAttribute)).Any())
                context.ValidationMetadata.ValidatorMetadata.Add(new RequiredAttribute());
            foreach (var attribute in context?.ValidationMetadata.ValidatorMetadata)
            {
                var tAttr = attribute as ValidationAttribute;
                if (tAttr?.ErrorMessage == null && tAttr?.ErrorMessageResourceName == null)
                {
                    var name = tAttr.GetType().Name;
                    if (ValidationMessages.ResourceManager.GetString(name, CultureInfo.InvariantCulture) != null)
                    {
                        tAttr.ErrorMessageResourceType = typeof(ValidationMessages);
                        tAttr.ErrorMessageResourceName = name;
                        tAttr.ErrorMessage = null;
                    }
                }
            }
        }
    }
}
