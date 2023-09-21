using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Donkey.Administration.Environment;

namespace Donkey.Administration.WebApi
{
    internal class GeneralKeyManager : IGeneralKeyManager, IAsyncDisposable
    {
        private Dictionary<DateTime, string> _activeKeys;
        private Task _timeValidationTask;
        private CancellationTokenSource _tokenSource;
        private bool _disposed;
        private IHashEncoder _hashEncoder;
        private readonly string _initializingTime;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly List<string> _allowedReceivers = new List<string>()
        {
            "n1ght",
            "Krivoku",
            "alyona-zZzZ",
            "evanescence",
            "creagent"
        };

        private volatile string _firstKey;

        public GeneralKeyManager(IHashEncoder hashEncoder)
        {
            _tokenSource = new CancellationTokenSource();
            _disposed = false;
            _activeKeys = new Dictionary<DateTime, string>();
            _hashEncoder = hashEncoder;

            string initializingTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            _firstKey = _hashEncoder.GetHash($"donkey_inc-{initializingTime}");

            _initializingTime = initializingTime;
            _logger.Warn($"FirstKey updated: {_firstKey}");

            //_timeValidationTask = Task.Run(() => StartValidating(_tokenSource.Token));

            Task.Run(() => FixTimeAsync());
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(GeneralKeyManager));
            }
            _tokenSource.Cancel();

            await _timeValidationTask;

            _disposed = true;
        }

        private async Task FixTimeAsync()
        {
            while (true)
            {
                string currentTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                if(currentTime != _initializingTime)
                {
                    UpdateFirstKey();

                    _timeValidationTask = Task.Run(() => StartValidating(_tokenSource.Token));
                    break;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }

        public Task<string> GetKeyAsync(string receiver, string firstKey)
        {
            return Task.Run(() =>
            {
                lock (_allowedReceivers)
                {
                    if (!_allowedReceivers.Contains(receiver))
                    {
                        throw new InvalidOperationException("Получатель не существует среди требуемых");
                    }
                }

                lock (_firstKey)
                {
                    if (_firstKey != firstKey)
                    {
                        throw new InvalidOperationException("First key is invalid");
                    }
                }

                string key = Guid.NewGuid().ToString();

                lock (_activeKeys)
                {
                    _activeKeys.Add(DateTime.Now, key);
                }

                return key;
            });
            
        }

        public Task<bool> KeyValidAsync(string key)
        {
            //return Task.FromResult(true);
            return Task.Run(() =>
            {
                lock (_activeKeys)
                {
                    return _activeKeys.ContainsValue(key);
                }
            });
        }

        private async Task StartValidating(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                //_logger.Warn($"{nameof(StartValidating)} called");

                await Task.Delay(TimeSpan.FromSeconds(60), token);
                UpdateFirstKey();

                lock (_activeKeys)
                {
                    foreach(var dictKey in _activeKeys.Keys)
                    {
                        var delta = DateTime.Now - dictKey;

                        if(delta >= TimeSpan.FromMinutes(10))
                        {
                            _activeKeys.Remove(dictKey);
                        }
                    }
                }
            }
        }

        private void UpdateFirstKey()
        {
            string datetime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            lock (_firstKey)
            {
                lock (_hashEncoder)
                {
                    _firstKey = _hashEncoder.GetHash($"donkey_inc-{datetime}");

                    _logger.Warn($"FirstKey updated: {_firstKey}");
                }
            }
        }        
    }
}
