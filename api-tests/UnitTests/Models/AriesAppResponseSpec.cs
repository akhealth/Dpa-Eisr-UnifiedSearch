using System.Collections.Generic;
using Xunit;
using SearchApi.Models;
using System;

namespace SearchApi.Tests.Models
{
    public class AriesAppResponseSpec
    {
        [Fact]
        public void AriesAppResponse_returns_Application()
        {
            // Arrange
            var response = new AriesAppResponse
            {
                ARIESa7 = new ARIESa7
                {
                    optDESCRIPTION = "mockDescription",
                    optOPTION = "mockOption",
                    outApplication = new ApplicationList()
                    {
                        AriesApplication = new List<ApplicationInformation>()
                        {
                            new ApplicationInformation()
                            {
                                AppNumber = "mockAppNumber",
                                AppRecvdDate = "1/1/2018",
                                AppStatusCode = "mockStatusCode",
                                AppStatusDesc = "mockStatusDesc"
                            }
                        }
                    },
                    outIndvID = "mockIndvID"
                }
            };

            // Act
            var application = response.ARIESa7.Applications[0];

            // Assert
            Assert.Equal(application.ApplicationNumber, response.ARIESa7.outApplication.AriesApplication[0].AppNumber);
            Assert.Equal(application.Status, response.ARIESa7.outApplication.AriesApplication[0].AppStatusDesc);
            Assert.Equal(application.ReceivedDate, DateTime.Parse(response.ARIESa7.outApplication.AriesApplication[0].AppRecvdDate));
        }
    }
}