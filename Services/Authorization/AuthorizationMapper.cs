using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotifyPlayer.Services.Authorization
{
    public class AuthorizationMapper : IAuthorizationMapper
    {
        private readonly List<Mapping> _mappings = new List<Mapping>();

        public string Get(string key)
        {
            var mapping = _mappings.FirstOrDefault(x => x.Key == key);

            return mapping?.Timestamp > DateTime.Now ? mapping.Code : null;
        }
        public string Set(string code)
        {
            var key = GenerateKey();

            _mappings.Add(new Mapping
            {
                Key = key,
                Code = code,
                Timestamp = DateTime.Now.AddMinutes(10)
            });

            return key;
        }

        private string GenerateKey()
        {
            var random = new Random();
            var key = random.Next(100000, 999999).ToString();

            if (_mappings.FirstOrDefault(x => x.Key == key) == null)
                return key;
            else
                return GenerateKey();
        }

        protected class Mapping
        {
            public string Key { get; set; }
            public string Code { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
