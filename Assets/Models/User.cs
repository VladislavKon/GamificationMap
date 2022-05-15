using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    public class User
    {
        public string userName;
        public Guid id;
        public int points;
        public string role;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
