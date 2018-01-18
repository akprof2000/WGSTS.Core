using Core.Base;
using Core.Interfaces;
using InsideAppWatcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using WGSTS.Logger;

namespace UnitTestCore
{
    [TestClass]
    public class FullUnitTest
    {
        [TestMethod]
        public void CoreConfig_Test_Parameters()
        {
            var cc = new CoreConfig();
            Assert.AreEqual(10, cc.LogFileCount);
            Assert.AreEqual("substance", cc.SubstancePath);
            Assert.AreEqual(1024 * 1024 * 10, cc.LogFileSize);
            Assert.AreEqual("plugins", cc.PluginsPath);
        }

        [TestMethod]
        public void ActionConfig_Test_Parameters()
        {
            var ac = new ActionConfig();
            Assert.AreEqual(false, ac.Controled );
            Assert.AreEqual(null, ac.Events);
        }

        [TestMethod]
        public void CoreDispetcher_Test_StartAndStop()
        {
            CoreDispetcher.Logger = new DummyLogger();
            CoreDispetcher.Start();

            CoreDispetcher.Stop();

        }

        [TestMethod]
        public void BaseCoreConfiguration_Test_Parameters()
        {
            var bcc = new BaseCoreConfiguration()
            {
                ClassName = "UnitTestCore.tst",
                ConfigName = "FileConfigforTstUnitTestCore.json",
                FileName = "UnitTestCore.dll"
            };
            Assert.AreEqual("UnitTestCore.tst", bcc.ClassName);
            Assert.AreEqual("FileConfigforTstUnitTestCore.json", bcc.ConfigName);
            Assert.AreEqual("UnitTestCore.dll", bcc.FileName);
        }

        [TestMethod]
        public void CoreDispetcher_Test_OnNeedClose()
        {
            CoreDispetcher.Logger = new DummyLogger();
            bool start = false;
            CoreDispetcher.OnNeedClose += () => { start = true; };
            CoreDispetcher.Start();            
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServerConfig", "configmain.json");           
            var cc = new CoreConfig() { LogFileCount = (int)DateTime.Now.Ticks };
            File.WriteAllText(path, cc.ToJson());
            Thread.Sleep(12000);
            CoreDispetcher.Stop();

            Assert.AreEqual(true, start);

        }

        class Tst : ISubstance
        {
            public StatusSubstance Status { get; set; }
            public bool Controled { get; set; }
            public int Id { get; set; }
            public Guid Index { get; set; }
            public Guid GUID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime Creation { get; set; }
            public StorageActionType StorageAction { get; set; }
            public long Timestamp { get; set; }
        }

        [TestMethod]
        public void SandboxDataValue_Test_Parameters()
        {
            var sdv = new SandboxDataValue
            {
                Value = new Tst() { Timestamp = DateTime.Now.Ticks }
            };

            Assert.AreEqual(null, sdv.TheType);

            sdv = new SandboxDataValue();

            sdv.AddData(new Tst() { Timestamp = DateTime.Now.Ticks });

            Assert.AreEqual(typeof(Tst), sdv.TheType);


        }
        
        [TestMethod]
        public void SendReceiveAndStartApp_Test_Parameters()
        {

            Assert.AreEqual(SendReceiveAndStartApp.CoreStartDll, "Core.dll");
            Assert.AreEqual(SendReceiveAndStartApp.Port, 33333);
            Assert.AreEqual(SendReceiveAndStartApp.SelfPort, 22222);

        }

        [TestMethod]
        public void SendReceiveAndStartApp_Test_ReversPort()
        {

            Assert.AreEqual(SendReceiveAndStartApp.CoreStartDll, "Core.dll");
            Assert.AreEqual(SendReceiveAndStartApp.Port, 33333);
            Assert.AreEqual(SendReceiveAndStartApp.SelfPort, 22222);
            SendReceiveAndStartApp.RevercePort();
            Assert.AreEqual(SendReceiveAndStartApp.CoreStartDll, "Core.Watcher.dll");
            Assert.AreEqual(SendReceiveAndStartApp.Port, 22222);
            Assert.AreEqual(SendReceiveAndStartApp.SelfPort, 33333);

        }


        [TestMethod]
        public void SendReceiveAndStartApp_Test_Start_Stop()
        {
            SendReceiveAndStartApp.Sart();
            Thread.Sleep(15000);
            SendReceiveAndStartApp.Stop();
        }

    }
}
