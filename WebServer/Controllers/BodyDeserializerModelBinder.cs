using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Paccia;

namespace WebServer.Controllers
{
    public class BodyDeserializerModelBinder<T> : IModelBinder
    {
        // http://benfoster.io/blog/model-binder-dependency-injection-structuremap
        static readonly Lazy<ISerializer<T>> Serializer = new Lazy<ISerializer<T>>(() => new JsonSerializer<T>());

        public async Task BindModelAsync(ModelBindingContext context)
        {
            using (var memoryStream = await context.HttpContext.Request.Body.ToMemoryStreamAsync())
                context.Result = ModelBindingResult.Success(await Serializer.Value.DeserializeAsync(memoryStream));
        }
    }
}