// ReSharper disable InconsistentNaming

using System;
using Moq;
using NHStaticProxy.ModelMapper.Tests.Entities;
using PostSharp;
using PostSharp.Aspects;
using Xunit;

namespace NHStaticProxy.ModelMapper.Tests
{
    public class WeaverTests
    {
        [Fact]
        public void Entities_Implement_IPostSharpNHibernateProxy()
        {
            Assert.Contains(typeof(IPostSharpNHibernateProxy), typeof(Customer).GetInterfaces());
            Assert.Contains(typeof(IPostSharpNHibernateProxy), typeof(Order).GetInterfaces());
        }

        [Fact]
        public void MappedPropertiesSetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, IPostSharpNHibernateProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            cust.Name = "Zoubi";

            stub.Verify(x => x.InterceptSet(It.IsAny<LocationInterceptionArgs>()), Times.Once());
        }

        [Fact]
        public void MappedPropertiesGetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();
            cust.Name = "Zoubi";

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, IPostSharpNHibernateProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            var str = cust.Name;

            stub.Verify(x => x.InterceptGet(It.IsAny<LocationInterceptionArgs>()), Times.Once());
        }

        [Fact]
        public void NotMappedPropertiesSetters_AreNotIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, IPostSharpNHibernateProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            cust.NotMapped = "Zoubi";

            stub.Verify(x => x.InterceptSet(It.IsAny<LocationInterceptionArgs>()), Times.Never());
        }

        [Fact]
        public void NotMappedPropertiesGetters_AreNotIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();
            cust.NotMapped = "Zoubi";

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, IPostSharpNHibernateProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            var str = cust.NotMapped;

            stub.Verify(x => x.InterceptGet(It.IsAny<LocationInterceptionArgs>()), Times.Never());
        }
    }
}

// ReSharper restore InconsistentNaming