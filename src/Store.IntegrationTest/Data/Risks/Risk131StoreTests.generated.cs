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
using Energistics.DataAccess.WITSML131;
using Energistics.DataAccess.WITSML131.ComponentSchemas;
using Energistics.DataAccess.WITSML131.ReferenceData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDS.WITSMLstudio.Store.Data.Risks
{
    [TestClass]
    public partial class Risk131StoreTests : Risk131TestBase
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
        public void Risk131DataAdapter_GetFromStore_Can_Get_Risk()
        {
            AddParents();
            DevKit.AddAndAssert<RiskList, Risk>(Risk);
            DevKit.GetAndAssert<RiskList, Risk>(Risk);
       }

        [TestMethod]
        public void Risk131DataAdapter_AddToStore_Can_Add_Risk()
        {
            AddParents();
            DevKit.AddAndAssert<RiskList, Risk>(Risk);
        }

        [TestMethod]
        public void Risk131DataAdapter_UpdateInStore_Can_Update_Risk()
        {
            AddParents();
            DevKit.AddAndAssert<RiskList, Risk>(Risk);
            DevKit.UpdateAndAssert<RiskList, Risk>(Risk);
            DevKit.GetAndAssert<RiskList, Risk>(Risk);
        }

        [TestMethod]
        public void Risk131DataAdapter_DeleteFromStore_Can_Delete_Risk()
        {
            AddParents();
            DevKit.AddAndAssert<RiskList, Risk>(Risk);
            DevKit.DeleteAndAssert<RiskList, Risk>(Risk);
            DevKit.GetAndAssert<RiskList, Risk>(Risk, isNotNull: false);
        }

        [TestMethod]
        public void Risk131WitsmlStore_GetFromStore_Can_Transform_Risk()
        {
            AddParents();
            DevKit.AddAndAssert<RiskList, Risk>(Risk);

            // Re-initialize all capServer providers
            DevKit.Store.CapServerProviders = null;
            DevKit.Container.BuildUp(DevKit.Store);

            string typeIn, queryIn;
            var query = DevKit.List(DevKit.CreateQuery(Risk));
            DevKit.SetupParameters<RiskList, Risk>(query, ObjectTypes.Risk, out typeIn, out queryIn);

            var options = OptionsIn.Join(OptionsIn.ReturnElements.All, OptionsIn.DataVersion.Version141);
            var request = new WMLS_GetFromStoreRequest(typeIn, queryIn, options, null);
            var response = DevKit.Store.WMLS_GetFromStore(request);

            Assert.IsFalse(string.IsNullOrWhiteSpace(response.XMLout));
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var result = WitsmlParser.Parse(response.XMLout);
            var version = ObjectTypes.GetVersion(result.Root);
            Assert.AreEqual(OptionsIn.DataVersion.Version141.Value, version);
        }
    }
}