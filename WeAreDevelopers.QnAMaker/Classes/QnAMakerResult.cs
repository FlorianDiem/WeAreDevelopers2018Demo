using Newtonsoft.Json;
using System.Collections.Generic;

namespace WeAreDevelopers.QnAMaker.Classes
{
    public class QnAMakerResult
    {
        /// <summary>
        /// The top answer found in the QnA Service.
        /// </summary>
        [JsonProperty(PropertyName = "answers")]
        public List<QnAMakerResultItem> Answers { get; set; }

        public class QnAMakerResultItem
        {
            /// <summary>
            /// The top answer found in the QnA Service.
            /// </summary>
            [JsonProperty(PropertyName = "answer")]
            public string Answer { get; set; }

            /// <summary>
            /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
            /// </summary>
            [JsonProperty(PropertyName = "score")]
            public double Score { get; set; }
        }
    }
}