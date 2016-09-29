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
using Energistics.DataAccess;
using Energistics.DataAccess.WITSML131;
using Energistics.DataAccess.WITSML131.ComponentSchemas;
using Energistics.DataAccess.WITSML131.ReferenceData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Witsml.Data.Logs;
using PDS.Witsml.Data.Trajectories;

namespace PDS.Witsml.Server
{
    public class DevKit131Aspect : DevKitAspect
    {
        private const MeasuredDepthUom MdUom = MeasuredDepthUom.m;
        private const WellVerticalCoordinateUom TvdUom = WellVerticalCoordinateUom.m;
        private const PlaneAngleUom AngleUom = PlaneAngleUom.dega;

        public DevKit131Aspect(TestContext context, string url = null) : base(url, WMLSVersion.WITSML131, context)
        {
            LogGenerator = new Log131Generator();
            TrajectoryGenerator = new Trajectory131Generator();
        }

        public Log131Generator LogGenerator { get; }

        public Trajectory131Generator TrajectoryGenerator { get; }

        public override string DataSchemaVersion
        {
            get { return OptionsIn.DataVersion.Version131.Value; }
        }

        public void InitHeader(Log log, LogIndexType indexType, bool increasing = true)
        {
            log.IndexType = indexType;
            log.IndexCurve = new IndexCurve(indexType == LogIndexType.datetime ? "TIME" : "MD")
            {
                ColumnIndex = 1
            };

            log.Direction = increasing ? LogIndexDirection.increasing : LogIndexDirection.decreasing;
            log.LogCurveInfo = List<LogCurveInfo>();

            log.LogCurveInfo.Add(
                new LogCurveInfo()
                {
                    Uid = log.IndexCurve.Value,
                    Mnemonic = log.IndexCurve.Value,
                    TypeLogData = indexType == LogIndexType.datetime ? LogDataType.datetime : LogDataType.@double,
                    Unit = indexType == LogIndexType.datetime ? "s" : "m",
                    ColumnIndex = 1
                });

            log.LogCurveInfo.Add(
                new LogCurveInfo()
                {
                    Uid = "ROP",
                    Mnemonic = "ROP",
                    TypeLogData = LogDataType.@double,
                    Unit = "m/h",
                    ColumnIndex = 2
                });

            log.LogCurveInfo.Add(
                new LogCurveInfo()
                {
                    Uid = "GR",
                    Mnemonic = "GR",
                    TypeLogData = LogDataType.@double,
                    Unit = "gAPI",
                    ColumnIndex = 3
                });

            InitData(log, Mnemonics(log), Units(log));
        }

        public void InitData(Log log, string mnemonics, string units, params object[] values)
        {
            if (log.LogData == null)
            {
                log.LogData = List<string>();
            }

            if (values != null && values.Any())
            {
                log.LogData.Add(String.Join(",", values.Select(x => x == null ? string.Empty : x)));
            }
        }

        public void InitDataMany(Log log, string mnemonics, string units, int numRows, double factor = 1.0, bool isDepthLog = true, bool hasEmptyChannel = true, bool increasing = true)
        {
            var depthStart = log.StartIndex != null ? log.StartIndex.Value : 0;
            var timeStart = DateTimeOffset.UtcNow.AddDays(-1);
            var interval = increasing ? 1 : -1;

            for (int i = 0; i < numRows; i++)
            {
                if (isDepthLog)
                {
                    InitData(log, mnemonics, units, depthStart + i * interval, hasEmptyChannel ? (int?)null : i, depthStart + i * factor);
                }
                else
                {
                    InitData(log, mnemonics, units, timeStart.AddSeconds(i).ToString("o"), hasEmptyChannel ? (int?)null : i, i * factor);
                }
            }
        }

        public LogList QueryLogByRange(Log log, double? startIndex, double? endIndex)
        {
            var query = Query<LogList>();
            query.Log = One<Log>(x => { x.Uid = log.Uid; x.UidWell = log.UidWell; x.UidWellbore = log.UidWellbore; });
            var queryLog = query.Log.First();

            if (startIndex.HasValue)
            {
                queryLog.StartIndex = new GenericMeasure() { Value = startIndex.Value };
            }

            if (endIndex.HasValue)
            {
                queryLog.EndIndex = new GenericMeasure() { Value = endIndex.Value };
            }

            var result = Proxy.Read(query, OptionsIn.ReturnElements.All);
            return result;
        }

        public string Units(Log log)
        {
            return log.LogCurveInfo != null
                ? String.Join(",", log.LogCurveInfo.Select(x => x.Unit ?? string.Empty))
                : string.Empty;
        }

