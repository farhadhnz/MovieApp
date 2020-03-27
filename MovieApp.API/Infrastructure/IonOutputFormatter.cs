using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.API.Infrastructure
{
    public class IonOutputFormatter : TextOutputFormatter
    {
        private readonly SystemTextJsonOutputFormatter _systemTextJsonOutputFormatter;

        public IonOutputFormatter(SystemTextJsonOutputFormatter systemTextJsonOutputFormatter)
        {
            if (systemTextJsonOutputFormatter == null) throw new ArgumentNullException(nameof(systemTextJsonOutputFormatter));
            _systemTextJsonOutputFormatter = systemTextJsonOutputFormatter;

            SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/ion+json"));
            SupportedEncodings.Add(Encoding.UTF8);
            
        }
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            return _systemTextJsonOutputFormatter.WriteResponseBodyAsync(context, selectedEncoding);
        }
    }
}
