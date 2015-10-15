using System.Collections.Generic;
using System.Reflection;
using ImageMaker.CommonViewModels.Messenger;
using ImageMaker.CommonViewModels.Ninject;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using Moq;
using Ninject;
using Ninject.Modules;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ImageMaker.ViewModelsTests
{
    [TestFixture]
    public class NavigationTest
    {
        [Test]
        public void NavigateForward_NavigateToModelThatIsAlreadyInChainBeforeCurrent_ChainAfterTheTargetIsEmpty()
        {
            var kernel = NinjectBootstrapper.GetKernel(new TestModule());
            var firstClassMock = new Mock<ViewModelBaseFactory<TestClass1>>();
            TestClass1 instance1 = new TestClass1();
            firstClassMock.Setup(x => x.Get(It.IsAny<object>())).Returns(instance1);

            var secondClassMock = new Mock<ViewModelBaseFactory<TestClass2>>();
            TestClass2 instance2 = new TestClass2();
            secondClassMock.Setup(x => x.Get(It.IsAny<object>())).Returns(instance2);

            var thirdClassMock = new Mock<ViewModelBaseFactory<TestClass3>>();
            TestClass3 instance3 = new TestClass3();
            thirdClassMock.Setup(x => x.Get(It.IsAny<object>())).Returns(instance3);

            var fourthClassMock = new Mock<ViewModelBaseFactory<TestClass4>>();
            TestClass4 instance4 = new TestClass4();
            fourthClassMock.Setup(x => x.Get(It.IsAny<object>())).Returns(instance4);

            var factory = new ChildrenViewModelsFactory(new List<IViewModelFactory>
                                                        {
                                                            firstClassMock.Object,
                                                            secondClassMock.Object,
                                                            thirdClassMock.Object,
                                                            fourthClassMock.Object,
                                                        });

            kernel.Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WithConstructorArgument("childrenViewModelsFactory", factory);

            var nav = kernel.Get<IViewModelNavigator>();
            nav.NavigateForward(instance1);
            nav.NavigateForward<TestClass2>(instance1, null);
            nav.NavigateForward<TestClass3>(instance2, null);
            nav.NavigateForward<TestClass4>(instance3, null);

            nav.NavigateForward<TestClass1>(instance4, null);

            FieldInfo storage = typeof(ViewModelNavigator).GetField("_storage", BindingFlags.NonPublic | BindingFlags.Instance);
// ReSharper disable once PossibleNullReferenceException
            ViewModelStorage store = (ViewModelStorage) storage.GetValue(nav);
            FieldInfo order = typeof(ViewModelStorage).GetField("_navigationOrder", BindingFlags.NonPublic | BindingFlags.Instance);
// ReSharper disable once PossibleNullReferenceException
            LinkedList<BaseViewModel> orderVal = (LinkedList<BaseViewModel>)order.GetValue(store);
            Assert.IsTrue(orderVal.Count == 1);
        }

        [Test]
        public void NavigateForward_NavigateToModelThatIsAlreadyInChainAfterCurrent_ChainContainsTwoItems()
        {
            var kernel = NinjectBootstrapper.GetKernel(new TestModule());
            var firstClassMock = new Mock<ViewModelBaseFactory<TestClass1>>();
            TestClass1 instance1 = new TestClass1();
            firstClassMock.Setup(x => x.Get(It.IsAny<object>())).Returns(instance1);

            var secondClassMock = new Mock<ViewModelBaseFactory<TestClass2>>();
            TestClass2 instance2 = new TestClass2();
            secondClassMock.Setup(x => x.Get(It.IsAny<object>())).Returns(instance2);

            var thirdClassMock = new Mock<ViewModelBaseFactory<TestClass3>>();
            TestClass3 instance3 = new TestClass3();
            thirdClassMock.Setup(x => x.Get(It.IsAny<object>())).Returns(instance3);

            var fourthClassMock = new Mock<ViewModelBaseFactory<TestClass4>>();
            TestClass4 instance4 = new TestClass4();
            fourthClassMock.Setup(x => x.Get(It.IsAny<object>())).Returns(instance4);

            var factory = new ChildrenViewModelsFactory(new List<IViewModelFactory>
                                                        {
                                                            firstClassMock.Object,
                                                            secondClassMock.Object,
                                                            thirdClassMock.Object,
                                                            fourthClassMock.Object,
                                                        });

            kernel.Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WithConstructorArgument("childrenViewModelsFactory", factory);

            var nav = kernel.Get<IViewModelNavigator>();
            nav.NavigateForward(instance1);
            nav.NavigateForward<TestClass2>(instance1, null);
            nav.NavigateBack(instance2);
            nav.NavigateForward<TestClass2>(instance1, null);

            FieldInfo storage = typeof(ViewModelNavigator).GetField("_storage", BindingFlags.NonPublic | BindingFlags.Instance);
            // ReSharper disable once PossibleNullReferenceException
            ViewModelStorage store = (ViewModelStorage)storage.GetValue(nav);
            FieldInfo order = typeof(ViewModelStorage).GetField("_navigationOrder", BindingFlags.NonPublic | BindingFlags.Instance);
            // ReSharper disable once PossibleNullReferenceException
            LinkedList<BaseViewModel> orderVal = (LinkedList<BaseViewModel>)order.GetValue(store);
            Assert.IsTrue(orderVal.Count == 2);
        }
    }
    
    public class TestClass1 : BaseViewModel
    {
        
    }

    public class TestClass2 : BaseViewModel
    {
        
    }

    public class TestClass3 : BaseViewModel
    {
        
    }

    public class TestClass4 : BaseViewModel
    {
        
    }


    public class TestModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ViewModelStorage>().ToSelf().InSingletonScope();
            Bind<MessageFactory>().ToSelf();
            Bind<IMessenger>().To<MvvmLightMessenger>().InSingletonScope();
        }
    }
}
