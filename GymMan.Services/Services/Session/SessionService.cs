using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMan.Services.Services.Session
{
    public class SessionService : ISessionService
    {
        private ConcurrentDictionary<string, SessionModel> _userSessions = new ConcurrentDictionary<string, SessionModel>();

        ///<inheritdoc cref="ISessionService.CreateSession"/>
        public SessionModel CreateSession(string sessionKey, SessionModel session)
        {
            _userSessions[sessionKey] = session;
            return session;
        }

        ///<inheritdoc cref="ISessionService.TryGetSession"/>
        public bool TryGetSession(string sessionKey, out SessionModel? session)
        {
            return _userSessions.TryGetValue(sessionKey, out session);
        }

        ///<inheritdoc cref="ISessionService.RemoveSession"/>
        public bool RemoveSession(string sessionKey)
        {
            return _userSessions.TryRemove(sessionKey, out _);
        }

        ///<inheritdoc cref="ISessionService.ClearAllSessions"/>
        public void ClearAllSessions()
        {
            _userSessions.Clear();
        }
    }
}
