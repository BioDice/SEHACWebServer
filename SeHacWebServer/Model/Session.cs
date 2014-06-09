using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    class Session
    {
        private string _sessionId;

        public string SessionId
        {
            get { return _sessionId; }
            set { _sessionId = value; }
        }
        private string _role;

        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }

        private string _user;

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }
        public Session(string id,string role,string user)
        {
            this._user = user;
            this._sessionId = id;
            this._role = role;
        }
    }
}
