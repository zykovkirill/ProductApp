//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
//using ProductApp.Shared.Models;
//using System;
//using System.IO;
//using System.Threading.Tasks;

//namespace ProductApp.Server.BindModels
//{
//    public class StreamBinder : IModelBinder
//    {
//        public Task BindModelAsync(ModelBindingContext bindingContext)
//        {
//            bindingContext.Result = ModelBindingResult.Success(bindingContext.HttpContext.Request.Body);
//            return Task.CompletedTask;
//        }
//    }

//    public class StreamBinderProvider : IModelBinderProvider
//    {
//        public IModelBinder GetBinder(ModelBinderProviderContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            if (context.Metadata.ModelType == typeof(ProductRequestClient))
//            {
//                return new BinderTypeModelBinder(typeof(StreamBinder));
//            }

//            return null;
//        }
//    }
//}
