// ReSharper disable InconsistentNaming

using Moq;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHStaticProxy.Tests.Entities;
using PostSharp;
using PostSharp.Aspects;
using Xunit;

namespace NHStaticProxy.Tests
{
    public class WeaverTests
    {
        [Fact]
        public void Test()
        {
            var att = new ModelMapperStaticProxyConfigurationAttribute();
            
            var cfg = new Configuration();

            cfg.DataBaseIntegration(db =>
                                        {
                                            db.Dialect<GenericDialect>();
                                        });

            foreach (var hbmMapping in att.HbmMappings)
                cfg.AddDeserializedMapping(hbmMapping, "fnu");

            
        }
        
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
        public void MappedPropertiesWithBackingFieldSetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, IPostSharpNHibernateProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            cust.PropertyWithField = "PropertyWithField";

            stub.Verify(x => x.InterceptSet(It.IsAny<LocationInterceptionArgs>()), Times.Once());
        }

        [Fact]
        public void MappedPropertiesWithBackingFieldGetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();
            cust.PropertyWithField = "PropertyWithField";

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, IPostSharpNHibernateProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            var str = cust.PropertyWithField;

            stub.Verify(x => x.InterceptGet(It.IsAny<LocationInterceptionArgs>()), Times.Once());
        }

        [Fact]
        public void MappedFieldsSetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, IPostSharpNHibernateProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            cust.fieldOnly = "fieldOnly";

            stub.Verify(x => x.InterceptSet(It.IsAny<LocationInterceptionArgs>()), Times.Once());
        }

        [Fact]
        public void MappedFieldsGetters_AreIntercepted_WhenEntityIsProxy()
        {
            var cust = new Customer();
            cust.fieldOnly = "fieldOnly";

            var stub = new Mock<IStaticProxyLazyInitializer>();

            var nhProxy = Post.Cast<Customer, IPostSharpNHibernateProxy>(cust);

            nhProxy.SetInterceptor(stub.Object);

            var str = cust.PropertyWithField;

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