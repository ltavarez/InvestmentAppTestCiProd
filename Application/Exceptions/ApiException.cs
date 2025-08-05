using System.Globalization;

namespace InvestmentApp.Core.Application.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, int statuCode) : base(message) {
            StatusCode = statuCode;
        }
        public ApiException(string message, params object[] args) 
            : base(String.Format(CultureInfo.CurrentCulture,message,args))
        {            
        }
    }
}