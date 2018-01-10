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
using System;
using System.Collections.Generic;
using System.Linq;
using Energistics.DataAccess;
using Energistics.DataAccess.WITSML141;
using Energistics.DataAccess.WITSML141.ComponentSchemas;
using Energistics.DataAccess.WITSML141.ReferenceData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDS.WITSMLstudio.Store.Data.ToolErrorTermSets
{
    [TestClass]
    public partial class ToolErrorTermSet141ValidatorTests : ToolErrorTermSet141TestBase
    {

        #region Error -401

        public static readonly string QueryInvalidPluralRoot =
            "<toolErrorTermSet xmlns=\"http://www.witsml.org/schemas/1series\" version=\"1.4.1.1\">" + Environment.NewLine +
            "  <toolErrorTermSet>" + Environment.NewLine +
            "    <name>Test Plural Root Element</name>" + Environment.NewLine +
            "  </toolErrorTermSet>" + Environment.NewLine +
            "</toolErrorTermSet>";

        [TestMethod]
        public void ToolErrorTermSet141Validator_GetFromStore_Error_401_No_Plural_Root_Element()
        {
            var response = DevKit.GetFromStore(ObjectTypes.ToolErrorTermSet, QueryInvalidPluralRoot, null, null);
            Assert.AreEqual((short)ErrorCodes.MissingPluralRootElement, response.Result);
        }

        [TestMethod]
        public void ToolErrorTermSet141Validator_AddToStore_Error_401_No_Plural_Root_Element()
        {
            var response = DevKit.AddToStore(ObjectTypes.ToolErrorTermSet, QueryInvalidPluralRoot, null, null);
            Assert.AreEqual((short)ErrorCodes.MissingPluralRootElement, response?.Result);
        }

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_401_No_Plural_Root_Element()
        {
            var response = DevKit.UpdateInStore(ObjectTypes.ToolErrorTermSet, QueryInvalidPluralRoot, null, null);
            Assert.AreEqual((short)ErrorCodes.MissingPluralRootElement, response?.Result);
        }

        [TestMethod]
        public void ToolErrorTermSet141Validator_DeleteFromStore_Error_401_No_Plural_Root_Element()
        {
            var response = DevKit.DeleteFromStore(ObjectTypes.ToolErrorTermSet, QueryInvalidPluralRoot, null, null);
            Assert.AreEqual((short)ErrorCodes.MissingPluralRootElement, response?.Result);
        }

        #endregion Error -401

        #region Error -402

        #endregion Error -402

        #region Error -403

        [TestMethod]
        public void ToolErrorTermSet141Validator_GetFromStore_Error_403_RequestObjectSelectionCapability_True_MissingNamespace()
        {
            var response = DevKit.GetFromStore(ObjectTypes.ToolErrorTermSet, QueryMissingNamespace, null, optionsIn: OptionsIn.RequestObjectSelectionCapability.True);
            Assert.AreEqual((short)ErrorCodes.MissingDefaultWitsmlNamespace, response.Result);
        }

        [TestMethod]
        public void ToolErrorTermSet141Validator_GetFromStore_Error_403_RequestObjectSelectionCapability_True_BadNamespace()
        {
            var response = DevKit.GetFromStore(ObjectTypes.ToolErrorTermSet, QueryInvalidNamespace, null, optionsIn: OptionsIn.RequestObjectSelectionCapability.True);
            Assert.AreEqual((short)ErrorCodes.MissingDefaultWitsmlNamespace, response.Result);
        }

        [TestMethod]
        public void ToolErrorTermSet141Validator_GetFromStore_Error_403_RequestObjectSelectionCapability_None_BadNamespace()
        {
            var response = DevKit.GetFromStore(ObjectTypes.ToolErrorTermSet, QueryInvalidNamespace, null, optionsIn: OptionsIn.RequestObjectSelectionCapability.None);
            Assert.AreEqual((short)ErrorCodes.MissingDefaultWitsmlNamespace, response.Result);
        }

        #endregion Error -403

        #region Error -405

        [TestMethod]
        public void ToolErrorTermSet141Validator_AddToStore_Error_405_ToolErrorTermSet_Already_Exists()
        {
            AddParents();
            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet);
            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet, ErrorCodes.DataObjectUidAlreadyExists);
        }

        #endregion Error -405

        #region Error -407

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_407_ToolErrorTermSet_Missing_Witsml_Object_Type()
        {
            AddParents();
            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet);
            var response = DevKit.Update<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet, string.Empty);
            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.MissingWmlTypeIn, response.Result);
        }

        #endregion Error -407

        #region Error -408

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_408_ToolErrorTermSet_Empty_QueryIn()
        {
            var response = DevKit.UpdateInStore(ObjectTypes.ToolErrorTermSet, string.Empty, null, null);
            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.MissingInputTemplate, response.Result);
        }

        #endregion Error -408

        #region Error -409

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_409_ToolErrorTermSet_QueryIn_Must_Conform_To_Schema()
        {
            AddParents();
            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet);

            var nonConformingXml = string.Format(BasicXMLTemplate, ToolErrorTermSet.Uid,

                $"<name>{ToolErrorTermSet.Name}</name><name>{ToolErrorTermSet.Name}</name>");

            var response = DevKit.UpdateInStore(ObjectTypes.ToolErrorTermSet, nonConformingXml, null, null);
            Assert.AreEqual((short)ErrorCodes.InputTemplateNonConforming, response.Result);
        }

        #endregion Error -409

        #region Error -415

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_415_ToolErrorTermSet_Update_Without_Specifing_UID()
        {
            AddParents();
            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet);
            ToolErrorTermSet.Uid = string.Empty;
            DevKit.UpdateAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet, ErrorCodes.DataObjectUidMissing);
        }

        #endregion Error -415

        #region Error -416

        [TestMethod]
        public void ToolErrorTermSet141Validator_DeleteFromStore_Error_416_ToolErrorTermSet_Delete_With_Empty_UID()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");
            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet);

            var deleteXml = string.Format(BasicXMLTemplate,ToolErrorTermSet.Uid,

                "<commonData><extensionNameValue uid=\"\" /></commonData>");

            var results = DevKit.DeleteFromStore(ObjectTypes.ToolErrorTermSet, deleteXml, null, null);

            Assert.IsNotNull(results);
            Assert.AreEqual((short)ErrorCodes.EmptyUidSpecified, results.Result);
        }

        #endregion Error -416

        #region Error -418

        [TestMethod]
        public void ToolErrorTermSet141Validator_DeleteFromStore_Error_418_ToolErrorTermSet_Delete_With_Missing_Uid()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");
            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet);

            var deleteXml = string.Format(BasicXMLTemplate,ToolErrorTermSet.Uid,

                "<commonData><extensionNameValue  uid=\"\" /></commonData>");

            var results = DevKit.DeleteFromStore(ObjectTypes.ToolErrorTermSet, deleteXml, null, null);

            Assert.IsNotNull(results);
            Assert.AreEqual((short)ErrorCodes.EmptyUidSpecified, results.Result);
        }

        #endregion Error -418

        #region Error -419

        [TestMethod]
        public void ToolErrorTermSet141Validator_DeleteFromStore_Error_419_ToolErrorTermSet_Deleting_Empty_NonRecurring_Container_Element()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");
            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet);

            var deleteXml = string.Format(BasicXMLTemplate,ToolErrorTermSet.Uid,

                "<commonData />");

            var results = DevKit.DeleteFromStore(ObjectTypes.ToolErrorTermSet, deleteXml, null, null);

            Assert.IsNotNull(results);
            Assert.AreEqual((short)ErrorCodes.EmptyNonRecurringElementSpecified, results.Result);
        }

        #endregion Error -419

        #region Error -420

        [TestMethod]
        public void ToolErrorTermSet141Validator_DeleteFromStore_Error_420_ToolErrorTermSet_Specifying_A_Non_Recuring_Element_That_Is_Required()
        {

            DevKit.AddAndAssert(ToolErrorTermSet);

            var deleteXml = string.Format(BasicXMLTemplate,ToolErrorTermSet.Uid,

                "<name />");
            var results = DevKit.DeleteFromStore(ObjectTypes.ToolErrorTermSet, deleteXml, null, null);

            Assert.IsNotNull(results);
            Assert.AreEqual((short)ErrorCodes.EmptyMandatoryNodeSpecified, results.Result);
        }

        #endregion Error -420

        #region Error -433

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_433_ToolErrorTermSet_Does_Not_Exist()
        {
            AddParents();
            DevKit.UpdateAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet, ErrorCodes.DataObjectNotExist);
        }

        #endregion Error -433

        #region Error -438

        [TestMethod]
        public void ToolErrorTermSet141Validator_GetFromStore_Error_438_ToolErrorTermSet_Recurring_Elements_Have_Inconsistent_Selection()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");
            var ext2 = DevKit.ExtensionNameValue("Ext-2", "1.0", "m");

            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1, ext2
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet);

            var queryXml = string.Format(BasicXMLTemplate,ToolErrorTermSet.Uid,

                "<commonData>" +
                $"<extensionNameValue uid=\"\"><name>Ext-1</name></extensionNameValue>" +
                "<extensionNameValue uid=\"\"><value uom=\"\">1.0</value></extensionNameValue>" +
                "</commonData>");

            var results = DevKit.GetFromStore(ObjectTypes.ToolErrorTermSet, queryXml, null, null);

            Assert.IsNotNull(results);
            Assert.AreEqual((short)ErrorCodes.RecurringItemsInconsistentSelection, results.Result);
        }

        #endregion Error -438

        #region Error -439

        [TestMethod]
        public void ToolErrorTermSet141Validator_GetFromStore_Error_439_ToolErrorTermSet_Recurring_Elements_Has_Empty_Selection_Value()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");
            var ext2 = DevKit.ExtensionNameValue("Ext-2", "1.0", "m");

            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1, ext2
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet);

            var queryXml = string.Format(BasicXMLTemplate,ToolErrorTermSet.Uid,

                "<commonData>" +
                $"<extensionNameValue uid=\"\"><name>Ext-1</name></extensionNameValue>" +
                "<extensionNameValue uid=\"\"><name></name></extensionNameValue>" +
                "</commonData>");

            var results = DevKit.GetFromStore(ObjectTypes.ToolErrorTermSet, queryXml, null, null);

            Assert.IsNotNull(results);
            Assert.AreEqual((short)ErrorCodes.RecurringItemsEmptySelection, results.Result);
        }

        #endregion Error -439

        #region Error -444

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_444_ToolErrorTermSet_Updating_More_Than_One_Data_Object()
        {
            AddParents();
            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet);

            var updateXml = "<toolErrorTermSets xmlns=\"http://www.witsml.org/schemas/1series\" version=\"1.4.1.1\"><toolErrorTermSet uid=\"{0}\"></toolErrorTermSet><toolErrorTermSet uid=\"{0}\"></toolErrorTermSet></toolErrorTermSets>";
            updateXml = string.Format(updateXml, ToolErrorTermSet.Uid);

            var response = DevKit.UpdateInStore(ObjectTypes.ToolErrorTermSet, updateXml, null, null);
            Assert.AreEqual((short)ErrorCodes.InputTemplateMultipleDataObjects, response.Result);
        }

        #endregion Error -444

        #region Error -445

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_445_ToolErrorTermSet_Empty_New_Element()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");

            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet);

            ext1 = DevKit.ExtensionNameValue("Ext-1", string.Empty, string.Empty, PrimitiveType.@double, string.Empty);
            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1
                }
            };

            DevKit.UpdateAndAssert(ToolErrorTermSet, ErrorCodes.EmptyNewElementsOrAttributes);
        }

        #endregion Error -445

        #region Error -448

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_448_ToolErrorTermSet_Missing_Uid()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");

            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet);

            var updateXml = string.Format(BasicXMLTemplate,ToolErrorTermSet.Uid,

                "<commonData>" +
                $"<extensionNameValue uid=\"\"><value uom=\"ft\" /></extensionNameValue>" +
                "</commonData>");

            var response = DevKit.UpdateInStore(ObjectTypes.ToolErrorTermSet, updateXml, null, null);
            Assert.AreEqual((short)ErrorCodes.MissingElementUidForUpdate, response.Result);
        }

        #endregion Error -448

        #region Error -464

        [TestMethod]
        public void ToolErrorTermSet141Validator_AddToStore_Error_464_ToolErrorTermSet_Uid_Not_Unique()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");
            var ext2 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");

            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1, ext2
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet, ErrorCodes.ChildUidNotUnique);
        }

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_464_ToolErrorTermSet_Uid_Not_Unique()
        {

            var ext1 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");

            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1
                }
            };

            DevKit.AddAndAssert(ToolErrorTermSet);

            var ext2 = DevKit.ExtensionNameValue("Ext-1", "1.0", "m");

            ToolErrorTermSet.CommonData = new CommonData
            {
                ExtensionNameValue = new List<ExtensionNameValue>
                {
                    ext1, ext2
                }
            };

            DevKit.UpdateAndAssert(ToolErrorTermSet, ErrorCodes.ChildUidNotUnique);
        }

        #endregion Error -464

        #region Error -468

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_468_ToolErrorTermSet_No_Schema_Version_Declared()
        {

            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet);
            var response = DevKit.UpdateInStore(ObjectTypes.ToolErrorTermSet, QueryMissingVersion, null, null);
            Assert.AreEqual((short)ErrorCodes.MissingDataSchemaVersion, response.Result);
        }

        #endregion Error -468

        #region Error -483

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_483_ToolErrorTermSet_Update_With_Non_Conforming_Template()
        {
            AddParents();
            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet);
            var response = DevKit.UpdateInStore(ObjectTypes.ToolErrorTermSet, QueryEmptyRoot, null, null);
            Assert.AreEqual((short)ErrorCodes.UpdateTemplateNonConforming, response.Result);
        }

        #endregion Error -483

        #region Error -484

        [TestMethod]
        public void ToolErrorTermSet141Validator_UpdateInStore_Error_484_ToolErrorTermSet_Update_Will_Delete_Required_Element()
        {

            DevKit.AddAndAssert<ToolErrorTermSetList, ToolErrorTermSet>(ToolErrorTermSet);

            var nonConformingXml = string.Format(BasicXMLTemplate, ToolErrorTermSet.Uid,

                $"<name></name>");

            var response = DevKit.UpdateInStore(ObjectTypes.ToolErrorTermSet, nonConformingXml, null, null);
            Assert.AreEqual((short)ErrorCodes.MissingRequiredData, response.Result);
        }

        #endregion Error -484

        #region Error -486

        [TestMethod]
        public void ToolErrorTermSet141Validator_AddToStore_Error_486_ToolErrorTermSet_Data_Object_Types_Dont_Match()
        {

            var xmlIn = string.Format(BasicXMLTemplate, ToolErrorTermSet.Uid,

                string.Empty);

            var response = DevKit.AddToStore(ObjectTypes.Wellbore, xmlIn, null, null);

            Assert.AreEqual((short)ErrorCodes.DataObjectTypesDontMatch, response.Result);
        }

        #endregion Error -486

    }
}