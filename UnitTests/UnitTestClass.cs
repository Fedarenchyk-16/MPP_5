using DependencyInjectionContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class UnitTestClass
    {
        public interface ISimpleInterface {}
        public class SimpleClass : ISimpleInterface
        {
            int a = 0;
            bool b = false;

            public override bool Equals(object obj)
            {
                if (obj is SimpleClass sd)
                    return a == sd.a && b == sd.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }

        public interface ICollectionDependency { }
        public class CollectionDependency1 : ICollectionDependency
        {
            int a = 1;
            char b = 'a';
            public override bool Equals(object obj)
            {
                if (obj is CollectionDependency1 cc)
                    return a == cc.a && b == cc.b;
                return false;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }
        public class CollectionDependency2 : ICollectionDependency
        {
            int a = 2;
            char b = 'b';

            public override bool Equals(object obj)
            {
                if (obj is CollectionDependency2 cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }
        public class CollectionDependency3 : ICollectionDependency
        {
            int a = 3;
            char b = 'c';

            public override bool Equals(object obj)
            {
                if (obj is CollectionDependency3 cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }
        public interface ISingletonInterface { }
        public class SingletonClass : ISingletonInterface
        {
            int a = 3;
            char b = 'c';

            public override bool Equals(object obj)
            {
                if (obj is SingletonClass cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<ISimpleInterface, SimpleClass>(LifeType.InstancePerDependency);
            DependencyProvider container = new DependencyProvider(dependencyConfiguration);
            var actual = container.Resolve<ISimpleInterface>();
            var expected = new SimpleClass();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestMethod2()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<ICollectionDependency, CollectionDependency1>(LifeType.InstancePerDependency);
            dependencyConfiguration.Register<ICollectionDependency, CollectionDependency2>(LifeType.InstancePerDependency);
            dependencyConfiguration.Register<ICollectionDependency, CollectionDependency3>(LifeType.InstancePerDependency);
            DependencyProvider container = new DependencyProvider(dependencyConfiguration);
            var actual = (List<ICollectionDependency>)container.ResolveAll<ICollectionDependency>();
            var expected = new List<ICollectionDependency>();
            expected.Add(new CollectionDependency1());
            expected.Add(new CollectionDependency2());
            expected.Add(new CollectionDependency3());
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
        [TestMethod]
        public void TestMethod3()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<ISingletonInterface, SingletonClass>(LifeType.Singleton);
            DependencyProvider container = new DependencyProvider(dependencyConfiguration);
            var expected = Task.Run(() => container.Resolve<ISingletonInterface>());
            var actual1 = Task.Run(() => container.Resolve<ISingletonInterface>());
            var actual2 = Task.Run(() => container.Resolve<ISingletonInterface>());
            var actual3 = Task.Run(() => container.Resolve<ISingletonInterface>());

            Assert.AreEqual(expected.Result, actual1.Result);
            Assert.AreEqual(expected.Result, actual2.Result);
            Assert.AreEqual(expected.Result, actual3.Result);
        }
    }
}
