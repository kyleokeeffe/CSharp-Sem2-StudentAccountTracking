using System;
using System.Collections.Generic;
using System.Text;

namespace COMP123_006_OKeeffeKyle_Assignment4.Services.Interfaces
{
    interface IGPAService<T>
    {
        public void Save(T obj);
        public T Load(T obj);
        public List<T> LoadAll();
        public void Update(T obj);
    }
}
