using System;
using Xunit;

namespace TestProject
{
    public class UnitTest1
    {
        [Fact]
        public void DateStringTest1()
        {
            const string dateTimeString = "Apr 1 2021 12:00AM";

            bool IsDateTime = DateTime.TryParse(dateTimeString, out _);
            
            DateTime rs = IsDateTime ? DateTime.Parse(dateTimeString) : DateTime.Now;

            Assert.False(IsDateTime, "1 should not be prime");
        }
    }
}
