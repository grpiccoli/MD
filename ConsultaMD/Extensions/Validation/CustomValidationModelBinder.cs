using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace ConsultaMD.Extensions.Validation
{
    public class CustomValidationModelBinder : IModelBinder
    {
        private readonly IModelBinder _underlyingModelBinder;

        public CustomValidationModelBinder(IModelBinder underlyingModelBinder)
        {
            _underlyingModelBinder = underlyingModelBinder;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Perform model binding using original model binder
            await _underlyingModelBinder.BindModelAsync(bindingContext).ConfigureAwait(false);

            // If model binding failed don't continue
            if (bindingContext?.Result.Model == null)
            {
                return;
            }

            // Perform some additional work after model binding occurs but before validation is executed.
            // i.e. fetch some additional data to be used by validation
        }
    }
}
