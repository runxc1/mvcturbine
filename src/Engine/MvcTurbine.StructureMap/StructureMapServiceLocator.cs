namespace MvcTurbine.StructureMap {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ComponentModel;
    using global::StructureMap;

    /// <summary>
    /// Default implementation of the <seealso cref="IServiceLocator"/> contract with Ninject IoC.
    /// </summary>
    /// <remarks>
    /// To learn more about StructureMap, please visit its website: http://structuremap.sourceforge.net
    /// </remarks>
    [Serializable]
    public class StructureMapServiceLocator : IServiceLocator, IServiceInjector {
        private TurbineRegistry currentRegistry;
        private static bool isDisposing;

        /// <summary>
        /// Creates an instance with an empty <seealso cref="IContainer"/> instance.
        /// </summary>
        public StructureMapServiceLocator()
            : this(ObjectFactory.Container) {
        }

        /// <summary>
        /// Creates an instance of the locator with the specified <see cref="IContainer"/> instance.
        /// </summary>
        /// <param name="container">Instance of <see cref="IContainer"/> to use with this locator.</param>
        public StructureMapServiceLocator(IContainer container) {
            Container = container;
        }

        /// <summary>
        /// Gets the <see cref="IContainer"/> associated with this instance.
        /// </summary>
        public IContainer Container { private set; get; }

        public IList<object> ResolveServices(Type type) {
            var results = Container.GetAllInstances(type);
            return results.Cast<object>().ToList();
        }

        public IServiceRegistrar Batch() {
            currentRegistry = new TurbineRegistry(Container);
            return currentRegistry;
        }

        /// <summary>
        /// Resolves the service of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of service to resolve.</typeparam>
        /// <returns>An instance of the type, null otherwise.</returns>
        public T Resolve<T>() where T : class {
            try {
                return Container.GetInstance<T>();
            }
            catch (Exception ex) {
                throw new ServiceResolutionException(typeof(T), ex);
            }
        }

        /// <summary>
        /// Resolves the service of the specified type by the given string key.
        /// </summary>
        /// <typeparam name="T">Type of service to resolve.</typeparam>
        /// <param name="key">Unique key to distinguish the service.</param>
        /// <returns>An instance of the type, null otherwise.</returns>
        public T Resolve<T>(string key) where T : class {
            try {
                return Container.GetInstance<T>(key);
            }
            catch (Exception ex) {
                throw new ServiceResolutionException(typeof(T), ex);
            }
        }

        /// <summary>
        /// Resolves the service of the specified type by the given type key.
        /// </summary>
        /// <typeparam name="T">Type of service to resolve.</typeparam>
        /// <param name="type">Key type of the service.</param>
        /// <returns>An instance of the type, null otherwise.</returns>
        public T Resolve<T>(Type type) where T : class {
            try {
                return Container.GetInstance(type) as T;
            }
            catch (Exception ex) {
                throw new ServiceResolutionException(typeof(T), ex);
            }
        }

        ///<summary>
        /// Resolves the service of the specified type by the given type key.
        ///</summary>
        ///<param name="type">Type of service to resolve.</param>
        ///<returns>An instance of the type, null otherwise</returns>
        public object Resolve(Type type) {
            try {
                return Container.GetInstance(type);
            }
            catch (Exception ex) {
                throw new ServiceResolutionException(type, ex);
            }
        }

        /// <summary>
        /// Resolves the list of services of type <see cref="T"/> that are registered 
        /// within the locator.
        /// </summary>
        /// <typeparam name="T">Type of the service to resolve.</typeparam>
        /// <returns>A list of service of type <see cref="T"/>, null otherwise.</returns>
        public IList<T> ResolveServices<T>() where T : class {
            return Container.GetAllInstances<T>();
        }

        /// <summary>
        /// Registers the implemation type, <paramref name="implType"/>, with the locator under
        /// the <see cref="Interface"/> service type.
        /// </summary>
        /// <typeparam name="Interface">Type of the service to register.</typeparam>
        /// <param name="implType">Implementation type to use for registration.</param>
        public void Register<Interface>(Type implType) where Interface : class {
            currentRegistry.Register<Interface>(implType);
        }

        /// <summary>
        /// Registers the implemation type, <see cref="Implementation"/>, with the locator under
        /// the <see cref="Interface"/> service type.
        /// </summary>
        /// <typeparam name="Interface">Type of the service to register.</typeparam>
        /// <typeparam name="Implementation">Implementation type to use for registration.
        /// </typeparam>
        public void Register<Interface, Implementation>() where Implementation : class, Interface {
            currentRegistry.Register<Interface, Implementation>();
        }

        /// <summary>
        /// Registers the implemation type, <see cref="Implementation"/>, with the locator under
        /// the <see cref="Interface"/> service type.
        /// </summary>
        /// <typeparam name="Interface">Type of the service to register.</typeparam>
        /// <typeparam name="Implementation">Implementation type to use for registration.
        /// </typeparam>
        /// <param name="key">Unique key to distinguish the service.</param>
        public void Register<Interface, Implementation>(string key) where Implementation : class, Interface {
            currentRegistry.Register<Interface, Implementation>(key);
        }

        /// <summary>
        /// Registers the implementation type, <paramref name="type"/>, with the locator
        /// by the given string key.
        /// </summary>
        /// <param name="key">Unique key to distinguish the service.</param>
        /// <param name="type">Implementation type to use.</param>
        public void Register(string key, Type type) {
            currentRegistry.Register(key, type);
        }

        /// <summary>
        /// Registers the implementation type, <paramref name="implType"/>, with the locator
        /// by the given service type, <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Type of the service to register.</param>
        /// <param name="implType">Implementation to associate with the service.</param>
        public void Register(Type serviceType, Type implType) {
            currentRegistry.Register(serviceType, implType);
        }

        /// <summary>
        /// Registers the implementation type, <paramref name="implType"/>, with the locator
        /// by the given service type, <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Type of the service to register.</param>
        /// <param name="implType">Implementation to associate with the service.</param>
        /// <param name="key"></param>
        public void Register(Type serviceType, Type implType, string key) {
            currentRegistry.Register(serviceType, implType, key);
        }

        /// <summary>
        /// See <see cref="IServiceLocator.Register{Interface}(Interface)"/>.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <param name="instance"></param>
        public void Register<Interface>(Interface instance) where Interface : class {
            currentRegistry.Register(instance);
        }

        /// <summary>
        /// Resolves the service of the specified interface with the provided factory method.
        /// </summary>
        /// <param name="factoryMethod">The factory method which will be used to resolve this interface.</param>
        /// <returns>An instance of the type, null otherwise</returns>
        public void Register<Interface>(Func<Interface> factoryMethod) where Interface : class {
            currentRegistry.Register(factoryMethod);
        }


        public TService Inject<TService>(TService instance) where TService : class {
            if (instance == null)
                return null;

            // Honor SM's configuration, if any
            Container.BuildUp(instance);

            // Go through all properties and resolve them if any
            Type instanceType = instance.GetType();
            instanceType.GetProperties()
                .Where(property => property.CanWrite && Container.Model.HasImplementationsFor(property.PropertyType))
                .ForEach(property => property.SetValue(instance, Container.GetInstance(property.PropertyType), null));

            return instance;
        }

        public void TearDown<TService>(TService instance) where TService : class {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() {
            if (isDisposing) return;
            if (Container == null) return;

            isDisposing = true;
            Container.Dispose();
            Container = null;
        }
    }
}
