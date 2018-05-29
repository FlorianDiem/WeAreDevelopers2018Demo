using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeAreDevelopers.Core.DTO;
using WeAreDevelopers.Core.Interfaces;

namespace WeAreDevelopers.Core.Classes
{ 
    public abstract class MergeAndCreateAnswer:IMergeAndCreateAnswer
    {
        public abstract string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer);
        public abstract string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer,JObject StarwarsAnswer2);
    }
    public class SingleProcessHeight: MergeAndCreateAnswer
    {      
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer)
        {            
            string Name = processedLuisResult.Name_0;
            string result = starwarsAnswer.Value<string>(processedLuisResult.Intent);

            if ((String.IsNullOrEmpty(result) != true) && (result != "unknown"))
            {
                return Name + " is " + result + " centimeters tall.";
            }
            else
            {
                return $"I am sorry! The force within me is not strong enough to know how tall {Name} is";
            }            
        }

        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer, JObject StarwarsAnswer2)
        {
            throw new NotImplementedException();
        }
    }

    public class SingleProcessYear : MergeAndCreateAnswer
    {
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer)
        {
            string Name = processedLuisResult.Name_0;
            string result = starwarsAnswer.Value<string>(processedLuisResult.Intent);

            if ((String.IsNullOrEmpty(result) != true) && (result != "unknown"))
            {
                return Name + " was born in the year " + result;
            }
            else
            {
                return $"I am sorry! The force within me is not strong enough to know which year {Name} was born.";
            }            
        }
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer, JObject StarwarsAnswer2)
        {
            throw new NotImplementedException();
        }
    }

    public class SingleProcessMass : MergeAndCreateAnswer
    {
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer)
        {
            string Name = processedLuisResult.Name_0;
            string result = starwarsAnswer.Value<string>(processedLuisResult.Intent);

            if((String.IsNullOrEmpty(result) != true) && (result != "unknown"))
            {
                return Name + " weighs about " + result + " kilogramms.";
            }
            else
            {
                return $"I am sorry! The force within me is not strong enough to know the weight of {Name}";
            }
        }
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer, JObject StarwarsAnswer2)
        {
            throw new NotImplementedException();
        }
    }

    public class SingleProcessEyeColor : MergeAndCreateAnswer
    {
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer)
        {
            string Name = processedLuisResult.Name_0;
            string result = starwarsAnswer.Value<string>(processedLuisResult.Intent);
            
            if ((String.IsNullOrEmpty(result) != true) && (result != "unknown"))
            {
                return "The eyes of " + Name + " are " + result;
            }
            else
            {
                return $"I am sorry! The force within me is not strong enough to know the eye color of {Name}";
            }
        }
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer, JObject StarwarsAnswer2)
        {
            throw new NotImplementedException();
        }
    }

    public class PluralProcessYear : MergeAndCreateAnswer
    {
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer)
        {
            throw new NotImplementedException();
        }
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer,JObject starwarsAnswer2)
        {
            string Name = processedLuisResult.Name_0;
            string result = starwarsAnswer.Value<string>(processedLuisResult.Intent);
            return "The eyes of " + Name + " are " + result;
        }
    }

    public class PluralProcessMass : MergeAndCreateAnswer
    {
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer)
        {
            throw new NotImplementedException();
        }
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer, JObject starwarsAnswer2)
        {
            string Name = processedLuisResult.Name_0;
            string result = starwarsAnswer.Value<string>(processedLuisResult.Intent);
            return "The eyes of " + Name + " are " + result;
        }
    }

    public class PluralProcessHeight : MergeAndCreateAnswer
    {
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer)
        {
            throw new NotImplementedException();
        }
        public override string DoMergeAndCreateAnswer(TransferObjectLuis processedLuisResult, JObject starwarsAnswer, JObject starwarsAnswer2)
        {
            string Name = processedLuisResult.Name_0;
            string Name1 = processedLuisResult.Name_1;

            string result = starwarsAnswer.Value<string>(processedLuisResult.Intent);
            string result1 = starwarsAnswer2.Value<string>(processedLuisResult.Intent);

            return "The y of " + Name + " are " + result + " " + Name1 + " are " + result1;
        }
    }

}