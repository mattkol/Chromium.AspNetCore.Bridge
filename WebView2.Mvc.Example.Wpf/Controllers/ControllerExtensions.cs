using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebView2.Mvc.Example.Wpf
{
    public static class ControllerExtensions
    {
        // https://stackoverflow.com/questions/40912375/return-view-as-string-in-net-core
        public static void WriteViewToHtmlFile<TModel>(this Controller controller, ViewResult result, bool partial = false)
        {
            var viewName = result.ViewName;
            var model = result.Model;

            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

                if (viewResult.Success == false)
                {
                    return; // $"A view with the name {viewName} could not be found";
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                var task = Task.Run(async () => await viewResult.View.RenderAsync(viewContext));
                task.Wait();

                var htmlFileName = viewName + ".html";
                var filePath = Path.Combine(@"C:\ChromelyDlls\html", htmlFileName);

                var htmlString = writer.GetStringBuilder().ToString();
              //  System.IO.File.WriteAllText(htmlFileName, htmlString);
                System.IO.File.WriteAllText(filePath, htmlString);
            }
        }
    }
}
