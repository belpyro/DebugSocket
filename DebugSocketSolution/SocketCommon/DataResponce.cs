using System;

namespace SocketCommon
{

    [Serializable]
    public class DataResponce
    {
        public DataResponce(bool isError, object value)
        {
            HasError = isError;
            Data = value;
        } 

        public bool HasError { get; set; } 

        public object Data { get; set; }
    }
}