
using System.Windows;

namespace XData.Windows.Components
{
    public abstract class Behavior : FrameworkElement
    {
        protected object AssociatedObject
        {
            get;
            private set;
        } 
 
        public void Attach(object obj)
        {
            if (obj != AssociatedObject)
            {              
                AssociatedObject = obj;
                OnAttached();
            }
        }

        protected abstract void OnAttached();
        
    }

    public abstract class Behavior<T> : Behavior
    {
        protected new T AssociatedObject
        {
            get
            {
                return (T)base.AssociatedObject;
            }
        }
    }


}
