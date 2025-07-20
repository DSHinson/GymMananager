using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMan.Services.Services.Session
{
    /// <summary>
    /// Defines methods for managing user sessions.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Creates or replaces a session associated with the specified session key.
        /// </summary>
        /// <param name="sessionKey">A unique identifier for the session.</param>
        /// <param name="session">The session data to store.</param>
        /// <returns>The created session.</returns>
        SessionModel CreateSession(string sessionKey, SessionModel session);

        /// <summary>
        /// Attempts to retrieve a session associated with the specified session key.
        /// </summary>
        /// <param name="sessionKey">The session key to look up.</param>
        /// <param name="session">When this method returns, contains the session if found; otherwise, null.</param>
        /// <returns><c>true</c> if the session was found; otherwise, <c>false</c>.</returns>
        bool TryGetSession(string sessionKey, out SessionModel? session);

        /// <summary>
        /// Removes the session associated with the specified session key.
        /// </summary>
        /// <param name="sessionKey">The session key to remove.</param>
        /// <returns><c>true</c> if the session was successfully removed; otherwise, <c>false</c>.</returns>
        bool RemoveSession(string sessionKey);

        /// <summary>
        /// Removes all active sessions.
        /// </summary>
        void ClearAllSessions();
    }

}
