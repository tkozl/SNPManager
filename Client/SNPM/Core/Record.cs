using SNPM.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core
{
    public class Record : ObservableObject, IRecord
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _location;

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private string _comment;

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private DateTime _lastAccess;

        public DateTime LastAccess
        {
            get { return _lastAccess; }
            set { _lastAccess = value; }
        }



        public Record(
            string name,
            string location,
            string username,
            string comment
        )
        {
            this.Name = name;
            this.Location = location;
            this.Username = username;
            this.Comment = comment;
            this.LastAccess = DateTime.Now;
        }
    }
}
