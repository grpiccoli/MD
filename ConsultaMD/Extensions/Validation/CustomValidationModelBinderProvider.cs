using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Extensions.Validation
{
    public class CustomValidationModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinderProvider _underlyingModelBinderProvider;

        public CustomValidationModelBinderProvider(IModelBinderProvider underlyingModelBinderProvider)
        {
            _underlyingModelBinderProvider = underlyingModelBinderProvider;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var underlyingModelBinderProvider = _underlyingModelBinderProvider.GetBinder(context);
            return new CustomValidationModelBinder(underlyingModelBinderProvider);
        }
    }
}
