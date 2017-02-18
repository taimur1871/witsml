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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Energistics.DataAccess;
using Energistics.DataAccess.WITSML141;
using Energistics.DataAccess.WITSML141.ComponentSchemas;
using Energistics.DataAccess.WITSML141.ReferenceData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Framework;
using PDS.Witsml.Server.Configuration;

namespace PDS.Witsml.Server.Data.Wells
{
    [TestClass]
    public partial class Well141DataAdapterGetTests : Well141TestBase
    {
        private static readonly string _inputXmlTemplate =
            "<wells xmlns=\"http://www.witsml.org/schemas/1series\" version=\"1.4.1.1\">" + Environment.NewLine +
            "<well uid=\"" + "{0}" + "\">" + Environment.NewLine +
            "<name>{1}</name>" + Environment.NewLine +
            "<dTimLicense>" + "{2}" + "</dTimLicense>" + Environment.NewLine +
            "<timeZone>-06:00</timeZone>" + Environment.NewLine +
            "</well>" + Environment.NewLine +
            "</wells>";

        private static readonly string _xmlInMeasureData =
            "<wells xmlns=\"http://www.witsml.org/schemas/1series\" version=\"1.4.1.1\">" + Environment.NewLine +
            "   <well uid=\"" + "{0}" + "\">" + Environment.NewLine +
            "     <name>" + "{1}" + "</name>" + Environment.NewLine +
            "     <timeZone>-06:00</timeZone>" + Environment.NewLine +
            "     <wellheadElevation uom=\"ft\">{2}</wellheadElevation>" + Environment.NewLine +
            "   </well>" + Environment.NewLine +
            "</wells>";
        
        [TestMethod]
        public void Well141DataProvider_GetFromStore_Query_OptionsIn_requestObjectSelectionCapability()
        {
            var well = new Well();
            var result = DevKit.Query<WellList, Well>(well, optionsIn: OptionsIn.RequestObjectSelectionCapability.True);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            well = result.FirstOrDefault();
            Assert.IsNotNull(well);
            Assert.AreEqual("abc", well.Uid);
            Assert.IsNotNull(well.StatusWell);
            Assert.IsTrue(well.WellLocation.Count == 1);
            Assert.AreEqual(1, well.PercentInterest.Value);
            Assert.IsNotNull(well.CommonData.DateTimeLastChange);
        }