        public string Mnemonics(Log log)
        {
            return log.LogCurveInfo != null
                ? String.Join(",", log.LogCurveInfo.Select(x => x.Mnemonic))
                : string.Empty;
        }

        public Log CreateLog(string uid, string name, string uidWell, string nameWell, string uidWellbore, string nameWellbore)
        {
            return new Log()
            {
                Uid = uid,
                Name = name,
                UidWell = uidWell,
                NameWell = nameWell,
                UidWellbore = uidWellbore,
                NameWellbore = nameWellbore,
            };
        }

        /// <summary>
        /// Adds well object and test the return code
        /// </summary>
        /// <param name="well">the well</param>
        /// <param name="errorCode">the errorCode</param>
        public WMLS_AddToStoreResponse AddAndAssert(Well well, ErrorCodes errorCode = ErrorCodes.Success)
        {
            return AddAndAssert<WellList, Well>(well, errorCode);
        }

        /// <summary>
        /// Adds wellbore object and test the return code
        /// </summary>
        /// <param name="wellbore">the wellbore</param>
        /// <param name="errorCode">the errorCode</param>
        public WMLS_AddToStoreResponse AddAndAssert(Wellbore wellbore, ErrorCodes errorCode = ErrorCodes.Success)
        {
            return AddAndAssert<WellboreList, Wellbore>(wellbore, errorCode);
        }

        /// <summary>
        /// Adds log object and test the return code
        /// </summary>
        /// <param name="log">the log.</param>
        /// <param name="errorCode">the errorCode.</param>
        public WMLS_AddToStoreResponse AddAndAssert(Log log, ErrorCodes errorCode = ErrorCodes.Success)
        {
            return AddAndAssert<LogList, Log>(log, errorCode);
        }

        /// <summary>
        /// Adds trajectory object and test the return code
        /// </summary>
        /// <param name="trajectory">the trajectory.</param>
        /// <param name="errorCode">the errorCode.</param>
        public WMLS_AddToStoreResponse AddAndAssert(Trajectory trajectory, ErrorCodes errorCode = ErrorCodes.Success)
        {
            return AddAndAssert<TrajectoryList, Trajectory>(trajectory, errorCode);
        }

        /// <summary>
        /// Adds rig object and test the return code
        /// </summary>
        /// <param name="rig">the rig</param>
        /// <param name="errorCode">the errorCode</param>
        public WMLS_AddToStoreResponse AddAndAssert(Rig rig, ErrorCodes errorCode = ErrorCodes.Success)
        {
            return AddAndAssert<RigList, Rig>(rig, errorCode);
        }

        /// <summary>
        /// Does get query for single well object and test for result count equal to 1 and is not null
        /// </summary>
        /// <param name="well">the well</param>
        /// <param name="isNotNull">if set to <c>true</c> the result should not be null.</param>
        /// <param name="optionsIn">The options in.</param>
        /// <param name="queryByExample">if set to <c>true</c> query by example.</param>
        /// <returns>The first well from the response</returns>
        public Well GetAndAssert(Well well, bool isNotNull = true, string optionsIn = null, bool queryByExample = false)
        {
            return GetAndAssert<WellList, Well>(well, isNotNull, optionsIn, queryByExample);
        }

        /// <summary>
        /// Does get query for single wellbore object and test for result count equal to 1 and is not null
        /// </summary>
        /// <param name="wellbore">the wellbore</param>
        /// <param name="isNotNull">if set to <c>true</c> the result should not be null.</param>
        /// <param name="optionsIn">The options in.</param>
        /// <param name="queryByExample">if set to <c>true</c> query by example.</param>
        /// <returns>The first wellbore from the response</returns>
        public Wellbore GetAndAssert(Wellbore wellbore, bool isNotNull = true, string optionsIn = null, bool queryByExample = false)
        {
            return GetAndAssert<WellboreList, Wellbore>(wellbore, isNotNull, optionsIn, queryByExample);
        }

        /// <summary>
        /// Does get query for single log object and test for result count equal to 1 and is not null
        /// </summary>
        /// <param name="log">the log with UIDs for well and wellbore</param>
        /// <param name="isNotNull">if set to <c>true</c> the result should not be null.</param>
        /// <param name="optionsIn">The options in.</param>
        /// <param name="queryByExample">if set to <c>true</c> query by example.</param>
        /// <returns>The first log from the response</returns>
        public Log GetAndAssert(Log log, bool isNotNull = true, string optionsIn = null, bool queryByExample = false)
        {
            return GetAndAssert<LogList, Log>(log, isNotNull, optionsIn, queryByExample);
        }

