using System;
using System.Composition;
using System.Composition.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace SourceMax.Web.IoC {

    /// <summary>
    /// A MEF-based implementation of the DefaultControllerFactory that is used by MVC 4
    /// to create Controllers for MVC functionality, but not ApiControllers.
    /// </summary>
    /// <remarks>
    /// This class is derived from Kenny Tordeur's implementation on his blog post at
    /// http://blogs.realdolmen.com/experts/2012/08/31/mef-in-asp-net-mvc-4-and-webapi/.
    /// However, his implementation uses "old" MEF instead of the light weight NuGet
    /// version here http://www.nuget.org/packages/Microsoft.Composition.
    /// </remarks>
    public class MefControllerFactory : DefaultControllerFactory {

        protected virtual CompositionHost CompositionHost { get; set; }

        public MefControllerFactory(CompositionHost compositionContainer) {

            this.CompositionHost = compositionContainer;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType) {

            var export = this.CompositionHost.GetExport(controllerType);

            IController result = null;

            if (null != export) {
                result = export as IController;
            }
            else {
                result = base.GetControllerInstance(requestContext, controllerType);
                this.CompositionHost.SatisfyImports(result);
            }

            return result;
        }
    }
}