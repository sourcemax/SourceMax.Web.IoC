using System;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;

using SourceMax.Web.IoC;

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.MefConfig), "Initialize")]

namespace $rootnamespace$ {

    public class MefConfig {
	    
        /// <summary>
        /// This method provides the entry point that is called by WebActivator based on the assembly directive above.
        /// This method is responsible for bootstrapping the Mef configuration based on the assemblies and conventions
        /// provided in the methods below.
        /// </summary>
        public static void Initialize() {

            // Get an array of assemblies that will be providing the import and export functionality
            var appAssemblies = GetAppAssemblies();

            // Get the conventions that will be used to find the imports and exports within the application
            var conventions = GetConventions();

            // Use the assemblies and conventions to create the dependency container
            var container = GetContainer(appAssemblies, conventions);

            // Create and apply a MefDependencyResolver so ApiControllers can be composed
            GlobalConfiguration.Configuration.DependencyResolver = new MefDependencyResolver(container);

            // Create and apply a MefControllerFactory so (non-WebApi) controllers can be composed
            ControllerBuilder.Current.SetControllerFactory(new MefControllerFactory(container));
        }


        /// <summary>
        /// This method is responsible for compiling an array of assemblies that house the imports
        /// and exports that the system will need to resolve.
        /// </summary>
        /// <returns>An array of Assembly objects.</returns>
        private static Assembly[] GetAppAssemblies() {

            var appAssemblies = new Assembly[] { Assembly.GetExecutingAssembly() };

            return appAssemblies;
        }


        /// <summary>
        /// This method is responsible for building up the conventions that can be used to resolve
        /// import and export requests for the system. For example, all Controllers (that inherit
        /// from IController) will be automatically exported.
        /// </summary>
        /// <returns>A ConventionBuilder object containing all of the conventions.</returns>
        private static ConventionBuilder GetConventions() {

            var conventions = new ConventionBuilder();

            // TODO: Customize or add more conventions here as necessary
            conventions.ForTypesDerivedFrom<IHttpController>().Export();

            conventions.ForTypesDerivedFrom<IController>().Export();

            conventions.ForTypesMatching(t => t.Namespace != null && t.Namespace.EndsWith(".Parts"))
                .Export()
                .ExportInterfaces();

            return conventions;
        }


        /// <summary>
        /// This method uses the assemblies and conventions to create the composition container that will
        /// be used by the system for composition.
        /// </summary>
        /// <param name="assemblies">An array of assemblies that are used to supply imports and exports.</param>
        /// <param name="conventions">A set of conventions to be used to automatically import and export.</param>
        /// <returns>A Mef-based Composition host that is ready to resolve composition requests.</returns>
        private static CompositionHost GetContainer(Assembly[] assemblies, ConventionBuilder conventions) {

            var container = new ContainerConfiguration()
                .WithAssemblies(assemblies, conventions)
                .CreateContainer();

            return container;
        }
    }
}