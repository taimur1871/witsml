//----------------------------------------------------------------------- 
// PDS WITSMLstudio Store, 2017.2
//
// Copyright 2017 PDS Americas LLC
// 
// Licensed under the PDS Open Source WITSML Product License Agreement (the
// "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//     http://www.pds.group/WITSMLstudio/OpenSource/ProductLicenseAgreement
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

// ----------------------------------------------------------------------
// <auto-generated>
//     Changes to this file may cause incorrect behavior and will be lost
//     if the code is regenerated.
// </auto-generated>
// ----------------------------------------------------------------------

using Energistics.DataAccess;
using Energistics.DataAccess.WITSML141;
using Energistics.DataAccess.WITSML141.ComponentSchemas;
using Energistics.DataAccess.WITSML141.ReferenceData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace PDS.WITSMLstudio.Store.Data.StimJobs
{
    [TestClass]
    public partial class StimJob141StoreTests : StimJob141TestBase
    {
        partial void BeforeEachTest();

        partial void AfterEachTest();

        protected override void OnTestSetUp()
        {
            BeforeEachTest();
        }

        protected override void OnTestCleanUp()
        {
            AfterEachTest();
        }

        [TestMethod]
        public void StimJob141DataAdapter_GetFromStore_Can_Get_StimJob()
        {
            AddParents();
            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);
            DevKit.GetAndAssert<StimJobList, StimJob>(StimJob);
       }

        [TestMethod]
        public void StimJob141DataAdapter_AddToStore_Can_Add_StimJob()
        {
            AddParents();
            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);
        }

        [TestMethod]
        public void StimJob141DataAdapter_UpdateInStore_Can_Update_StimJob()
        {
            AddParents();
            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);
            DevKit.UpdateAndAssert<StimJobList, StimJob>(StimJob);
            DevKit.GetAndAssert<StimJobList, StimJob>(StimJob);
        }

        [TestMethod]
        public void StimJob141DataAdapter_DeleteFromStore_Can_Delete_StimJob()
        {
            AddParents();
            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);
            DevKit.DeleteAndAssert<StimJobList, StimJob>(StimJob);
            DevKit.GetAndAssert<StimJobList, StimJob>(StimJob, isNotNull: false);
        }

        [TestMethod]
        public void StimJob141DataAdapter_AddToStore_Creates_ChangeLog()
        {
            AddParents();

            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);

            var result = DevKit.GetAndAssert<StimJobList, StimJob>(StimJob);
            var expectedHistoryCount = 1;
            var expectedChangeType = ChangeInfoType.add;
            DevKit.AssertChangeLog(result, expectedHistoryCount, expectedChangeType);
        }

        [TestMethod]
        public void StimJob141DataAdapter_UpdateInStore_Updates_ChangeLog()
        {
            AddParents();

            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);

            // Update the StimJob141
            StimJob.Name = "Change";
            DevKit.UpdateAndAssert(StimJob);

            var result = DevKit.GetAndAssert<StimJobList, StimJob>(StimJob);
            var expectedHistoryCount = 2;
            var expectedChangeType = ChangeInfoType.update;
            DevKit.AssertChangeLog(result, expectedHistoryCount, expectedChangeType);
        }

        [TestMethod]
        public void StimJob141DataAdapter_DeleteFromStore_Updates_ChangeLog()
        {
            AddParents();

            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);

            // Delete the StimJob141
            DevKit.DeleteAndAssert(StimJob);

            var expectedHistoryCount = 2;
            var expectedChangeType = ChangeInfoType.delete;
            DevKit.AssertChangeLog(StimJob, expectedHistoryCount, expectedChangeType);
        }

        [TestMethod]
        public void StimJob141DataAdapter_ChangeLog_Tracks_ChangeHistory_For_Add_Update_Delete()
        {
            AddParents();

            // Add the StimJob141
            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);

            // Verify ChangeLog for Add
            var result = DevKit.GetAndAssert<StimJobList, StimJob>(StimJob);
            var expectedHistoryCount = 1;
            var expectedChangeType = ChangeInfoType.add;
            DevKit.AssertChangeLog(result, expectedHistoryCount, expectedChangeType);

            // Update the StimJob141
            StimJob.Name = "Change";
            DevKit.UpdateAndAssert(StimJob);

            result = DevKit.GetAndAssert<StimJobList, StimJob>(StimJob);
            expectedHistoryCount = 2;
            expectedChangeType = ChangeInfoType.update;
            DevKit.AssertChangeLog(result, expectedHistoryCount, expectedChangeType);

            // Delete the StimJob141
            DevKit.DeleteAndAssert(StimJob);

            expectedHistoryCount = 3;
            expectedChangeType = ChangeInfoType.delete;
            DevKit.AssertChangeLog(StimJob, expectedHistoryCount, expectedChangeType);

            // Re-add the same StimJob141...
            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);

            //... the same changeLog should be reused.
            result = DevKit.GetAndAssert<StimJobList, StimJob>(StimJob);
            expectedHistoryCount = 4;
            expectedChangeType = ChangeInfoType.add;
            DevKit.AssertChangeLog(result, expectedHistoryCount, expectedChangeType);

            DevKit.AssertChangeHistoryTimesUnique(result);
        }

        [TestMethod]
        public void StimJob141DataAdapter_GetFromStore_Filter_ExtensionNameValue()
        {
            AddParents();

            var extensionName1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");
            var extensionName2 = DevKit.ExtensionNameValue("Ext-2", "2.0", "cm", PrimitiveType.@float);
            extensionName2.MeasureClass = MeasureClass.Length;
            var extensionName3 = DevKit.ExtensionNameValue("Ext-3", "3.0", "cm", PrimitiveType.unknown);

            StimJob.CommonData = new CommonData()
            {
                ExtensionNameValue = new List<ExtensionNameValue>()
                {
                    extensionName1, extensionName2, extensionName3
                }
            };

            // Add the StimJob141
            DevKit.AddAndAssert(StimJob);

            // Query for first extension
            var commonDataXml = "<commonData>" + Environment.NewLine +
                                "<extensionNameValue uid=\"\">" + Environment.NewLine +
                                "<name />{0}" + Environment.NewLine +
                                "</extensionNameValue>" + Environment.NewLine +
                                "</commonData>";

            var extValueQuery = string.Format(commonDataXml, "<dataType>double</dataType>");
            var queryXml = string.Format(BasicXMLTemplate, StimJob.UidWell, StimJob.UidWellbore, StimJob.Uid, extValueQuery);
            var result = DevKit.Query<StimJobList, StimJob>(ObjectTypes.StimJob, queryXml, null, OptionsIn.ReturnElements.Requested);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            var resultStimJob = result[0];
            Assert.IsNotNull(resultStimJob);

            var commonData = resultStimJob.CommonData;
            Assert.IsNotNull(commonData);
            Assert.AreEqual(1, commonData.ExtensionNameValue.Count);

            var env = commonData.ExtensionNameValue[0];
            Assert.IsNotNull(env);
            Assert.AreEqual(extensionName1.Uid, env.Uid);
            Assert.AreEqual(extensionName1.Name, env.Name);

            // Query for second extension
            extValueQuery = string.Format(commonDataXml, "<measureClass>length</measureClass>");
            queryXml = string.Format(BasicXMLTemplate, StimJob.UidWell, StimJob.UidWellbore, StimJob.Uid, extValueQuery);
            result = DevKit.Query<StimJobList, StimJob>(ObjectTypes.StimJob, queryXml, null, OptionsIn.ReturnElements.Requested);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            resultStimJob = result[0];
            Assert.IsNotNull(resultStimJob);

            commonData = resultStimJob.CommonData;
            Assert.IsNotNull(commonData);
            Assert.AreEqual(1, commonData.ExtensionNameValue.Count);

            env = commonData.ExtensionNameValue[0];
            Assert.IsNotNull(env);
            Assert.AreEqual(extensionName2.Uid, env.Uid);
            Assert.AreEqual(extensionName2.Name, env.Name);

            // Query for third extension
            extValueQuery = string.Format(commonDataXml, "<dataType>unknown</dataType>");
            queryXml = string.Format(BasicXMLTemplate, StimJob.UidWell, StimJob.UidWellbore, StimJob.Uid, extValueQuery);
            result = DevKit.Query<StimJobList, StimJob>(ObjectTypes.StimJob, queryXml, null, OptionsIn.ReturnElements.Requested);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            resultStimJob = result[0];
            Assert.IsNotNull(resultStimJob);

            commonData = resultStimJob.CommonData;
            Assert.IsNotNull(commonData);
            Assert.AreEqual(1, commonData.ExtensionNameValue.Count);

            env = commonData.ExtensionNameValue[0];
            Assert.IsNotNull(env);
            Assert.AreEqual(extensionName3.Uid, env.Uid);
            Assert.AreEqual(extensionName3.Name, env.Name);
        }

        [TestMethod]
        public void StimJob141DataAdapter_ChangeLog_Syncs_StimJob_Name_Changes()
        {
            AddParents();

            // Add the StimJob141
            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);

            // Assert that all StimJob names match corresponding changeLog names
            DevKit.AssertChangeLogNames(StimJob);

            // Update the StimJob141 names
            StimJob.Name = "Change";
            StimJob.NameWell = "Well Name Change";

            StimJob.NameWellbore = "Wellbore Name Change";

            DevKit.UpdateAndAssert(StimJob);

            // Assert that all StimJob names match corresponding changeLog names
            DevKit.AssertChangeLogNames(StimJob);
        }
    }
}