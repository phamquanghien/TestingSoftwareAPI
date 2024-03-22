using System.Diagnostics;

namespace TestingSoftwareAPI.Models.Process
{
    public class RandomValue
    {
        public int[] RandomDistinctArray(int minValue, int quantity)
        {
            Stopwatch stw = new Stopwatch();
            int[] returnArray = new int[quantity];
            Random rd = new Random();
            for(int i = 0; i < quantity; i++)
            {
                returnArray[i] = minValue;
                minValue++;
            }
            
            for(int i = 0; i < quantity-1; i++)
            {
                int randomValue = rd.Next(i+1, returnArray.Count());
                int tempValue = returnArray[i];
                returnArray[i] = returnArray[randomValue];
                returnArray[randomValue] = tempValue;
            }

            return returnArray;
        }
    }
}