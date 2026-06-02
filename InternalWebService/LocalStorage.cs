using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService
{
    public class LocalStorage
    {
        private Dictionary<string, StorageItem> db = new Dictionary<string, StorageItem>();

        

        public string AddNew(StorageItem content)
        {
            var key = Guid.NewGuid().ToString();
            db.Add(key,content);

            return key;
        }


        public void Set(string key, StorageItem content)
        {
            if (db.ContainsKey(key))
            {
                db[key] = content;
            }
            else
            {
                db.Add(key, content);
            }
        }


        public void SetValue(string key,string newValue)
        {
            if (db.ContainsKey(key))
            {
                if (db[key].ValueChanged != null)
                {
                    db[key].ValueChanged.Invoke(key, db[key].Value,newValue);
                }
                db[key].Value = newValue;
            }
        }
     

        public StorageItem Get(string key)
        {
            if(db.TryGetValue(key,out var value))
            {
                return value;
            }
            return null;
        }

        public bool Exists(string key)
        {
            return db.ContainsKey(key);
        }

    }



    public class StorageItem
    {
        public string Value { get; set; }
        public Action<object,string,string> ValueChanged { get; set; }
    }

}
