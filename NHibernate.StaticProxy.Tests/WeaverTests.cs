// ReSharper disable InconsistentNaming

using System;
using Moq;
using NHibernate.StaticProxy.Tests.Entities;
using PostSharp;
using PostSharp.Aspects;
using Xunit;

namespace NHibernate.StaticProxy.Tests
{
    public class WeaverTests
    {
        [Fact]
        public void Entities_Implement_IPostSharpNHibernateProxy()
        {
            Assert.True(Activator.CreateInstance(typeof(Customer)) is INHibernateStaticProxy);
            Assert.True(Activator.CreateInstance(typeof(Order)) is INHibernateStaticProxy);
        }

        [Fact]
        public void Entities_DerivingFromBase_ImplementIPostSharpNHibernateProxy()
        {
            Assert.True(Activator.CreateInstance(typeof(Customer2)) is INHibernateStaticProxy);
            Assert.True(Activator.CreateInstance(typeof(Order2)) is INHibernateStaticProxy);
        }

        [Fact]
        public void MappedPropertiesSetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, INHibernateStaticProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            cust.Name = "Zoubi";

            stub.Verify(x => x.InterceptSet(It.IsAny<ILocationBinding>(), "Zoubi"), Times.Once());
        }

        [Fact]
        public void MappedPropertiesGetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();
            cust.Name = "Zoubi";

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, INHibernateStaticProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            var str = cust.Name;

            stub.Verify(x => x.InterceptGet(It.IsAny<ILocationBinding>()), Times.Once());
        }

        [Fact]
        public void MappedPropertiesWithBackingFieldSetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, INHibernateStaticProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            cust.PropertyWithField = "PropertyWithField";

            stub.Verify(x => x.InterceptSet(It.IsAny<ILocationBinding>(), "PropertyWithField"), Times.Once());
        }

        [Fact]
        public void MappedPropertiesWithBackingFieldGetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();
            cust.PropertyWithField = "PropertyWithField";

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, INHibernateStaticProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            var str = cust.PropertyWithField;

            stub.Verify(x => x.InterceptGet(It.IsAny<ILocationBinding>()), Times.Once());
        }

        [Fact]
        public void MappedFieldsSetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, INHibernateStaticProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            cust.fieldOnly = "fieldOnly";

            stub.Verify(x => x.InterceptSet(It.IsAny<ILocationBinding>(), "fieldOnly"), Times.Once());
        }

        [Fact]
        public void MappedFieldsGetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();
            cust.fieldOnly = "fieldOnly";

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, INHibernateStaticProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            var str = cust.PropertyWithField;

            stub.Verify(x => x.InterceptGet(It.IsAny<ILocationBinding>()), Times.Once());
        }

        [Fact]
        public void NotMappedPropertiesSetters_AreNotIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, INHibernateStaticProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            cust.NotMapped = "Zoubi";

            stub.Verify(x => x.InterceptSet(It.IsAny<ILocationBinding>(), "Zoubi"), Times.Never());
        }

        [Fact]
        public void NotMappedPropertiesGetters_AreNotIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();
            cust.NotMapped = "Zoubi";

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, INHibernateStaticProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            var str = cust.NotMapped;

            stub.Verify(x => x.InterceptGet(It.IsAny<ILocationBinding>()), Times.Never());
        }
    }
}

// ReSharper restore InconsistentNaming