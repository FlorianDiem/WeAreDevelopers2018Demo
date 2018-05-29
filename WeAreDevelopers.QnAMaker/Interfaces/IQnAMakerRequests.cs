using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeAreDevelopers.QnAMaker.Classes;

namespace WeAreDevelopers.QnAMaker.Interfaces
{
    public interface IQnAMakerRequests
    {
        Task<QnAMakerResult> GetQnAMakerResponse(string query, string knowledgeBaseId, string subscriptionKey);
    }
}