        [TestMethod]
        public void Well141DataProvider_GetFromStore_Query_OptionsIn_PrivateGroupOnly()
        {
            // Prevent large debug log output
            WitsmlSettings.TruncateXmlOutDebugSize = 100;

            Well.Name = DevKit.Name("Well-add-01");
            var response = DevKit.Add<WellList, Well>(Well);
            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            CommonData commonData = new CommonData {PrivateGroupOnly = true};
            Well.Uid = DevKit.Uid();
            Well.Name = DevKit.Name("Well-add-02");  
            Well.CommonData = commonData;
            response = DevKit.Add<WellList, Well>(Well);
            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;
            var valid = !string.IsNullOrEmpty(uid);
            Assert.IsTrue(valid);

            var well = new Well();
            var result = DevKit.Query<WellList, Well>(well, optionsIn: OptionsIn.ReturnElements.All + ";" + OptionsIn.RequestPrivateGroupOnly.True);
            Assert.IsNotNull(result);

            var notPrivateGroupWells = result.Where(x =>
            {
                bool isPrivate = x.CommonData.PrivateGroupOnly ?? false;
                return !isPrivate;
            });
            Assert.IsFalse(notPrivateGroupWells.Any());

            well = result.FirstOrDefault(x => uid.Equals(x.Uid));
            Assert.IsNotNull(well);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Return_Element_All()
        {
            var well = DevKit.CreateTestWell();
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;

            var query = new Well { Uid = uid };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            Assert.AreEqual(1, result.Count);
            var returnWell = result.FirstOrDefault();
            AssertTestWell(well, returnWell);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Return_Element_Id_Only()
        {
            Well well, query, returnWell;
            string uid;
            CreateAndAssertTestWell(out well, out uid, out query, out returnWell);

            query = new Well { Uid = uid };
            string xmlOut;
            WitsmlQueryParser parser;
            GetQueryResultsAndParser(query, OptionsIn.ReturnElements.IdOnly, out xmlOut, out parser);
            Assert.IsFalse(parser.HasElements("wellDatum"));

            var wellList = EnergisticsConverter.XmlToObject<WellList>(xmlOut);
            Assert.AreEqual(1, wellList.Well.Count);
            returnWell = wellList.Well.FirstOrDefault();

            Assert.IsNotNull(returnWell);
            Assert.AreEqual(well.Name, returnWell.Name);
            Assert.IsNull(returnWell.DateTimeSpud);
            Assert.IsNull(returnWell.GroundElevation);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Return_Element_Id_Only_With_Additional_Elements()
        {
            Well well, query, returnWell;
            string uid;
            CreateAndAssertTestWell(out well, out uid, out query, out returnWell);

            query = new Well { Uid = uid, Country = string.Empty, CommonData = new CommonData() };
            string xmlOut;
            WitsmlQueryParser parser;
            GetQueryResultsAndParser(query, OptionsIn.ReturnElements.IdOnly, out xmlOut, out parser);

            Assert.IsTrue(parser.HasElements("country"));
            Assert.IsTrue(parser.HasElements("commonData"));
            Assert.IsFalse(parser.HasElements("wellDatum"));
            
            var wellList = EnergisticsConverter.XmlToObject<WellList>(xmlOut);
            Assert.AreEqual(1, wellList.Well.Count);
            returnWell = wellList.Well.FirstOrDefault();

            Assert.IsNotNull(returnWell);
            Assert.AreEqual(well.Name, returnWell.Name);
            Assert.AreEqual(well.Country, returnWell.Country);
            Assert.AreEqual(well.CommonData.ItemState.ToString(), returnWell.CommonData.ItemState.ToString());
            Assert.AreEqual(well.CommonData.Comments, returnWell.CommonData.Comments);
            Assert.IsNull(returnWell.DateTimeSpud);
            Assert.IsNull(returnWell.GroundElevation);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Return_Element_Default()
        {
            Well well, query, returnWell;
            string uid;
            CreateAndAssertTestWell(out well, out uid, out query, out returnWell);

            query = new Well { Uid = uid, WellDatum = new List<WellDatum> { new WellDatum() } };
            string xmlOut;
            WitsmlQueryParser parser;
            GetQueryResultsAndParser(query, null, out xmlOut, out parser);
            Assert.IsFalse(parser.HasElements("name"));

            var wellList = EnergisticsConverter.XmlToObject<WellList>(xmlOut);
            Assert.AreEqual(1, wellList.Well.Count);
            returnWell = wellList.Well.FirstOrDefault();

            Assert.IsNotNull(returnWell);
            Assert.IsNull(returnWell.DateTimeSpud);
            Assert.IsNull(returnWell.GroundElevation);
            Assert.IsNull(returnWell.CommonData);

            foreach (var datum in well.WellDatum)
            {
                var returnDatum = returnWell.WellDatum.FirstOrDefault(d => d.Uid == datum.Uid);
                Assert.IsNotNull(returnDatum);
                Assert.AreEqual(datum.Code, returnDatum.Code);
            }
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_ReturnElements_Requested1()
        {
            Well well, query, returnWell;
            string uid;
            CreateAndAssertTestWell(out well, out uid, out query, out returnWell);

            query = new Well { Uid = uid, CommonData = new CommonData { Comments = string.Empty } };
            var queryIn = EnergisticsConverter.ObjectToXml(new WellList { Well = new List<Well> { query } });
            var xmlOut = DevKit.GetFromStore(ObjectTypes.Well, queryIn, null, optionsIn: OptionsIn.ReturnElements.Requested).XMLout;
            var document = WitsmlParser.Parse(xmlOut);
            var parser = new WitsmlQueryParser(document.Root, ObjectTypes.Well, null);

            Assert.IsFalse(parser.HasElements("name"));
            Assert.IsFalse(parser.HasElements("wellDatum"));

            var wellList = EnergisticsConverter.XmlToObject<WellList>(xmlOut);
            Assert.AreEqual(1, wellList.Well.Count);
            returnWell = wellList.Well.FirstOrDefault();

            Assert.IsNotNull(returnWell);
            Assert.IsNull(returnWell.DateTimeSpud);
            Assert.IsNull(returnWell.GroundElevation);

            var commonData = returnWell.CommonData;

            Assert.IsNotNull(commonData);
            Assert.IsFalse(string.IsNullOrEmpty(commonData.Comments));
            Assert.IsNull(commonData.DateTimeLastChange);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore__ReturnElements_IdOnly()
        {
            var wellName = DevKit.Name("Well-to-add-01");
            var well = new Well { Name = wellName, TimeZone = DevKit.TimeZone, NameLegal = "Company Legal Name", Field = "Big Field" };
            var response = DevKit.Add<WellList, Well>(well);
            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var queryWell = new Well { Name = wellName };
            var result = DevKit.Get<WellList, Well>(DevKit.List(queryWell), optionsIn: OptionsIn.ReturnElements.IdOnly);
            Assert.IsNotNull(result);

            var xmlout = result.XMLout;
            var doc = new XmlDocument();
            doc.LoadXml(xmlout);
            var wells = doc.DocumentElement;

            Assert.IsNotNull(wells);

            var uidExists = false;
            foreach (XmlNode node in wells.ChildNodes)
            {
                uidExists = true;
                Assert.IsNotNull(node);
                Assert.IsTrue(node.Attributes?.Count == 1);
                Assert.IsTrue(node.HasChildNodes);
                Assert.AreEqual(1, node.ChildNodes.Count);
                Assert.AreEqual("name", node.ChildNodes[0].Name);
            }
            Assert.IsTrue(uidExists);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_ReturnElements_Requested2()
        {
            var wellName = DevKit.Name("Well-to-add-01");
            var well = new Well { Name = wellName, TimeZone = DevKit.TimeZone, NameLegal = "Company Legal Name", Field = "Big Field" };
            var response = DevKit.Add<WellList, Well>(well);
            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            string uid = response.SuppMsgOut;

            var queryWell = new Well { Uid = uid, Name = wellName, NameLegal = "", Field = "" };
            var result = DevKit.Get<WellList, Well>(DevKit.List(queryWell), optionsIn: OptionsIn.ReturnElements.Requested);
            Assert.IsNotNull(result);

            var xmlout = result.XMLout;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlout);
            XmlElement wells = doc.DocumentElement;
            Assert.IsNotNull(wells);

            bool uidExists = false;
            foreach (XmlNode node in wells.ChildNodes)
            {
                Assert.IsNotNull(node);
                Assert.AreEqual(1, node.Attributes?.Count);
                Assert.IsTrue(node.HasChildNodes);
                Assert.IsTrue(node.ChildNodes.Count <= 3);
                Assert.AreEqual("name", node.ChildNodes[0].Name);
                Assert.AreEqual(wellName, node.ChildNodes[0].InnerText);

                if (uid.Equals(node.Attributes?[0].InnerText))
                {
                    uidExists = true;
                    Assert.AreEqual("nameLegal", node.ChildNodes[1].Name);
                    Assert.AreEqual("Company Legal Name", node.ChildNodes[1].InnerText);
                    Assert.AreEqual("field", node.ChildNodes[2].Name);
                    Assert.AreEqual("Big Field", node.ChildNodes[2].InnerText);
                }
            }

            Assert.IsTrue(uidExists);
        }

        [TestMethod]
        public void WitsmlDataProvider_GetFromStore_Get_FullWell()
        {
            Well well, query, returnWell;
            string uid;
            CreateAndAssertTestWell(out well, out uid, out query, out returnWell);

            well.Uid = uid;
            well.CommonData.DateTimeCreation = returnWell.CommonData.DateTimeCreation;
            well.CommonData.DateTimeLastChange = returnWell.CommonData.DateTimeLastChange;                        
            string wellXml = EnergisticsConverter.ObjectToXml(well);
            string returnXml = EnergisticsConverter.ObjectToXml(returnWell);

            Assert.AreEqual(wellXml, returnXml);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_Uid_ReturnElement_All_dTimLicense_With_Offset()
        {
            string inputXml = string.Format(_inputXmlTemplate,
                string.Empty, "Full Test Well With Offset", "2001-05-15T13:20:00-05:00");

            WellList wells = EnergisticsConverter.XmlToObject<WellList>(inputXml);
            var well = wells.Items[0] as Well;
            Assert.IsNotNull(well);
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;
            var query = new Well { Uid = uid };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            Assert.AreEqual(1, result.Count);
            var returnWell = result.FirstOrDefault();
            Assert.IsNotNull(returnWell);

            well.Uid = uid;
            well.CommonData = returnWell.CommonData;
            string wellXml = EnergisticsConverter.ObjectToXml(well);
            string returnXml = EnergisticsConverter.ObjectToXml(returnWell);

            Assert.AreEqual(wellXml, returnXml);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_By_dTimLicense_With_Custom_Timestamp()
        {
            var uid = DevKit.Uid();
            var timeStr = "2001-05-15T13:20:00.0000000+00:00";

            string inputXml = string.Format(_inputXmlTemplate, uid, "Full Test Well Custom Timestamp", timeStr);

            WellList wells = EnergisticsConverter.XmlToObject<WellList>(inputXml);
            var well = wells.Items[0] as Well;
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var result = DevKit.GetFromStore(ObjectTypes.Well, inputXml, null, null);
            var results = EnergisticsConverter.XmlToObject<WellList>(result.XMLout).Well;

            Assert.AreEqual(1, results.Count);
            var returnWell = results.FirstOrDefault();

            Assert.IsNotNull(returnWell);
            Assert.IsNotNull(returnWell.DateTimeLicense);
            Assert.AreEqual(timeStr, returnWell.DateTimeLicense.Value.ToString());
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_Uid_Caseless_Compare()
        {
            var testUid = "Uid_Caseless_" + DevKit.Uid();
            var query = new Well { Uid = testUid };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.IdOnly);

            if (result.Count == 0)
            {
                var well = DevKit.GetFullWell();
                well.Uid = testUid;
                var response = DevKit.Add<WellList, Well>(well);

                Assert.IsNotNull(response);
                Assert.AreEqual((short)ErrorCodes.Success, response.Result);
            }

            query = new Well { Uid = testUid.ToUpper()};
            result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            Assert.IsTrue(result.Any(x => x.Uid == testUid));
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_Different_Case()
        {
            var well = DevKit.GetFullWell();
            well.Uid = DevKit.Uid();
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;
            var query = new Well { Uid = "", Name = well.Name.ToLower(), NameLegal = well.NameLegal.ToUpper() };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            Assert.IsTrue(result.Any(x => x.Uid == uid));
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_Criteria_Not_Satisfied()
        {
            var dummy = "Dummy";
            var datumKb = DevKit.WellDatum(dummy);
            var query = new Well { Uid = dummy, Name = dummy, NameLegal = dummy, Country=dummy, County=dummy, WellDatum = DevKit.List(datumKb) };
            var result = DevKit.Get<WellList, Well>(DevKit.List(query), ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);
            Assert.IsNotNull(result);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result.XMLout);
            XmlElement wells = doc.DocumentElement;
            Assert.IsNotNull(wells);

            // Section 6.6.4
            Assert.AreEqual(ObjectTypes.SingleToPlural(ObjectTypes.Well), wells.Name);
            Assert.IsFalse(DevKit.HasChildNodes(wells));
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_MultiQueries_Same_Object_Returned()
        {
            var well = DevKit.GetFullWell();
            well.Uid = DevKit.Uid();
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;

            var datumKb = DevKit.WellDatum("Kelly Bushing");       
            var query1 = new Well { Uid = "", WellDatum = DevKit.List(datumKb) };
            var query2 = new Well { Uid = uid };
            var result = DevKit.Get<WellList, Well>(DevKit.List(query1, query2), ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            Assert.IsNotNull(result.XMLout);
            var resultWellList = EnergisticsConverter.XmlToObject<WellList>(result.XMLout);

            Assert.IsNotNull(resultWellList);
            var sameWellList = resultWellList.Items.Cast<Well>().Where(x => x.Uid == uid);

            // Section 6.6.4.1
            Assert.IsTrue(sameWellList.Count() > 1);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_MultiQueries_One_Query_Fails()
        {
            var well = DevKit.GetFullWell();
            well.Uid = DevKit.Uid();
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;

            var datumKb = DevKit.WellDatum("Kelly Bushing", ElevCodeEnum.KB);
            var datumSl = DevKit.WellDatum(null, ElevCodeEnum.SL);

            var badWellQuery = new Well { Uid = "", WellDatum = DevKit.List(datumKb, datumSl) };
            var goodWellQuery = new Well { Uid = uid };

            var result = DevKit.Get<WellList, Well>(DevKit.List(goodWellQuery, badWellQuery), ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            // Section 6.6.4 
            Assert.AreEqual((short)ErrorCodes.RecurringItemsInconsistentSelection, result.Result);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_Not_Equal_Comparison_dTimCreation()
        {
            var well01 = DevKit.GetFullWell();
            well01.Uid = DevKit.Uid();
            var response = DevKit.Add<WellList, Well>(well01);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid01 = response.SuppMsgOut;
            var now = DateTimeOffset.UtcNow;

            var well02 = DevKit.GetFullWell();
            well02.Uid = DevKit.Uid();
            response = DevKit.Add<WellList, Well>(well02);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid02 = response.SuppMsgOut;

            var query = new Well { CommonData = new CommonData() };
            query.CommonData.DateTimeCreation = now;
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            // Section 6.6.4
            Assert.IsTrue(result.Any(x => x.Uid == uid02));
            Assert.IsFalse(result.Any(x => x.Uid == uid01));
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_Not_Equal_Comparison_dTimLastChange()
        {
            var well01 = DevKit.GetFullWell();
            well01.Uid = DevKit.Uid();
            var response = DevKit.Add<WellList, Well>(well01);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);
            var uid01 = response.SuppMsgOut;

            var query = new Well { Uid = uid01 };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(uid01, result[0].Uid);

            var wellLastChangeTime = result[0].CommonData.DateTimeLastChange;

            var well02 = DevKit.GetFullWell();
            well02.Uid = DevKit.Uid();
            well02.CommonData.DateTimeCreation = DateTimeOffset.UtcNow;
            response = DevKit.Add<WellList, Well>(well02);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);
            var uid02 = response.SuppMsgOut;

            query = new Well { CommonData = new CommonData() };
            query.CommonData.DateTimeLastChange = wellLastChangeTime;
            result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            // Section 6.6.4
            Assert.IsTrue(result.Any(x => x.Uid == uid02));
            Assert.IsFalse(result.Any(x => x.Uid == uid01));
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_Do_Not_Return_Empty_Values()
        {
            var well = DevKit.CreateTestWell();
            Assert.IsNull(well.WaterDepth);
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;

            var query = new Well { Uid = uid };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);
            Assert.AreEqual(1, result.Count);

            // Section 6.6.4.1 
            Assert.IsNull(result[0].WaterDepth);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Selection_Recurring_Items()
        {
            var well = DevKit.GetFullWell();
            well.Uid = DevKit.Uid();
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;

            var datumKb = DevKit.WellDatum("Kelly Bushing", ElevCodeEnum.KB);
            var datumSl = DevKit.WellDatum("Sea Level", ElevCodeEnum.SL);
            var query = new Well { Uid = "", WellDatum = DevKit.List(datumKb,  datumSl) };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            Assert.IsTrue(result.Any(x => x.Uid == uid));
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStrore_Selection_Recurring_Items_Criteria_OR()
        {
            var well01 = DevKit.GetFullWell();
            well01.Uid = DevKit.Uid();
            well01.WellDatum.RemoveAt(0);            
            var response = DevKit.Add<WellList, Well>(well01);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);
            var uid01 = response.SuppMsgOut;

            var well02 = DevKit.GetFullWell();
            well02.Uid = DevKit.Uid();
            well02.WellDatum.RemoveAt(1);
            response = DevKit.Add<WellList, Well>(well02);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);
            var uid02 = response.SuppMsgOut;

            var datumKb = DevKit.WellDatum("Kelly Bushing", ElevCodeEnum.KB);
            var datumSl = DevKit.WellDatum("Sea Level", ElevCodeEnum.SL);
            var query = new Well { WellDatum = DevKit.List(datumKb, datumSl) };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            // Section 4.1.5
            Assert.IsTrue(result.Any(x => x.Uid == uid01));
            Assert.IsTrue(result.Any(x => x.Uid == uid02));
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Supports_NaN_With_Property_And_XElement_Name_Not_Same()
        {
            // Add well
            Well.PercentInterest = new DimensionlessMeasure(99.8, DimensionlessUom.Euc);
            var response = DevKit.Add<WellList, Well>(Well);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uidWell = response.SuppMsgOut;

            // Query well with NaN
            var queryIn = "<?xml version=\"1.0\"?>" + Environment.NewLine +
                "<wells version=\"1.4.1.1\" xmlns=\"http://www.witsml.org/schemas/1series\">" + Environment.NewLine +
                    "<well uid=\"" + uidWell + "\">" + Environment.NewLine +
                         "<pcInterest uom=\"Euc\">NaN</pcInterest>" + Environment.NewLine +
                    "</well>" + Environment.NewLine +
               "</wells>";

            var results = DevKit.GetFromStore(ObjectTypes.Well, queryIn, null, "returnElements=requested");
            Assert.AreEqual((short)ErrorCodes.Success, results.Result);

            var wellList = EnergisticsConverter.XmlToObject<WellList>(results.XMLout);
            Assert.AreEqual(1, wellList.Well.Count);
            Assert.AreEqual("Euc", wellList.Well[0].PercentInterest.Uom.ToString());
            Assert.AreEqual(99.8, wellList.Well[0].PercentInterest.Value);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Supports_NaN_On_Class_Property()
        {
            // Add well
            Well.WellDatum = new List<WellDatum>();
            var datum = DevKit.WellDatum("Kelly Bushing", code: ElevCodeEnum.KB, uid: ElevCodeEnum.KB.ToString());
            datum.Elevation = new WellElevationCoord() { Uom = WellVerticalCoordinateUom.ft, Value = 99.8 };
            Well.WellDatum.Add(datum);

            var response = DevKit.Add<WellList, Well>(Well);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uidWell = response.SuppMsgOut;

            // Query well with NaN
            var queryIn = "<?xml version=\"1.0\"?>" + Environment.NewLine +
                "<wells version=\"1.4.1.1\" xmlns=\"http://www.witsml.org/schemas/1series\">" + Environment.NewLine +
                    "<well uid=\"" + uidWell + "\">" + Environment.NewLine +
                           "<wellDatum uid=\"KB\">" + Environment.NewLine +
                           "    <name>Kelly Bushing</name>" + Environment.NewLine +
                           "    <code>KB</code>" + Environment.NewLine +
                           "    <elevation uom=\"ft\">NaN</elevation>" + Environment.NewLine +
                           "</wellDatum>" + Environment.NewLine +
                    "</well>" + Environment.NewLine +
               "</wells>";

            var results = DevKit.GetFromStore(ObjectTypes.Well, queryIn, null, "returnElements=requested");
            Assert.AreEqual((short)ErrorCodes.Success, results.Result);

            var wellList = EnergisticsConverter.XmlToObject<WellList>(results.XMLout);
            Assert.AreEqual(1, wellList.Well.Count);
            Assert.AreEqual("Kelly Bushing", wellList.Well[0].WellDatum[0].Name);
            Assert.AreEqual(99.8, wellList.Well[0].WellDatum[0].Elevation.Value);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Can_Get_Measure_Data_With_Uom_And_Null()
        {
            Get_Measure_Data_With_Value(string.Empty);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Can_Get_Measure_Data_With_Uom_And_NaN()
        {
            Get_Measure_Data_With_Value("NaN");
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Can_Get_Uom_Data_OptionsIn_Requested()
        {
            // Add well
            var well = DevKit.GetFullWell();
            well.Uid = DevKit.Uid();
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;

            string xmlIn = "<wells xmlns=\"http://www.witsml.org/schemas/1series\" version=\"1.4.1.1\">" + Environment.NewLine +
                           "   <well uid=\"" + uid + "\">" + Environment.NewLine +
                           "     <name>" + well.Name + "</name>" + Environment.NewLine +
                           "    <pcInterest uom=\"" + well.PercentInterest.Uom + "\">" + "</pcInterest>" + Environment.NewLine +
                           "    <wellDatum uid=\"" + well.WellDatum[0].Uid + "\">" + Environment.NewLine +
                           "      <name>" + well.WellDatum[0].Name + "</name>" + Environment.NewLine +
                           "      <code>" + well.WellDatum[0].Code + "</code>" + Environment.NewLine +
                           "      <elevation uom=\"" + well.WellDatum[0].Elevation.Uom + "\">" + "</elevation>" + Environment.NewLine +
                           "    </wellDatum>" + Environment.NewLine +
                           "   </well>" + Environment.NewLine +
                           "</wells>";

            // Make a requested query
            var getResponse = DevKit.GetFromStore(ObjectTypes.Well, xmlIn, null, "returnElements=requested");
            Assert.IsNotNull(getResponse);
            Assert.AreEqual((short)ErrorCodes.Success, getResponse.Result);

            // Convert the XMLout to a well list.
            var wellList = EnergisticsConverter.XmlToObject<WellList>(getResponse.XMLout);

            // Test that our well was returned in the output
            Assert.AreEqual(1, wellList.Well.Count);

            // Test that the queriedWell's uom and uom values are the same as the added well after a requested query
            var queriedWell = wellList.Well[0];
            Assert.AreEqual(well.PercentInterest.Uom, queriedWell.PercentInterest.Uom);
            Assert.AreEqual(well.PercentInterest.Value, queriedWell.PercentInterest.Value);
            Assert.AreEqual(well.WellDatum[0].Elevation.Uom, queriedWell.WellDatum[0].Elevation.Uom);
            Assert.AreEqual(well.WellDatum[0].Elevation.Value, queriedWell.WellDatum[0].Elevation.Value);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Can_GetWell_And_Ignore_Invalid_Element()
        {
            Well.Name = DevKit.Name("Bug-5855-GetFromStore-Bad-Element");
            Well.Operator = "AAA Company";

            var response = DevKit.Add<WellList, Well>(Well);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uidWell = response.SuppMsgOut;

            // Query well with invalid element
            var queryIn = string.Format(BasicXMLTemplate, uidWell,
                "<operator/>" +
                "<fieldsssssss>Big Field</fieldsssssss>");

            var results = DevKit.GetFromStore(ObjectTypes.Well, queryIn, null, "returnElements=requested");
            Assert.AreEqual((short)ErrorCodes.Success, results.Result);

            var wellList = EnergisticsConverter.XmlToObject<WellList>(results.XMLout);
            Assert.AreEqual(1, wellList.Well.Count);
            Assert.AreEqual("AAA Company", wellList.Well[0].Operator);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Can_GetWell_And_Ignore_Invalid_Attribute()
        {
            Well.Name = DevKit.Name("Bug-5855-GetFromStore-Bad-Attribute");
            Well.Operator = "AAA Company";
            Well.Field = "Very Big Field";

            var response = DevKit.Add<WellList, Well>(Well);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uidWell = response.SuppMsgOut;

            // Query well with invalid attribute
            var queryIn = string.Format(BasicXMLTemplate, uidWell,
                "<operator/>" +
                "<field abc=\"abc\"></field>");

            var results = DevKit.GetFromStore(ObjectTypes.Well, queryIn, null, "returnElements=requested");
            Assert.AreEqual((short)ErrorCodes.Success, results.Result);

            var wellList = EnergisticsConverter.XmlToObject<WellList>(results.XMLout);
            Assert.AreEqual(1, wellList.Well.Count);
            Assert.AreEqual("AAA Company", wellList.Well[0].Operator);
            Assert.AreEqual("Very Big Field", wellList.Well[0].Field);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Can_GetWell_With_Invalid_Child_Element()
        {
            Well.Name = DevKit.Name("Bug-5855-UpdateInStore-Invalid-Child-Element");
            Well.Operator = "AAA Company";

            var response = DevKit.Add<WellList, Well>(Well);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uidWell = response.SuppMsgOut;

            // Query well with invalid attribute
            var queryIn = string.Format(BasicXMLTemplate, uidWell,
                "<name/>" +
                "<operator><abc>BBB Company</abc></operator>");

            var results = DevKit.GetFromStore(ObjectTypes.Well, queryIn, null, "returnElements=requested");
            Assert.AreEqual((short)ErrorCodes.Success, results.Result);

            var wellList = EnergisticsConverter.XmlToObject<WellList>(results.XMLout);
            Assert.AreEqual(1, wellList.Well.Count);
            Assert.AreEqual(Well.Name, wellList.Well[0].Name);
            Assert.AreEqual("AAA Company", wellList.Well[0].Operator);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Parse_DocumentInfo_Element()
        {
            string queryInWithDocumentInfo = "<wells xmlns=\"http://www.witsml.org/schemas/1series\" version=\"1.4.1.1\"><documentInfo /><well /></wells>";
            var response = DevKit.GetFromStore(ObjectTypes.Well, queryInWithDocumentInfo, null, "");
            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var wellListObject = EnergisticsConverter.XmlToObject<WellList>(response.XMLout);
            Assert.IsNotNull(wellListObject);
            Assert.IsNotNull(wellListObject.DocumentInfo);
            Assert.AreEqual(ObjectTypes.Well, wellListObject.DocumentInfo.DocumentName.Value);
        }

        [TestMethod]
        public void Well141DataAdapter_GetFromStore_Ignore_Uom_Attributes()
        {
            var well = DevKit.CreateTestWell();
            var responseAddWell = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(responseAddWell);
            Assert.AreEqual((short)ErrorCodes.Success, responseAddWell.Result);

            var uid = responseAddWell.SuppMsgOut;

            string queryIn = @" <wells xmlns=""http://www.witsml.org/schemas/1series"" version=""1.4.1.1"">                                
                                <well uid=""" + uid + @""">
                                    <name />
                                    <groundElevation uom=""m"" />
                                    <measuredDepth uom=""ft"" /> 
                                    <waterDepth uom=""ft"" />                               
                                </well>
                                </wells>";

            var response = DevKit.GetFromStore(ObjectTypes.Well, queryIn, null, "");
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var wellListObject = EnergisticsConverter.XmlToObject<WellList>(response.XMLout);
            Assert.IsNotNull(wellListObject);
            Assert.AreEqual(1, wellListObject.Well.Count);
            Assert.AreEqual(uid, wellListObject.Well[0].Uid);
            Assert.AreEqual(well.Name, wellListObject.Well[0].Name);
        }

        private void AssertTestWell(Well expected, Well actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Country, actual.Country);
            Assert.AreEqual(expected.DateTimeSpud.ToString(), actual.DateTimeSpud.ToString());
            Assert.AreEqual(expected.GroundElevation.Value, actual.GroundElevation.Value);
            Assert.AreEqual(expected.WellDatum.Count, actual.WellDatum.Count);

            foreach (var datum in expected.WellDatum)
            {
                var returnDatum = actual.WellDatum.FirstOrDefault(d => d.Uid == datum.Uid);
                Assert.IsNotNull(returnDatum);
                Assert.AreEqual(datum.Code, returnDatum.Code);
            }

            Assert.IsNotNull(actual.CommonData);
            Assert.IsNotNull(actual.CommonData.DateTimeLastChange);
            Assert.AreEqual(expected.CommonData.ItemState, actual.CommonData.ItemState);
            Assert.AreEqual(expected.CommonData.Comments, actual.CommonData.Comments);
        }

        private void Get_Measure_Data_With_Value(string measureDataValue)
        {
            // Add well
            var well = DevKit.GetFullWell();
            well.Uid = DevKit.Uid();
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            var uid = response.SuppMsgOut;
            string xmlIn = string.Format(_xmlInMeasureData, uid, well.Name, measureDataValue);
            var getResponse = DevKit.GetFromStore(ObjectTypes.Well, xmlIn, null, null);

            Assert.IsNotNull(getResponse);
            Assert.AreEqual((short)ErrorCodes.Success, getResponse.Result);

            var wellList = EnergisticsConverter.XmlToObject<WellList>(getResponse.XMLout);
            Assert.AreEqual(1, wellList.Well.Count);
            Assert.AreEqual(500, wellList.Well[0].WellheadElevation.Value);
        }

        private void GetQueryResultsAndParser(Well query, OptionsIn.ReturnElements optionsIn, out string xmlOut, out WitsmlQueryParser parser)
        {
            var queryIn = EnergisticsConverter.ObjectToXml(new WellList { Well = new List<Well> { query } });
            xmlOut = DevKit.GetFromStore(ObjectTypes.Well, queryIn, null, optionsIn).XMLout;
            var document = WitsmlParser.Parse(xmlOut);
            parser = new WitsmlQueryParser(document.Root, ObjectTypes.Well, null);
        }

        private void CreateAndAssertTestWell(out Well well, out string uid, out Well query, out Well returnWell)
        {
            well = DevKit.CreateTestWell();
            var response = DevKit.Add<WellList, Well>(well);

            Assert.IsNotNull(response);
            Assert.AreEqual((short)ErrorCodes.Success, response.Result);

            uid = response.SuppMsgOut;
            query = new Well { Uid = uid };
            var result = DevKit.Query<WellList, Well>(query, ObjectTypes.Well, null, optionsIn: OptionsIn.ReturnElements.All);

            Assert.AreEqual(1, result.Count);
            returnWell = result.FirstOrDefault();
            AssertTestWell(well, returnWell);
        }
    }
}
