using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.WebApi
{
    internal class AdminsKeyManager : IAdminKeyManager
    {
        private readonly IHashEncoder _hashEncoder;

        //private readonly HashSet<string> _activeKeys;
        private readonly ConcurrentDictionary<string, string> _activeKeys;

        public AdminsKeyManager(IHashEncoder hashEncoder)
        {
            _hashEncoder = hashEncoder;
            _activeKeys = new ConcurrentDictionary<string, string>();
        }
        public Task<string> GetKeyAsync(string username, string passwordHash)
        {
            string key = _hashEncoder.GetHash($"!{username}_!_{passwordHash}");

            if (!_activeKeys.ContainsKey(key))
            {
                _activeKeys.TryAdd(key, username);
            }

            return Task.FromResult(key);
        }

        public Task<string> GetLoginAsync(string key)
        {
            if (_activeKeys.ContainsKey(key))
            {
                return Task.FromResult(_activeKeys[key]);
            }
            throw new ArgumentException();
        }

        public Task<bool> KeyValidAsync(string key)
        {
            return Task.FromResult(_activeKeys.ContainsKey(key));
        }
    }
}
