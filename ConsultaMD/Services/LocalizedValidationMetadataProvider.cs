using ConsultaMD.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class LocalizedValidationMetadataProvider : IValidationMetadataProvider
    {
        public LocalizedValidationMetadataProvider()
        {
        }
        private readonly ResourceManager resourceManager;
        private readonly Type resourceType;
        public LocalizedValidationMetadataProvider(string baseName, Type type)
        {
            resourceType = type;
            resourceManager = new ResourceManager(baseName,
                type.GetTypeInfo().Assembly);
        }
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context.Key.ModelType.GetTypeInfo().IsValueType && 
                context.ValidationMetadata.ValidatorMetadata
                .Where(m => m.GetType() == typeof(RequiredAttribute)).Count() == 0)
                context.ValidationMetadata.ValidatorMetadata.Add(new RequiredAttribute());
            foreach (var attribute in context.ValidationMetadata.ValidatorMetadata)
            {
                var tAttr = attribute as ValidationAttribute;
                if (tAttr?.ErrorMessage == null && tAttr?.ErrorMessageResourceName == null)
                {
                    var name = tAttr.GetType().Name;
                    if (ValidationMessages.ResourceManager.GetString(name) != null)
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
