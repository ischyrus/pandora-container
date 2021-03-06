/*
 * Copyright 2009 Daniel H�lbling - http://www.tigraine.at
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Pandora.Tests
{
    using Testclasses;
    using Xunit;

    public class ContainerFixture
    {
        [Fact]
        public void CanResolveClassWithoutDependencies()
        {
            var componentStore = new ComponentStore();
            componentStore.Add<IService, ClassWithNoDependencies>();

            var container = new PandoraContainer(componentStore);
            var result = container.Resolve<IService>();

            Assert.IsType<ClassWithNoDependencies>(result);
        }

        [Fact]
        public void CanResolveClassWithOneDependency()
        {
            IComponentStore componentStore = new ComponentStore();
            componentStore.Add<IService, ClassWithNoDependencies>();
            componentStore.Add<IService2, ClassWithOneDependency>();

            PandoraContainer locator = new PandoraContainer(componentStore);
            var result = locator.Resolve<IService2>();

            Assert.IsType<ClassWithOneDependency>(result);
        }

        [Fact]
        public void CanResolveClassWithMultipleDependencies()
        {
            var store = new ComponentStore();
            store.Add<IService, ClassWithNoDependencies>();
            store.Add<IService2, ClassWithOneDependency>();
            store.Add<IService3, ClassDependingOnClassWithOneDependency>();

            var container = new PandoraContainer(store);
            var result = container.Resolve<IService3>();

            Assert.IsType<ClassDependingOnClassWithOneDependency>(result);
        }

        [Fact]
        public void ThrowsExceptionIfTypeNotRegistered()
        {
            var store = new ComponentStore();

            var container = new PandoraContainer(store);

            Assert.Throws<ServiceNotFoundException>(() => {
                                                              var result = container.Resolve<IService>();
            });  
        }

        [Fact]
        public void ThrowsExceptionIfDependencyCouldNotBeSatisfied()
        {
            var store = new ComponentStore();
            store.Add<ClassWithOneDependency, ClassWithOneDependency>();
            var container = new PandoraContainer(store);

            Assert.Throws<DependencyMissingException>(() => container.Resolve<ClassWithOneDependency>());
        }

        [Fact]
        public void CanResolveConcreteType()
        {
            var store = new ComponentStore();
            store.Add<ClassWithNoDependencies, ClassWithNoDependencies>();
            var container = new PandoraContainer(store);

            Assert.DoesNotThrow(() => container.Resolve<ClassWithNoDependencies>());
        }

        [Fact]
        public void DependencyMissingExceptionPropagatesThroughMultipleLevels()
        {
            var store = new ComponentStore();
            store.Add<IService3, ClassDependingOnClassWithOneDependency>();
            store.Add<IService2, ClassWithOneDependency>();

            var container = new PandoraContainer(store);

            Assert.Throws<DependencyMissingException>(() => container.Resolve<IService3>());
        }

        [Fact]
        public void CanResolveWhenGivenTwoConstructorsWhereOneCantBeSatisfied()
        {
            var store = new ComponentStore();
            store.Add<IService4, ClassWithMultipleConstructors>();

            var container = new PandoraContainer(store);

            Assert.DoesNotThrow(() => container.Resolve<IService4>());
        }

        [Fact]
        public void CanResolveDependencyChainOfSameService()
        {
            var store = new ComponentStore();
            store.Add<IService, ClassWithDependencyOnItsOwnService>();
            store.Add<IService, ClassWithNoDependencies>();

            var container = new PandoraContainer(store);

            var service = container.Resolve<IService>();

            Assert.IsType(typeof (ClassWithDependencyOnItsOwnService), service);
            var ownService = (ClassWithDependencyOnItsOwnService)service;
            Assert.IsType(typeof (ClassWithNoDependencies), ownService.SubService);
        }

        [Fact]
        public void CanResolveDependencyChainOfSameServiceWithMultipleLevels()
        {
            var store = new ComponentStore();
            store.Add<IService, ClassWithDependencyOnItsOwnService>();
            store.Add<IService, ClassWithDependencyOnItsOwnService>();
            store.Add<IService, ClassWithNoDependencies>();

            var container = new PandoraContainer(store);

            var service = container.Resolve<IService>();

            var level1 = (ClassWithDependencyOnItsOwnService) service;
            var level2 = (ClassWithDependencyOnItsOwnService) level1.SubService;
            var level3 = (ClassWithNoDependencies)level2.SubService;

            Assert.IsType(typeof (ClassWithDependencyOnItsOwnService), level1);
            Assert.IsType(typeof(ClassWithDependencyOnItsOwnService), level2);
            Assert.IsType(typeof(ClassWithNoDependencies), level3);
        }

        [Fact]
        public void ThrowsDependencyMissingExceptionIfDependencyChainCannotBeSatisfied()
        {
            var store = new ComponentStore();
            store.Add<IService, ClassWithDependencyOnItsOwnService>();

            var container = new PandoraContainer(store);

            Assert.Throws<DependencyMissingException>(() => container.Resolve<IService>());
        }

        [Fact]
        public void CanResolveClassWithTwoDependenciesOnItsOwnServiceWithOnlyOneSubdependencyRegistration()
        {
            var store = new ComponentStore();
            store.Add<IService, ClassWithTwoDependenciesOnItsOwnService>();
            store.Add<IService, ClassWithNoDependencies>();

            var container = new PandoraContainer(store);

            var service = container.Resolve<IService>();
            Assert.IsType(typeof (ClassWithTwoDependenciesOnItsOwnService), service);
        }

        [Fact]
        public void CanSplitGraph()
        {

            /*
             * Dotty Graph: 
             * 
            digraph G {
              a [label="IService :: A"] ;
              a -> b [label="service1"] ;
              a -> c [label="service2"] ;
              b [label="IService :: B"] ;
              b -> d ;
              d [label="IService :: C"] ;
              c [label="IService :: B"] ;
              c -> f ;
              f [label="IService :: C"] ;
            }
             */

            var store = new ComponentStore();
            store.Add<IService, ClassWithTwoDependenciesOnItsOwnService>();
            store.Add<IService, ClassWithDependencyOnItsOwnService>();
            store.Add<IService, ClassWithNoDependencies>();

            var container = new PandoraContainer(store);

            var service = container.Resolve<IService>();
            Assert.IsType(typeof (ClassWithTwoDependenciesOnItsOwnService), service);
        }
    }
}