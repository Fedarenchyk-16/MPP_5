using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionContainer
{
    public class DependencyProvider//контейнер
    {
        DependencyConfiguration dependencyConfiguration;
        public DependencyProvider(DependencyConfiguration configuration)//передаем конфигурацию
        {
            dependencyConfiguration = configuration;
        }
        public TInterface Resolve<TInterface>()
            where TInterface : class
        {
            return (TInterface)Resolve(typeof(TInterface));
        }

        public IEnumerable<T> ResolveAll<T>()
            where T : class
        {
            return (IEnumerable<T>)ResolveAll(typeof(T));
        }

        public IEnumerable<object> ResolveAll(Type dependencyType)
        {
            if (dependencyConfiguration.GetAllDependencies(dependencyType, out var dependencies))
            {
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dependencyType));

                foreach (var dependency in dependencies)
                {
                    list.Add(ResolveDependency(dependency));
                }

                return (IEnumerable<object>)list;
            }

            return null;
        }

        private object ResolveDependency(Dependency dependency)
        {
            object result = null;
            if (dependency.lifeType == LifeType.InstancePerDependency)
            {
                result = InstanceCreator.GetInstance(dependency, dependencyConfiguration);
            }
            else if (dependency.lifeType == LifeType.Singleton)
            {
                lock (dependency)
                {
                    if (dependency.Instance == null)
                    {
                        result = InstanceCreator.GetInstance(dependency, dependencyConfiguration);
                        dependency.Instance = result;
                    }
                    else
                    {
                        result = dependency.Instance;
                    }
                }
            }

            return result;
        }

        private Dependency GetDependency(Type dependencyType)
        {
            if (dependencyType.IsGenericType &&
                dependencyConfiguration.GetDependency(dependencyType.GetGenericTypeDefinition(), out var genericDependency))
            {
                var genericType = genericDependency.type.MakeGenericType(dependencyType.GenericTypeArguments);
                if (genericDependency.Instance == null)
                {
                    genericDependency.Instance = InstanceCreator.GetInstance(genericDependency, dependencyConfiguration);
                }
                
                var tempGenericDependency = new Dependency(genericType, genericDependency.lifeType) { Instance = genericDependency.Instance };
                return tempGenericDependency;
            }
            if (dependencyConfiguration.GetDependency(dependencyType, out var dependency)) 
            {
                return dependency;
            }
            return null;
        }

        public object Resolve(Type dependencyType)
        {
            if (typeof(IEnumerable).IsAssignableFrom(dependencyType))
            {
                return ResolveAll(dependencyType.GetGenericArguments()[0]);
            }
            var dependency = GetDependency(dependencyType);

            return ResolveDependency(dependency);
        }
    }
}
