using System;

namespace SocketCommon
{

    [Serializable]
    public class DataResponce
    {
        private DataResponce(){}

        public DataResponce(bool isError, object value)
        {
            HasError = isError;
            Data = value;
        } 

        public bool HasError { get; private set; } 

        public object Data { get; private set; }
    }
}