using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;

namespace SourceMax.Web.IoC {

    /// <summary>
    /// A MEF-based implementation of the MefDependencyResolver that is used to
    /// to create WebApi controllers, but not regular MVC web controllers.
    /// </summary>
    /// <remarks>
    /// This class is derived from the MEF project documentaion on CodePlex here 
    /// http://mef.codeplex.com/wikipage?title=Standalone%20Web%20API%20dependency%20resolver%20using%20Microsoft.Composition
    /// </remarks>
    public class MefDependencyResolver : MefDependencyScope, IDependencyResolver {

        private readonly ExportFactory<CompositionContext> RequestScopeFactory;

        public MefDependencyResolver(CompositionHost rootCompositionScope) : base(new Export<CompositionContext>(rootCompositionScope, rootCompositionScope.Dispose)) {

            if (rootCompositionScope == null) {
                throw new ArgumentNullException("rootCompositionScope");
            }

            var factoryContract = new CompositionContract(typeof(ExportFactory<CompositionContext>), null, new Dictionary<string, object> {
                { "SharingBoundaryNames", new[] { "HttpRequest" } }
            });

            this.RequestScopeFactory = (ExportFactory<CompositionContext>)rootCompositionScope.GetExport(factoryContract);
        }

        public IDependencyScope BeginScope() {

            return new MefDependencyScope(this.RequestScopeFactory.CreateExport());
        }
    }
}
