using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMan.Services.Services.Session
{
    using System;
    using System.Collections.Generic;

    public class SessionModel
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public DateTime ExpirationTime { get; } = DateTime.MinValue;

        // You can store additional arbitrary data if needed
        private readonly Dictionary<string, object> _data = new();

        public void SetData(string key, object value)
        {
            _data[key] = value;
        }

        public bool TryGetData<T>(string key, out T? value)
        {
            if (_data.TryGetValue(key, out var obj) && obj is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }

        public bool RemoveData(string key)
        {
            return _data.Remove(key);
        }

        public void ClearData()
        {
            _data.Clear();
        }
    }

}
