using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.Utils
{
    public static class LocationHelper
    {
        public static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var d1 = (double)lat1 * (Math.PI / 180.0);
            var num1 = (double)lon1 * (Math.PI / 180.0);
            var d2 = (double)lat2 * (Math.PI / 180.0);
            var num2 = (double)lon2 * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6371 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3))); // Result in Kilometers
        }
    }
}
