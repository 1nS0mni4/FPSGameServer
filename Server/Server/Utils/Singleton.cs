using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils {
    public class Singleton<T> where T : new() {
        private static T _instance = new T();
        public static T Instance {
            get { 
                if(_instance == null)
                    _instance = new T();
                return _instance; 
            }
        }
    }
}