        /// <summary>
        /// Does get query for single trajectory object and test for result count equal to 1 and is not null
        /// </summary>
        /// <param name="trajectory">the log with UIDs for well and wellbore</param>
        /// <param name="isNotNull">if set to <c>true</c> the result should not be null.</param>
        /// <param name="optionsIn">The options in.</param>
        /// <param name="queryByExample">if set to <c>true</c> query by example.</param>
        /// <returns>The first trajectory from the response</returns>
        public Trajectory GetAndAssert(Trajectory trajectory, bool isNotNull = true, string optionsIn = null, bool queryByExample = false)
        {
            return GetAndAssert<TrajectoryList, Trajectory>(trajectory, isNotNull, optionsIn, queryByExample);
        }

        /// <summary>
        /// Does get query for single trajectory object and test for result count equal to 1 and is not null
        /// </summary>
        /// <param name="basicXmlTemplate">A XML string with reference parameters for UIDs and body elements</param>
        /// <param name="trajectory">The trajectory.</param>
        /// <param name="queryContent">The query xml descendants of the trajectory element.</param>
        /// <param name="isNotNull">if set to <c>true</c> the result should not be null.</param>
        /// <param name="optionsIn">The options in.</param>
        /// <returns>The first trajectory from the response.</returns>
        public Trajectory GetAndAssertWithXml(string basicXmlTemplate, Trajectory trajectory, string queryContent = null, bool isNotNull = true, string optionsIn = null)
        {
            var queryIn = string.Format(basicXmlTemplate,trajectory.UidWell, trajectory.UidWellbore, trajectory.Uid, queryContent);

            var results = Query<TrajectoryList, Trajectory>(ObjectTypes.Trajectory, queryIn, null, optionsIn ?? OptionsIn.ReturnElements.All);
            Assert.AreEqual(isNotNull ? 1 : 0, results.Count);

            var result = results.FirstOrDefault();
            Assert.AreEqual(isNotNull, result != null);
            return result;
        }

        /// <summary>
        /// Does UpdateInStore on well object and test the return code
        /// </summary>
        /// <param name="well">the well</param>
        /// <param name="errorCode">The error code.</param>
        public void UpdateAndAssert(Well well, ErrorCodes errorCode = ErrorCodes.Success)
        {
            UpdateAndAssert<WellList, Well>(well, errorCode);
        }

        /// <summary>
        /// Does UpdateInStore on wellbore object and test the return code
        /// </summary>
        /// <param name="wellbore">the wellbore</param>
        /// <param name="errorCode">The error code.</param>
        public void UpdateAndAssert(Wellbore wellbore, ErrorCodes errorCode = ErrorCodes.Success)
        {
            UpdateAndAssert<WellboreList, Wellbore>(wellbore, errorCode);
        }

        /// <summary>
        /// Does UpdateInStore on log object and test the return code
        /// </summary>
        /// <param name="log">the log</param>
        /// <param name="errorCode">The error code.</param>
        public void UpdateAndAssert(Log log, ErrorCodes errorCode = ErrorCodes.Success)
        {
            UpdateAndAssert<LogList, Log>(log, errorCode);
        }

        /// <summary>
        /// Does UpdateInStore on trajectory object and test the return code
        /// </summary>
        /// <param name="trajectory">the trajectory</param>
        /// <param name="errorCode">The error code.</param>
        public void UpdateAndAssert(Trajectory trajectory, ErrorCodes errorCode = ErrorCodes.Success)
        {
            UpdateAndAssert<TrajectoryList, Trajectory>(trajectory, errorCode);
        }

        /// <summary>
        /// Deletes the well and test the return code
        /// </summary>
        /// <param name="well">The well.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="partialDelete">if set to <c>true</c> is partial delete.</param>
        public void DeleteAndAssert(Well well, ErrorCodes errorCode = ErrorCodes.Success, bool partialDelete = false)
        {
            DeleteAndAssert<WellList, Well>(well, errorCode, partialDelete);
        }

        /// <summary>
        /// Deletes the wellbore and test the return code
        /// </summary>
        /// <param name="wellbore">The wellbore.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="partialDelete">if set to <c>true</c> is partial delete.</param>
        public void DeleteAndAssert(Wellbore wellbore, ErrorCodes errorCode = ErrorCodes.Success, bool partialDelete = false)
        {
            DeleteAndAssert<WellboreList, Wellbore>(wellbore, errorCode, partialDelete);
        }

        /// <summary>
        /// Deletes the log and test the return code
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="partialDelete">if set to <c>true</c> is partial delete.</param>
        public void DeleteAndAssert(Log log, ErrorCodes errorCode = ErrorCodes.Success, bool partialDelete = false)
        {
            DeleteAndAssert<LogList, Log>(log, errorCode, partialDelete);
        }

