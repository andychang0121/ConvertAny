using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string dateTimeString = "Apr 1 2021 12:00AM";

            // �ഫ�e���ˬd�O�_�i�H�ഫ
            bool isDateTime = !string.IsNullOrEmpty(dateTimeString) 
                              && DateTime.TryParse(dateTimeString, out _);

            string rsString = isDateTime 
                ? DateTime.Parse(dateTimeString).ToString(CultureInfo.InvariantCulture) : string.Empty;

            DateTime rsDateTime = isDateTime
                ? DateTime.Parse(dateTimeString)
                : DateTime.Now;
        }
    }
}
