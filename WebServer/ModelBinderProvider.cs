using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebServer
{
    public class ModelBinderProvider : IModelBinderProvider
    {
        readonly IModelBinder _binder;

        public ModelBinderProvider(IModelBinder binder)
        {
            _binder = binder;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context) => 
            context.BindingInfo.BinderType == _binder.GetType() ? 
            _binder : 
            TryAndCreate(context.BindingInfo.BinderType);

        static IModelBinder TryAndCreate(Type type) => 
            type == null ?
            null :
            Activator.CreateInstance(type) as IModelBinder;

        public static IModelBinderProvider From(IModelBinder binder) => new ModelBinderProvider(binder);
    }
}