        /// <summary>
        /// Generations trajectory station data.
        /// </summary>
        /// <param name="numOfStations">The number of stations.</param>
        /// <param name="startMd">The start md.</param>
        /// <param name="mdUom">The MD index uom.</param>
        /// <param name="tvdUom">The Tvd uom.</param>
        /// <param name="angleUom">The angle uom.</param>
        /// <param name="inCludeExtra">True if to generate extra information for trajectory station.</param>
        /// <returns>The trajectoryStation collection.</returns>
        public List<TrajectoryStation> TrajectoryStations(int numOfStations, double startMd, MeasuredDepthUom mdUom = MdUom, WellVerticalCoordinateUom tvdUom = TvdUom, PlaneAngleUom angleUom = AngleUom, bool inCludeExtra = false)
        {
            return TrajectoryGenerator.GenerationStations(numOfStations, startMd, mdUom, tvdUom, angleUom, inCludeExtra);
        }

        public Well CreateFullWell()
        {
            string wellXml = "<wells xmlns=\"http://www.witsml.org/schemas/131\" version=\"1.3.1.1\">" + Environment.NewLine +
            "<well>" + Environment.NewLine +
            "<name>" + Name("Test Full Well") + " </name>" + Environment.NewLine +
            "<nameLegal>Company Legal Name</nameLegal>" + Environment.NewLine +
            "<numLicense>Company License Number</numLicense>" + Environment.NewLine +
            "<numGovt>Govt-Number</numGovt>" + Environment.NewLine +
            "<dTimLicense>2001-05-15T13:20:00Z</dTimLicense>" + Environment.NewLine +
            "<field>Big Field</field>" + Environment.NewLine +
            "<country>US</country>" + Environment.NewLine +
            "<state>TX</state>" + Environment.NewLine +
            "<county>Montgomery</county>" + Environment.NewLine +
            "<region>Region Name</region>" + Environment.NewLine +
            "<district>District Name</district>" + Environment.NewLine +
            "<block>Block Name</block>" + Environment.NewLine +
            "<timeZone>-06:00</timeZone>" + Environment.NewLine +
            "<operator>Operating Company</operator>" + Environment.NewLine +
            "<operatorDiv>Division Name</operatorDiv>" + Environment.NewLine +
            "<pcInterest uom=\"%\">65</pcInterest>" + Environment.NewLine +
            "<numAPI>123-543-987AZ</numAPI>" + Environment.NewLine +
            "<statusWell>drilling</statusWell>" + Environment.NewLine +
            "<purposeWell>exploration</purposeWell>" + Environment.NewLine +
            "<fluidWell>water</fluidWell>" + Environment.NewLine +
            "<dTimSpud>2001-05-31T08:15:00Z</dTimSpud>" + Environment.NewLine +
            "<dTimPa>2001-07-15T15:30:00Z</dTimPa>" + Environment.NewLine +
            "<wellheadElevation uom=\"ft\">500</wellheadElevation>" + Environment.NewLine +
            "<wellDatum uid=\"KB\">" + Environment.NewLine +
            "<name>Kelly Bushing</name>" + Environment.NewLine +
            "<code>KB</code>" + Environment.NewLine +
            "<elevation uom=\"ft\" datum=\"SL\">78.5</elevation>" + Environment.NewLine +
            "</wellDatum>" + Environment.NewLine +
            "<wellDatum uid=\"SL\">" + Environment.NewLine +
            "<name>Sea Level</name>" + Environment.NewLine +
            "<code>SL</code>" + Environment.NewLine +
            "<datumName namingSystem=\"EPSG\" code=\"5106\">Caspian Sea</datumName>" + Environment.NewLine +
            "</wellDatum>" + Environment.NewLine +
            "<groundElevation uom=\"ft\">250</groundElevation>" + Environment.NewLine +
            "<waterDepth uom=\"ft\">520</waterDepth>" + Environment.NewLine +
            "<wellLocation uid=\"loc-1\">" + Environment.NewLine +
            "<wellCRS uidRef=\"proj1\">ED50 / UTM Zone 31N</wellCRS>" + Environment.NewLine +
            "<easting uom=\"m\">425353.84</easting>" + Environment.NewLine +
            "<northing uom=\"m\">6623785.69</northing>" + Environment.NewLine +
            "<description>Location of well surface point in projected system.</description>" + Environment.NewLine +
            "</wellLocation>" + Environment.NewLine +
            "<referencePoint uid=\"SRP1\">" + Environment.NewLine +
            "<name>Slot Bay Centre</name>" + Environment.NewLine +
            "<type>Site Reference Point</type>" + Environment.NewLine +
            "<location uid=\"loc-1\">" + Environment.NewLine +
            "<wellCRS uidRef=\"proj1\">ED50 / UTM Zone 31N</wellCRS>" + Environment.NewLine +
            "<easting uom=\"m\">425366.47</easting>" + Environment.NewLine +
            "<northing uom=\"m\">6623781.95</northing>" + Environment.NewLine +
            "</location>" + Environment.NewLine +
            "<location uid=\"loc-2\">" + Environment.NewLine +
            "<wellCRS uidRef=\"localWell1\">WellOneWSP</wellCRS>" + Environment.NewLine +
            "<localX uom=\"m\">12.63</localX>" + Environment.NewLine +
            "<localY uom=\"m\">-3.74</localY>" + Environment.NewLine +
            "<description>Location of the Site Reference Point with respect to the well surface point</description>" + Environment.NewLine +
            "</location>" + Environment.NewLine +
            "</referencePoint>" + Environment.NewLine +
            "<referencePoint uid=\"WRP2\">" + Environment.NewLine +
            "<name>Sea Bed</name>" + Environment.NewLine +
            "<type>Well Reference Point</type>" + Environment.NewLine +
            "<elevation uom=\"ft\" datum=\"SL\">-118.4</elevation>" + Environment.NewLine +
            "<measuredDepth uom=\"ft\" datum=\"KB\">173.09</measuredDepth>" + Environment.NewLine +
            "<location uid=\"loc-1\">" + Environment.NewLine +
            "<wellCRS uidRef=\"proj1\">ED50 / UTM Zone 31N</wellCRS>" + Environment.NewLine +
            "<easting uom=\"m\">425353.84</easting>" + Environment.NewLine +
            "<northing uom=\"m\">6623785.69</northing>" + Environment.NewLine +
            "</location>" + Environment.NewLine +
            "<location uid=\"loc-2\">" + Environment.NewLine +
            "<wellCRS uidRef=\"geog1\">ED50</wellCRS>" + Environment.NewLine +
            "<latitude uom=\"dega\">59.743844</latitude>" + Environment.NewLine +
            "<longitude uom=\"dega\">1.67198083</longitude>" + Environment.NewLine +
            "</location>" + Environment.NewLine +
            "</referencePoint>" + Environment.NewLine +
            "<wellCRS uid=\"geog1\">" + Environment.NewLine +
            "<name>ED50</name>" + Environment.NewLine +
            "<geodeticCRS uidRef=\"4230\">4230</geodeticCRS>" + Environment.NewLine +
            "<description>ED50 system with EPSG code 4230.</description>" + Environment.NewLine +
            "</wellCRS>" + Environment.NewLine +
            "<wellCRS uid=\"proj1\">" + Environment.NewLine +
            "<name>ED50 / UTM Zone 31N</name>" + Environment.NewLine +
            "<mapProjectionCRS uidRef=\"23031\">ED50 / UTM Zone 31N</mapProjectionCRS>" + Environment.NewLine +
            "</wellCRS>" + Environment.NewLine +
            "<wellCRS uid=\"localWell1\">" + Environment.NewLine +
            "<name>WellOneWSP</name>" + Environment.NewLine +
            "<localCRS>" + Environment.NewLine +
            "<usesWellAsOrigin>true</usesWellAsOrigin>" + Environment.NewLine +
            "<yAxisAzimuth uom=\"dega\" northDirection=\"grid north\">0</yAxisAzimuth>" + Environment.NewLine +
            "<xRotationCounterClockwise>false</xRotationCounterClockwise>" + Environment.NewLine +
            "</localCRS>" + Environment.NewLine +
            "</wellCRS>" + Environment.NewLine +
            "<commonData>" + Environment.NewLine +
            "<dTimCreation>2016-03-07T22:53:59.249Z</dTimCreation>" + Environment.NewLine +
            "<dTimLastChange>2016-03-07T22:53:59.249Z</dTimLastChange > " + Environment.NewLine +
            "<itemState>plan</itemState>" + Environment.NewLine +
            "<comments>These are the comments associated with the Well data object.</comments>" + Environment.NewLine +
            "<defaultDatum uidRef=\"KB\">Kelly Bushing</defaultDatum>" + Environment.NewLine +
            "</commonData>" + Environment.NewLine +
            "</well>" + Environment.NewLine +
            "</wells>";

            WellList wells = EnergisticsConverter.XmlToObject<WellList>(wellXml);
            return wells.Items[0] as Well;
        }
    }
}
