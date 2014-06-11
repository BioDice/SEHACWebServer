using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeHacWebServer.Database;

namespace SeHacWebServer.Model
{
    public class Session
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
        public Session(string id,string role,string user,string ip)
        {
            this._user = user;
            this._sessionId = id;
            this._role = role;
            this._clientIp = ip;


            int startin = 60 - DateTime.Now.Second;
            var t = new System.Threading.Timer(TimerCallback,null, startin * 1000, 60000);
        }

        public void TimerCallback(Object o)
        {
            SessionManager.deleteSession(this);
        }

        private string _clientIp;

        public string ClientIp
        {
            get { return _clientIp; }
            set { _clientIp = value; }
        }

    }
}
