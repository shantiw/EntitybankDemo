using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XData.Data.Dynamic
{
    public class DynamicDictionary : DynamicObject, IDictionary<string, object>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Dictionary.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return Dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Dictionary[binder.Name] = value;
            return true;
        }

        protected readonly Dictionary<string, object> Dictionary = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                return Dictionary[key];
            }
            set
            {
                if (Dictionary.ContainsKey(key))
                {
                    object val = Dictionary[key];
                    if (val == value) return;
                }
                Dictionary[key] = value;
                OnPropertyChanged(key);
            }
        }

        public ICollection<string> Keys => Dictionary.Keys;

        public ICollection<object> Values => Dictionary.Values;

        public int Count => Dictionary.Count;

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;

        public void Add(string key, object value)
        {
            Dictionary.Add(key, value);
            OnPropertyChanged(key);
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            List<string> keys = new List<string>(Dictionary.Keys);
            Dictionary.Clear();
            foreach (string key in keys)
            {
                OnPropertyChanged(key);
            }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            if (Dictionary.ContainsKey(item.Key))
            {
                object value = Dictionary[item.Key];
                return item.Value == value;
            }

            return false;
        }

        public bool ContainsKey(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            Dictionary.ToArray().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            bool result = Dictionary.Remove(key);
            OnPropertyChanged(key);
            return result;
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            if (Contains(item))
            {
                return Remove(item.Key);
            }

            return false;
        }

        public bool TryGetValue(string key, out object value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }


    }
}
