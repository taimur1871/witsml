﻿//----------------------------------------------------------------------- 
// PDS.Witsml.Server, 2016.1
//
// Copyright 2016 Petrotechnical Data Systems
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Energistics.DataAccess;
using Energistics.DataAccess.WITSML141;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDS.Witsml.Server.Demo
{
    [TestClass]
    public class Demo
    {
        private DevKit141Aspect DevKit;
        private string BaseDir;
        private string DataDir;

        [TestInitialize]
        public void TestSetUp()
        {
            DevKit = new DevKit141Aspect();

            DevKit.Store.CapServerProviders = DevKit.Store.CapServerProviders
                .Where(x => x.DataSchemaVersion == OptionsIn.DataVersion.Version141.Value)
                .ToArray();

            BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            DataDir = BaseDir + @"\Demo\Data\";
        }

        public void Add_Log_from_file(string xmlfile)
        {
            var xmlin = File.ReadAllText(xmlfile);

            var logList = EnergisticsConverter.XmlToObject<LogList>(xmlin);
            Assert.IsNotNull(logList);
            Assert.IsTrue(logList.Log.Count > 0);

            var log = new Log() { Uid = logList.Log[0].Uid };
            var result = DevKit.Query<LogList, Log>(log);
            Assert.IsNotNull(result);
            if (result.Count > 0)
            {
                // Do not add if the log already exists.
                return;
            }

            var response = DevKit.AddToStore(ObjectTypes.Log, xmlin, null, null);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);
        }

        public void Add_Well_from_file(string xmlfile)
        {
            var xmlin = File.ReadAllText(xmlfile);

            var wellList = EnergisticsConverter.XmlToObject<WellList>(xmlin);
            Assert.IsNotNull(wellList);
            Assert.IsTrue(wellList.Well.Count > 0);

            var well = new Well() { Uid = wellList.Well[0].Uid };
            var result = DevKit.Query<WellList, Well>(well);
            Assert.IsNotNull(result);
            if (result.Count>0)
            {
                // Do not add if the well already exists.
                return;
            }

            var response = DevKit.AddToStore(ObjectTypes.Well, xmlin, null, null);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);
        }

        public void Add_Wellbore_from_file(string xmlfile)
        {
            var xmlin = File.ReadAllText(xmlfile);

            var wellboreList = EnergisticsConverter.XmlToObject<WellboreList>(xmlin);
            Assert.IsNotNull(wellboreList);
            Assert.IsTrue(wellboreList.Wellbore.Count > 0);

            var wellbore = new Wellbore() { Uid = wellboreList.Wellbore[0].Uid };
            var result = DevKit.Query<WellboreList, Wellbore>(wellbore);
            Assert.IsNotNull(result);
            if (result.Count > 0)
            {
                // Do not add if the wellbore already exists.
                return;
            }

            var response = DevKit.AddToStore(ObjectTypes.Wellbore, xmlin, null, null);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);
        }

        /// <summary>
        /// Add <see cref="Well"/> and <see cref="Wellbore"/> object to the store.
        /// </summary>
        [TestMethod]
        public void Add_parents()
        {
            string[] wellFiles = Directory.GetFiles(DataDir, "*_Well.xml");

            foreach (string xmlfile in wellFiles)
            {
                Add_Well_from_file(xmlfile);
            }

            string[] wellboreFiles = Directory.GetFiles(DataDir, "*_Wellbore.xml");
            foreach (string xmlfile in wellboreFiles)
            {
                Add_Wellbore_from_file(xmlfile);
            }
        }

        /// <summary>
        /// Add <see cref="Logs"/> to the store
        /// </summary>
        [TestMethod]
        public void Add_logs()
        {
            string[] logFiles = Directory.GetFiles(DataDir, "*_Log.xml");

            foreach (string xmlfile in logFiles)
            {
                Add_Log_from_file(xmlfile);
            }
        }

        /// <summary>
        /// Add <see cref="Well"/>, <see cref="Wellbore"/>, <see cref="Logs"/> to the store
        /// </summary>
        [TestMethod]
        public void Add_data()
        {
            Add_parents();
            Add_logs();
        }
    }
}
