using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Infrastructure.Classes
{
    internal class Command
    {
        public Command(string text)
        {
            _text = text;
        }
        public string Text {
            get
            {
                return _text;
            }
            set {
                _text = value;
            }
        }
        protected internal string _text;
    }
}
