//// Type: System.IdentityModel.Tokens.MruSessionSecurityTokenCache
//// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
//// MVID: E1BBA5EE-4BA4-45BC-A3C0-F079CBB05DC2
//// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.IdentityModel.dll

//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IdentityModel.Tokens;
//using System.Runtime;

//namespace Dlbr.CommonLogin.IdentityModel.Web.Internals
//{

//    internal class MruSessionSecurityTokenCache : SessionSecurityTokenCache
//    {
//        public static readonly TimeSpan DefaultPurgeInterval = TimeSpan.FromMinutes(15.0);
//        public const int DefaultTokenCacheSize = 0x4e20;
//        private Dictionary<SessionSecurityTokenCacheKey, CacheEntry> items;
//        private int maximumSize;
//        private CacheEntry mruEntry;
//        private LinkedList<SessionSecurityTokenCacheKey> mruList;
//        private DateTime nextPurgeTime;
//        private object purgeLock;
//        private int sizeAfterPurge;
//        private object syncRoot;

//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        public MruSessionSecurityTokenCache() : this(0x4e20)
//        {
//        }

//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        public MruSessionSecurityTokenCache(int maximumSize) : this(maximumSize, (IEqualityComparer<SessionSecurityTokenCacheKey>) null)
//        {
//        }

//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        public MruSessionSecurityTokenCache(int sizeAfterPurge, int maximumSize) : this(sizeAfterPurge, maximumSize, null)
//        {
//        }

//        public MruSessionSecurityTokenCache(int maximumSize, IEqualityComparer<SessionSecurityTokenCacheKey> comparer) : this((maximumSize / 5) * 4, maximumSize, comparer)
//        {
//        }

//        public MruSessionSecurityTokenCache(int sizeAfterPurge, int maximumSize, IEqualityComparer<SessionSecurityTokenCacheKey> comparer)
//        {
//            this.nextPurgeTime = DateTime.UtcNow + DefaultPurgeInterval;
//            this.syncRoot = new object();
//            this.purgeLock = new object();
//            if (sizeAfterPurge < 0)
//            {
//                throw new ArgumentException("ID0008", "sizeAfterPurge");
//            }
//            if (sizeAfterPurge >= maximumSize)
//            {
//                throw new ArgumentException("ID0009", "sizeAfterPurge");
//            }
//            this.items = new Dictionary<SessionSecurityTokenCacheKey, CacheEntry>(maximumSize, comparer);
//            this.maximumSize = maximumSize;
//            this.mruList = new LinkedList<SessionSecurityTokenCacheKey>();
//            this.sizeAfterPurge = sizeAfterPurge;
//            this.mruEntry = new CacheEntry();
//        }

//        public override void AddOrUpdate(SessionSecurityTokenCacheKey key, SessionSecurityToken value, DateTime expirationTime)
//        {
//            if (key == null)
//            {
//                throw new ArgumentNullException("key");
//            }
//            lock (this.syncRoot)
//            {
//                this.Purge();
//                this.Remove(key);
//                CacheEntry entry = new CacheEntry {
//                    Node = this.mruList.AddFirst(key),
//                    Value = value
//                };
//                this.items.Add(key, entry);
//                this.mruEntry = entry;
//            }
//        }

//        public override SessionSecurityToken Get(SessionSecurityTokenCacheKey key)
//        {
//            if (key == null)
//            {
//                return null;
//            }
//            SessionSecurityToken token = null;
//            lock (this.syncRoot)
//            {
//                CacheEntry entry;
//                if (((this.mruEntry.Node != null) && (key != null)) && key.Equals(this.mruEntry.Node.Value))
//                {
//                    return this.mruEntry.Value;
//                }
//                if (this.items.TryGetValue(key, out entry))
//                {
//                    token = entry.Value;
//                    if ((this.mruList.Count > 1) && !object.ReferenceEquals(this.mruList.First, entry.Node))
//                    {
//                        this.mruList.Remove(entry.Node);
//                        this.mruList.AddFirst(entry.Node);
//                        this.mruEntry = entry;
//                    }
//                }
//            }
//            return token;
//        }

//        public override IEnumerable<SessionSecurityToken> GetAll(string endpointId, System.Xml.UniqueId contextId)
//        {
//            Collection<SessionSecurityToken> collection = new Collection<SessionSecurityToken>();
//            if ((null != contextId) && !string.IsNullOrEmpty(endpointId))
//            {
//                SessionSecurityTokenCacheKey key = new SessionSecurityTokenCacheKey(endpointId, contextId, null) {
//                    IgnoreKeyGeneration = true
//                };
//                lock (this.syncRoot)
//                {
//                    foreach (SessionSecurityTokenCacheKey key2 in this.items.Keys)
//                    {
//                        if (key2.Equals(key))
//                        {
//                            CacheEntry entry = this.items[key2];
//                            if ((this.mruList.Count > 1) && !object.ReferenceEquals(this.mruList.First, entry.Node))
//                            {
//                                this.mruList.Remove(entry.Node);
//                                this.mruList.AddFirst(entry.Node);
//                                this.mruEntry = entry;
//                            }
//                            collection.Add(entry.Value);
//                        }
//                    }
//                }
//            }
//            return collection;
//        }

