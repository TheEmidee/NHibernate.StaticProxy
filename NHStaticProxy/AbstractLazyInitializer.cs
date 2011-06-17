using System;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace NHStaticProxy
{
    [Serializable]
    public abstract class AbstractLazyInitializer : ILazyInitializer
    {
        protected object _target = (object)null;
        private bool initialized;
        private object _id;
        [NonSerialized]
        private ISessionImplementor _session;
        private bool unwrap;
        private readonly string _entityName;
        private bool readOnly;
        private bool? readOnlyBeforeAttachedToSession;

        protected internal bool IsConnectedToSession
        {
            get
            {
                return this.GetProxyOrNull() != null;
            }
        }

        public object Identifier
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        public abstract Type PersistentClass { get; }

        public bool IsUninitialized
        {
            get
            {
                return !this.initialized;
            }
        }

        public bool Unwrap
        {
            get
            {
                return this.unwrap;
            }
            set
            {
                this.unwrap = value;
            }
        }

        public ISessionImplementor Session
        {
            get
            {
                return this._session;
            }
            set
            {
                if (value != this._session)
                {
                    if (value != null && this.IsConnectedToSession)
                        throw new LazyInitializationException(this._entityName, this._id, "Illegally attempted to associate a proxy with two open Sessions");
                    else
                        this._session = value;
                }
            }
        }

        public string EntityName
        {
            get
            {
                return this._entityName;
            }
        }

        protected internal object Target
        {
            get
            {
                return this._target;
            }
        }

        public bool IsReadOnlySettingAvailable
        {
            get
            {
                return this._session != null && !this._session.IsClosed;
            }
        }

        public bool ReadOnly
        {
            get
            {
                this.ErrorIfReadOnlySettingNotAvailable();
                return this.readOnly;
            }
            set
            {
                this.ErrorIfReadOnlySettingNotAvailable();
                if (this.readOnly != value)
                    this.SetReadOnly(value);
            }
        }

        static AbstractLazyInitializer()
        {
        }

        protected internal AbstractLazyInitializer(string entityName, object id, ISessionImplementor session)
        {
            this._id = id;
            this._entityName = entityName;
            if (session == null)
                this.UnsetSession();
            else
                this.SetSession(session);
        }

        public void SetSession(ISessionImplementor s)
        {
            if (s != this._session)
            {
                if (s == null)
                    this.UnsetSession();
                else if (this.IsConnectedToSession)
                {
                    throw new HibernateException("illegally attempted to associate a proxy with two open Sessions");
                }
                else
                {
                    this._session = s;
                    if (!this.readOnlyBeforeAttachedToSession.HasValue)
                    {
                        IEntityPersister entityPersister = s.Factory.GetEntityPersister(this._entityName);
                        this.SetReadOnly(s.PersistenceContext.DefaultReadOnly || !entityPersister.IsMutable);
                    }
                    else
                    {
                        this.SetReadOnly(this.readOnlyBeforeAttachedToSession.Value);
                        this.readOnlyBeforeAttachedToSession = new bool?();
                    }
                }
            }
        }

        public void UnsetSession()
        {
            this._session = (ISessionImplementor)null;
            this.readOnly = false;
            this.readOnlyBeforeAttachedToSession = new bool?();
        }

        public virtual void Initialize()
        {
            if (!this.initialized)
            {
                if (this._session == null)
                    throw new LazyInitializationException(this._entityName, this._id, "Could not initialize proxy - no Session.");
                else if (!this._session.IsOpen)
                    throw new LazyInitializationException(this._entityName, this._id, "Could not initialize proxy - the owning Session was closed.");
                else if (!this._session.IsConnected)
                {
                    throw new LazyInitializationException(this._entityName, this._id, "Could not initialize proxy - the owning Session is disconnected.");
                }
                else
                {
                    this._target = this._session.ImmediateLoad(this._entityName, this._id);
                    this.initialized = true;
                    this.CheckTargetState();
                }
            }
            else
                this.CheckTargetState();
        }

        public object GetImplementation()
        {
            this.Initialize();
            return this._target;
        }

        public object GetImplementation(ISessionImplementor s)
        {
            EntityKey key = new EntityKey(this.Identifier, s.Factory.GetEntityPersister(this.EntityName), s.EntityMode);
            return s.PersistenceContext.GetEntity(key);
        }

        public void SetImplementation(object target)
        {
            this._target = target;
            this.initialized = true;
        }

        private void ErrorIfReadOnlySettingNotAvailable()
        {
            if (this._session == null)
                throw new TransientObjectException("Proxy is detached (i.e, session is null). The read-only/modifiable setting is only accessible when the proxy is associated with an open session.");
            else if (this._session.IsClosed)
                throw new SessionException("Session is closed. The read-only/modifiable setting is only accessible when the proxy is associated with an open session.");
        }

        private static EntityKey GenerateEntityKeyOrNull(object id, ISessionImplementor s, string entityName)
        {
            if (id == null || s == null || entityName == null)
                return (EntityKey)null;
            else
                return new EntityKey(id, s.Factory.GetEntityPersister(entityName), s.EntityMode);
        }

        private void CheckTargetState()
        {
            if (!this.unwrap && this._target == null)
                this.Session.Factory.EntityNotFoundDelegate.HandleEntityNotFound(this._entityName, this._id);
        }

        private object GetProxyOrNull()
        {
            EntityKey key = AbstractLazyInitializer.GenerateEntityKeyOrNull(this._id, this._session, this._entityName);
            if (key != null && this._session != null && this._session.IsOpen)
                return this._session.PersistenceContext.GetProxy(key);
            else
                return (object)null;
        }

        private void SetReadOnly(bool readOnly)
        {
            if (!this._session.Factory.GetEntityPersister(this._entityName).IsMutable && !readOnly)
            {
                throw new InvalidOperationException("cannot make proxies for immutable entities modifiable");
            }
            else
            {
                this.readOnly = readOnly;
                if (this.initialized)
                {
                    EntityKey key = AbstractLazyInitializer.GenerateEntityKeyOrNull(this._id, this._session, this._entityName);
                    if (key != null && this._session.PersistenceContext.ContainsEntity(key))
                        this._session.PersistenceContext.SetReadOnly(this._target, readOnly);
                }
            }
        }
    }
}