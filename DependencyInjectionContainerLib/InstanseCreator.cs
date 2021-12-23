using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjectionContainer
{
    public static class InstanceCreator
    {
        public static object GetInstance(Dependency dependency, DependencyConfiguration dependencyConfiguration)
        {
            var provider = new DependencyProvider(dependencyConfiguration);
            ConstructorInfo[] constructors = dependency.type.GetConstructors();
            ConstructorInfo constructor = constructors[0];
            ParameterInfo[] constructorParams = constructor.GetParameters();
            object[] invokeArgs = new object[constructorParams.Length];

            for (int i = 0; i < constructorParams.Length; i++)
            {
                if (constructorParams[i].ParameterType.IsValueType)
                    throw new Exception("Implementation constructor takes invalid parameters.");

                invokeArgs[i] = provider.Resolve(constructorParams[i].ParameterType);
            }
            object result = Activator.CreateInstance(dependency.type, invokeArgs);
            return result;
        }
    }
}
