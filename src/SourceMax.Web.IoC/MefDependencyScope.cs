using System;
using System.Collections.Generic;
using System.Composition;
using System.Web.Http.Dependencies;

namespace SourceMax.Web.IoC {

    /// <summary>
    /// A MEF-based implementation of the MefDependencyScope that is used to
    /// to create WebApi controllers, but not regular MVC web controllers.
    /// </summary>
    /// <remarks>
    /// This class is derived from the MEF project documentaion on CodePlex here 
    /// http://mef.codeplex.com/wikipage?title=Standalone%20Web%20API%20dependency%20resolver%20using%20Microsoft.Composition
    /// </remarks>
    public class MefDependencyScope : IDependencyScope {

        private readonly Export<CompositionContext> _compositionScope;

        public MefDependencyScope(Export<CompositionContext> compositionScope) {

            if (compositionScope == null) {
                throw new ArgumentNullException("compositionScope");
            }

            this._compositionScope = compositionScope;
        }

        public object GetService(Type serviceType) {

            if (serviceType == null) {
                throw new ArgumentNullException("serviceType");
            }

            object result;
            this.CompositionScope.TryGetExport(serviceType, null, out result);
            return result;
        }

        public IEnumerable<object> GetServices(Type serviceType) {

            if (serviceType == null) {
                throw new ArgumentNullException("serviceType");
            }

            return this.CompositionScope.GetExports(serviceType, null);
        }

        protected CompositionContext CompositionScope {
            get { return this._compositionScope.Value; }
        }

        public void Dispose() {
            this.Dispose(true);
        }

        protected void Dispose(bool disposing) {

            if (disposing) {
                this._compositionScope.Dispose();
            }
        }
    }
}
