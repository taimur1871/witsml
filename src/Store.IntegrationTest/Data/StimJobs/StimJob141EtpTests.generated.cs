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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Energistics.Common;
using Energistics.DataAccess;
using Energistics.DataAccess.WITSML141;
using Energistics.DataAccess.WITSML141.ComponentSchemas;
using Energistics.DataAccess.WITSML141.ReferenceData;
using Energistics.Datatypes;
using Energistics.Protocol;
using Energistics.Protocol.Core;
using Energistics.Protocol.Discovery;
using Energistics.Protocol.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDS.WITSMLstudio.Store.Data.StimJobs
{
    [TestClass]
    public partial class StimJob141EtpTests : StimJob141TestBase
    {
        partial void BeforeEachTest();

        partial void AfterEachTest();

        protected override void OnTestSetUp()
        {
            EtpSetUp(DevKit.Container);
            BeforeEachTest();
            _server.Start();
        }

        protected override void OnTestCleanUp()
        {
            _server?.Stop();
            EtpCleanUp();
            AfterEachTest();
        }

        [TestMethod]
        public void StimJob141_Ensure_Creates_StimJob_With_Default_Values()
        {
            DevKit.EnsureAndAssert<StimJobList, StimJob>(StimJob);
        }

        [TestMethod]
        public async Task StimJob141_GetResources_Can_Get_All_StimJob_Resources()
        {
            AddParents();
            DevKit.AddAndAssert<StimJobList, StimJob>(StimJob);
            await RequestSessionAndAssert();

            var uri = StimJob.GetUri();
            var parentUri = uri.Parent;

            await GetResourcesAndAssert(parentUri);

            var folderUri = parentUri.Append(uri.ObjectType);
            await GetResourcesAndAssert(folderUri);
        }

        [TestMethod]
        public async Task StimJob141_PutObject_Can_Add_StimJob()
        {
            AddParents();
            await RequestSessionAndAssert();

            var handler = _client.Handler<IStoreCustomer>();
            var uri = StimJob.GetUri();

            var dataObject = CreateDataObject<StimJobList, StimJob>(uri, StimJob);

            // Get Object Expecting it Not to Exist
            await GetAndAssert(handler, uri, Energistics.EtpErrorCodes.NotFound);

            // Put Object
            await PutAndAssert(handler, dataObject);

            // Get Object
            var args = await GetAndAssert(handler, uri);

            // Check Data Object XML
            Assert.IsNotNull(args?.Message.DataObject);
            var xml = args.Message.DataObject.GetString();

            var result = Parse<StimJobList, StimJob>(xml);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task StimJob141_PutObject_Can_Update_StimJob()
        {
            AddParents();
            await RequestSessionAndAssert();

            var handler = _client.Handler<IStoreCustomer>();
            var uri = StimJob.GetUri();

            // Add a Comment to Data Object            
            StimJob.CommonData = new CommonData()
            {
                Comments = "Test PutObject"
            };

            var dataObject = CreateDataObject<StimJobList, StimJob>(uri, StimJob);

            // Get Object Expecting it Not to Exist
            await GetAndAssert(handler, uri, Energistics.EtpErrorCodes.NotFound);

            // Put Object for Add
            await PutAndAssert(handler, dataObject);

            // Get Added Object
            var args =await GetAndAssert(handler, uri);

            // Check Added Data Object XML
            Assert.IsNotNull(args?.Message.DataObject);
            var xml = args.Message.DataObject.GetString();

            var result = Parse<StimJobList, StimJob>(xml);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CommonData.Comments);

            // Remove Comment from Data Object
            result.CommonData.Comments = null;

            var updateDataObject = CreateDataObject<StimJobList, StimJob>(uri, result);

            // Put Object for Update
            await PutAndAssert(handler, updateDataObject);

            // Get Updated Object
            args = await GetAndAssert(handler, uri);

            // Check Added Data Object XML
            Assert.IsNotNull(args?.Message.DataObject);
            var updateXml = args.Message.DataObject.GetString();

            result = Parse<StimJobList, StimJob>(updateXml);
            Assert.IsNotNull(result);

            // Test Data Object overwrite
            Assert.IsNull(result.CommonData.Comments);
        }

        [TestMethod]
        public async Task StimJob141_DeleteObject_Can_Delete_StimJob()
        {
            AddParents();
            await RequestSessionAndAssert();

            var handler = _client.Handler<IStoreCustomer>();
            var uri = StimJob.GetUri();

            var dataObject = CreateDataObject<StimJobList, StimJob>(uri, StimJob);

            // Get Object Expecting it Not to Exist
            await GetAndAssert(handler, uri, Energistics.EtpErrorCodes.NotFound);

            // Put Object
            await PutAndAssert(handler, dataObject);

            // Get Object
            var args = await GetAndAssert(handler, uri);

            // Check Data Object XML
            Assert.IsNotNull(args?.Message.DataObject);
            var xml = args.Message.DataObject.GetString();

            var result = Parse<StimJobList, StimJob>(xml);
            Assert.IsNotNull(result);

            // Delete Object
            await DeleteAndAssert(handler, uri);

            // Get Object Expecting it Not to Exist
            await GetAndAssert(handler, uri, Energistics.EtpErrorCodes.NotFound);
        }
    }
}