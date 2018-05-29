using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeAreDevelopers.Core.Interfaces
{
    public interface IStarwarsRequest
    {
        Task<JObject> StarWarsDataRequest(Uri RequestUri);
   }
}
