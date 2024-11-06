
using Proyecto26;
using System;

namespace ApiModels
{
    [Serializable]
    public class ApiResponse
    {
        public dynamic Data { get; set; }

        public dynamic Err { get; set; }
    }
}