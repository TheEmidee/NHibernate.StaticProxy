using System;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace NHibernate.StaticProxy
{
    [Serializable]
    public abstract class AbstractLazyInitializer : ILazyInitializer
    {
        private readonly string entityName;
        private object id;
        private bool initialized;
        private bool readOnly;
        private bool? readOnlyBeforeAttachedToSession;
        [NonSerialized] private ISessionImplementor session;
        protected object target;
        private bool unwrap;

        protected internal AbstractLazyInitializer(string entityName, object id, ISessionImplementor session)
        {
            this.id = id;
            this.entityName = entityName;
            if (session == null)
                UnsetSession();
            else
                SetSession(session);
        }

        private bool IsConnectedToSession
        {
            get { return GetProxyOrNull() != null; }
        }

        protected internal object Target
        {
            get { return target; }
        }

        #region ILazyInitializer Members

        public object Identifier
        {
            get { return id; }
            set { id = value; }
        }

        public abstract System.Type PersistentClass { get; }

        public bool IsUninitialized
        {
            get { return !initialized; }
        }

        public bool Unwrap
        {
            get { return unwrap; }
            set { unwrap = value; }
        }

        public ISessionImplementor Session
        {
            get { return session; }
            set
            {
                if (value != session)
                {
                    if (value != null && IsConnectedToSession)
                        throw new LazyInitializationException(entityName, id, "Illegally attempted to associate a proxy with two open Sessions");
                    
                    session = value;
                }
            }
        }

        public string EntityName
        {
            get { return entityName; }
        }

        public bool IsReadOnlySettingAvailable
        {
            get { return session != null && !session.IsClosed; }
        }

        public bool ReadOnly
        {
            get
            {
                ErrorIfReadOnlySettingNotAvailable();
                return readOnly;
            }
            set
            {
                ErrorIfReadOnlySettingNotAvailable();
                if (readOnly != value)
                    SetReadOnly(value);
            }
        }

        public void SetSession(ISessionImplementor s)
        {
            if (s != session)
            {
                if (s == null)
                    UnsetSession();
                else if (IsConnectedToSession)
                {
                    throw new HibernateException("illegally attempted to associate a proxy with two open Sessions");
                }
                else
                {
                    session = s;
                    if (!readOnlyBeforeAttachedToSession.HasValue)
                    {
                        IEntityPersister entityPersister = s.Factory.GetEntityPersister(entityName);
                        SetReadOnly(s.PersistenceContext.DefaultReadOnly || !entityPersister.IsMutable);
                    }
                    else
                    {
                        SetReadOnly(readOnlyBeforeAttachedToSession.Value);
                        readOnlyBeforeAttachedToSession = new bool?();
                    }
                }
            }
        }

        public void UnsetSession()
        {
            session = null;
            readOnly = false;
            readOnlyBeforeAttachedToSession = new bool?();
        }

        public virtual void Initialize()
        {
            if (!initialized)
            {
                if (session == null)
                    throw new LazyInitializationException(entityName, id, "Could not initialize proxy - no Session.");
                if (!session.IsOpen)
                    throw new LazyInitializationException(entityName, id, "Could not initialize proxy - the owning Session was closed.");
                if (!session.IsConnected)
                    throw new LazyInitializationException(entityName, id, "Could not initialize proxy - the owning Session is disconnected.");

                target = session.ImmediateLoad(entityName, id);
                initialized = true;
                CheckTargetState();
            }
            else
                CheckTargetState();
        }

        public object GetImplementation()
        {
            Initialize();
            return target;
        }

        public object GetImplementation(ISessionImplementor s)
        {
            var key = new EntityKey(Identifier, s.Factory.GetEntityPersister(EntityName), s.EntityMode);
            return s.PersistenceContext.GetEntity(key);
        }

        public void SetImplementation(object target)
        {
            this.target = target;
            initialized = true;
        }

        #endregion

        private void ErrorIfReadOnlySettingNotAvailable()
        {
            if (session == null)
                throw new TransientObjectException("Proxy is detached (i.e, session is null). The read-only/modifiable setting is only accessible when the proxy is associated with an open session.");
            
            if (session.IsClosed)
                throw new SessionException("Session is closed. The read-only/modifiable setting is only accessible when the proxy is associated with an open session.");
        }

        private static EntityKey GenerateEntityKeyOrNull(object id, ISessionImplementor s, string entityName)
        {
            if (id == null || s == null || entityName == null)
                return null;
            
            return new EntityKey(id, s.Factory.GetEntityPersister(entityName), s.EntityMode);
        }

        private void CheckTargetState()
        {
            if (!unwrap && target == null)
                Session.Factory.EntityNotFoundDelegate.HandleEntityNotFound(entityName, id);
        }

        private object GetProxyOrNull()
        {
            EntityKey key = GenerateEntityKeyOrNull(id, session, entityName);
            if (key != null && session != null && session.IsOpen)
                return session.PersistenceContext.GetProxy(key);
            
            return null;
        }

        private void SetReadOnly(bool readOnly)
        {
            if (!session.Factory.GetEntityPersister(entityName).IsMutable && !readOnly)
                throw new InvalidOperationException("cannot make proxies for immutable entities modifiable");
            
            this.readOnly = readOnly;
            if (initialized)
            {
                EntityKey key = GenerateEntityKeyOrNull(id, session, entityName);
                if (key != null && session.PersistenceContext.ContainsEntity(key))
                    session.PersistenceContext.SetReadOnly(target, readOnly);
            }
        }
    }
}