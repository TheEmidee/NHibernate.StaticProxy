// ReSharper disable InconsistentNaming

using NHibernate.StaticProxy.Tests.Config;
using NHibernate.StaticProxy.Tests.Entities;
using Xunit;

namespace NHibernate.StaticProxy.Tests
{
    public class SavingInstanceToDatabase : NHTestsBase<HbmMappingProvider>
    {
        [Fact]
        public void CanSaveItemToDatabase()
        {
            int custId;
            using (var s = SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var obj = new Customer { Name = "Zoubi" };
                s.Save(obj);
                tx.Commit();
                Assert.NotEqual(0, obj.Id);
                custId = obj.Id;
            }

            using (var s = SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var load = s.Get<Customer>(custId);
                s.Delete(load);
                tx.Commit();
            }
        }

    }
}

// ReSharper restore InconsistentNaming