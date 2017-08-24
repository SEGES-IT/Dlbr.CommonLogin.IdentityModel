using System;
using Dlbr.CommonLogin.IdentityModel.WebApi;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Integration.IdentityModel
{
    [TestFixture]
    public class LibLogTests
    {
        [Test]
        public void LibLogLog4netAdapterPicksUpLogsFromLibrary()
        {
            var appender = ConfigureMemoryAppender();
            var sut = new DeflatedSamlTokenHeaderEncoder();
            sut.Encode("simulatedtoken");
            var events = appender.GetEvents();
            Assert.AreEqual(1,events.Length);
            foreach (var e in events)
            {
                Console.WriteLine(e.RenderedMessage);
            }
        }

        private static MemoryAppender ConfigureMemoryAppender()
        {
            Hierarchy hierarchy = (Hierarchy) LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();


            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
            return memory;
        }
    }
}