//        private void Purge()
//        {
//            if (this.items.Count >= this.maximumSize)
//            {
//                int num = this.maximumSize - this.sizeAfterPurge;
//                for (int i = 0; i < num; i++)
//                {
//                    SessionSecurityTokenCacheKey key = this.mruList.Last.Value;
//                    this.mruList.RemoveLast();
//                    this.items.Remove(key);
//                }
//            }
//        }

//        public override void Remove(SessionSecurityTokenCacheKey key)
//        {
//            if (key != null)
//            {
//                lock (this.syncRoot)
//                {
//                    CacheEntry entry;
//                    if (this.items.TryGetValue(key, out entry))
//                    {
//                        this.items.Remove(key);
//                        this.mruList.Remove(entry.Node);
//                        if (object.ReferenceEquals(this.mruEntry.Node, entry.Node))
//                        {
//                            this.mruEntry.Value = null;
//                            this.mruEntry.Node = null;
//                        }
//                    }
//                }
//            }
//        }

//        public override void RemoveAll(string endpointId)
//        {
//            throw new NotImplementedException("ID4294");
//        }

//        public override void RemoveAll(string endpointId, System.Xml.UniqueId contextId)
//        {
//            if ((null != contextId) && !string.IsNullOrEmpty(endpointId))
//            {
//                Dictionary<SessionSecurityTokenCacheKey, CacheEntry> dictionary = new Dictionary<SessionSecurityTokenCacheKey, CacheEntry>();
//                SessionSecurityTokenCacheKey key = new SessionSecurityTokenCacheKey(endpointId, contextId, null) {
//                    IgnoreKeyGeneration = true
//                };
//                lock (this.syncRoot)
//                {
//                    foreach (SessionSecurityTokenCacheKey key2 in this.items.Keys)
//                    {
//                        if (key2.Equals(key))
//                        {
//                            dictionary.Add(key2, this.items[key2]);
//                        }
//                    }
//                    foreach (SessionSecurityTokenCacheKey key3 in dictionary.Keys)
//                    {
//                        this.items.Remove(key3);
//                        CacheEntry entry = dictionary[key3];
//                        this.mruList.Remove(entry.Node);
//                        if (object.ReferenceEquals(this.mruEntry.Node, entry.Node))
//                        {
//                            this.mruEntry.Value = null;
//                            this.mruEntry.Node = null;
//                        }
//                    }
//                }
//            }
//        }

//        public int MaximumSize
//        {
//            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//            get
//            {
//                return this.maximumSize;
//            }
//        }

//        public class CacheEntry
//        {
//            public SessionSecurityToken Value { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set; }

//            public LinkedListNode<SessionSecurityTokenCacheKey> Node { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set; }

//            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//            public CacheEntry()
//            {
//            }
//        }

//    }
//}


namespace Dlbr.CommonLogin.IdentityModel.Web.Internals
{
    internal static class WSFederationConstants
    {
        internal const string Namespace = "http://docs.oasis-open.org/wsfed/federation/200706";

        public static class Actions
        {
            internal const string Attribute = "wattr1.0";
            internal const string Pseudonym = "wpseudo1.0";
            internal const string SignIn = "wsignin1.0";
            internal const string SignOut = "wsignout1.0";
            internal const string SignOutCleanup = "wsignoutcleanup1.0";
        }

        public static class Parameters
        {
            internal const string Action = "wa";
            internal const string Attribute = "wattr";
            internal const string AttributePtr = "wattrptr";
            internal const string AuthenticationType = "wauth";
            internal const string Context = "wctx";
            internal const string CurrentTime = "wct";
            internal const string Encoding = "wencoding";
            internal const string Federation = "wfed";
            internal const string Freshness = "wfresh";
            internal const string HomeRealm = "whr";
            internal const string Policy = "wp";
            internal const string Pseudonym = "wpseudo";
            internal const string PseudonymPtr = "wpseudoptr";
            internal const string Realm = "wtrealm";
            internal const string Reply = "wreply";
            internal const string Request = "wreq";
            internal const string RequestPtr = "wreqptr";
            internal const string Resource = "wres";
            internal const string Result = "wresult";
            internal const string ResultPtr = "wresultptr";
        }

        public static class FaultCodeValues
        {
            internal const string AlreadySignedIn = "AlreadySignedIn";
            internal const string BadRequest = "BadRequest";
            internal const string IssuerNameNotSupported = "IssuerNameNotSupported";
            internal const string NeedFresherCredentials = "NeedFresherCredentials";
            internal const string NoMatchInScope = "NoMatchInScope";
            internal const string NoPseudonymInScope = "NoPseudonymInScope";
            internal const string NotSignedIn = "NotSignedIn";
            internal const string RstParameterNotAccepted = "RstParameterNotAccepted";
            internal const string SpecificPolicy = "SpecificPolicy";
            internal const string UnsupportedClaimsDialect = "UnsupportedClaimsDialect";
            internal const string UnsupportedEncoding = "UnsupportedEncoding";
        }
    }
}

