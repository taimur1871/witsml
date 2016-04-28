﻿//----------------------------------------------------------------------- 
// PDS.Witsml, 2016.1
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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Witsml131 = Energistics.DataAccess.WITSML131;
using Witsml141 = Energistics.DataAccess.WITSML141;
using Witsml200 = Energistics.DataAccess.WITSML200;

namespace PDS.Witsml.Data.Channels
{
    [TestClass]
    public class ChannelDataReaderTests
    {
        private const string TimeLogData = @"[
            [[""2016-03-01T00:00:00.0-06:00""], [0.0, 0.0, 0.0]],
            [[""2016-03-01T00:00:01.0-06:00""], [1.0, 1.1, 1.2]],
            [[""2016-03-01T00:00:02.0-06:00""], [2.0, 2.1, 2.2]],
            [[""2016-03-01T00:00:03.0-06:00""], [3.0, 3.1, 3.2]],
            [[""2016-03-01T00:00:04.0-06:00""], [4.0, 4.1, 4.2]],
        ]";

        private const string DepthLogData1 = @"[
            [[0.0], [0.0, 0.1, 0.2]],
            [[0.1], [1.0, 1.1, 1.2]],
            [[0.2], [2.0, 2.1, 2.2]],
            [[0.3], [3.0, 3.1, 3.2]],
            [[0.4], [4.0, 4.1, 4.2]],
        ]";

        private const string DepthLogData2 = @"[
            [[0.5], [5.0, 5.1, 5.2]],
            [[0.6], [6.0, 6.1, 6.2]],
            [[0.7], [7.0, 7.1, 7.2]],
            [[0.8], [8.0, 8.1, 8.2]],
            [[0.9], [9.0, 9.1, 9.2]],
        ]";

        private const string ChannelSetData = @"[
            [[0.0, ""2016-03-01T00:00:00.0-06:00""], [[0.0, true], 0.1, 0.2]],
            [[0.1, ""2016-03-01T00:00:01.0-06:00""], [[1.0, true], 1.1, 1.2]],
            [[0.2, ""2016-03-01T00:00:02.0-06:00""], [[2.0, false], 2.1, 2.2]],
            [[0.3, ""2016-03-01T00:00:03.0-06:00""], [null,        null, 3.2]],
            [[0.4, ""2016-03-01T00:00:04.0-06:00""], [[4.0, true], 4.1, 4.2]],
        ]";

        private const string UpdateLogData1 = @"[
            [[0.0], [0.0, 0.1, 0.2]],
            [[1.0], [1.0, 1.1, 1.2]],
            [[2.0], [2.0, 2.1, 2.2]],
            [[3.0], [3.0, 3.1, 3.2]],
            [[4.0], [4.0, 4.1, 4.2]],
            [[5.0], [5.0, 5.1, 5.2]],
            [[5.5], [5.05, 5.15, 5.25]],
            [[6.0], [6.0, 6.1, 6.2]],
            [[7.0], [7.0, 7.1, 7.2]]
        ]";

        private const string UpdateLogData2 = @"[
            [[3.0], [null, null, 3.22]],
            [[3.5], [3.005, 3.115, 3.225]],
            [[5.0], [null, null, 5.22]],
            [[6.0], [null, null, 6.22]],
        ]";

        private const string UpdateLogData3 = @"[
            [[3.0], [null, null, 3.22]],
            [[3.5], [3.005, 3.115, 3.225]],
            [[5.0], [null, null, 5.22]],
            [[6.0], [null, 6.11, 6.22]],
            [[7.0], [null, 7.11, 7.22]],
            [[7.5], [null, null, 7.225]],
        ]";

        [TestMethod]
        public void ChannelDataReader_can_parse_null_data()
        {
            var reader = new ChannelDataReader(string.Empty);

            Assert.AreEqual(0, reader.Depth);
            Assert.AreEqual(0, reader.FieldCount);
            Assert.AreEqual(0, reader.RecordsAffected);
        }

        [TestMethod]
        public void ChannelDataReader_can_set_data_value()
        {
            var reader = new ChannelDataReader(UpdateLogData1, new[] { "MD", "ROP", "GR", "HKLD" })
                .WithIndex("MD", "m", true, false);

            Assert.AreEqual(1, reader.Depth);
            Assert.AreEqual(4, reader.FieldCount);
            Assert.AreEqual(9, reader.RecordsAffected);

            if (reader.Read())
            {
                reader.SetValue(1, 1000.0);
                Assert.AreEqual(1000.0, reader.GetDouble(1));
            }
        }

        [TestMethod]
        public void ChannelDataReader_can_read_time_log_data()
        {
            var reader = new ChannelDataReader(TimeLogData);
            int count = 0;

            Assert.AreEqual(1, reader.Depth);
            Assert.AreEqual(4, reader.FieldCount);
            Assert.AreEqual(5, reader.RecordsAffected);

            while (reader.Read())
            {
                Console.WriteLine("Row {0}: {1}, {2}, {3}, {4}", count++,
                    reader.GetDateTimeOffset(0),
                    reader.GetDouble(1),
                    reader.GetDouble(2),
                    reader.GetDouble(3));
            }
        }

        [TestMethod]
        public void ChannelDataReader_can_read_depth_log_data()
        {
            var reader = new ChannelDataReader(DepthLogData1, new[] { "MD", "ROP", "GR", "HKLD" });
            int count = 0;

            Assert.AreEqual(1, reader.Depth);
            Assert.AreEqual(4, reader.FieldCount);
            Assert.AreEqual(5, reader.RecordsAffected);

            while (reader.Read())
            {
                Console.WriteLine("Row {0}: {1}, {2}, {3}, {4}", count++,
                    reader.GetDouble(0),
                    reader.GetDouble(1),
                    reader.GetDouble(2),
                    reader.GetDouble(reader.GetOrdinal("HKLD")));
            }
        }

        [TestMethod]
        public void ChannelDataReader_can_calculate_channel_min_max_indices_with_single_value()
        {
            var reader = new ChannelDataReader(UpdateLogData2, new[] { "MD", "ROP", "GR", "HKLD" })
                .WithIndex("MD", "m", true, false);

            Assert.AreEqual(1, reader.Depth);
            Assert.AreEqual(4, reader.FieldCount);
            Assert.AreEqual(4, reader.RecordsAffected);

            var range = reader.GetChannelIndexRange(reader.GetOrdinal("GR"));

            Assert.AreEqual(3.5, range.Start);
            Assert.AreEqual(3.5, range.End);
        }

        [TestMethod]
        public void ChannelDataReader_can_calculate_channel_min_max_indices_with_multiple_values()
        {
            var reader = new ChannelDataReader(UpdateLogData3, new[] { "MD", "ROP", "GR", "HKLD" })
                .WithIndex("MD", "m", true, false);

            Assert.AreEqual(1, reader.Depth);
            Assert.AreEqual(4, reader.FieldCount);
            Assert.AreEqual(6, reader.RecordsAffected);

            var range = reader.GetChannelIndexRange(reader.GetOrdinal("GR"));

            Assert.AreEqual(3.5, range.Start);
            Assert.AreEqual(7.0, range.End);
        }

        [TestMethod]
        public void ChannelDataReader_can_read_ChannelSet_data()
        {
            var reader = new ChannelDataReader(ChannelSetData);
            var json = new StringBuilder("[");
            int count = 0;

            Assert.AreEqual(2, reader.Depth);
            Assert.AreEqual(5, reader.FieldCount);
            Assert.AreEqual(5, reader.RecordsAffected);
            json.AppendLine();

            while (reader.Read())
            {
                Console.WriteLine("Row {0}: {1}, {2}, {3}, {4}, {5}", count++,
                    reader.GetDouble(0),
                    reader.GetDateTimeOffset(1),
                    reader.GetString(2),
                    reader.GetDouble(3),
                    reader.GetDouble(4));

                json.AppendLine(reader.GetJson());
            }

            Assert.IsNull(reader.GetJson());

            // original
            Console.WriteLine();
            Console.WriteLine(ChannelSetData);

            // serialized
            Console.WriteLine();
            Console.WriteLine(json.Append("]"));
        }

        [TestMethod]
        [Ignore, Description("Fix: Indices not getting set in reader")]
        public void ChannelDataReader_can_slice()
        {
            var data = 
                "[" +
                "[[1],[1,1,1,1,1]]," +
                "[[2],[2,2,2,2,2]]," +
                "[[3],[3,3,3,3,3]]," +
                "[[4],[4,4,4,4,4]]," +
                "[[5],[5,5,5,5,5]]," +
                "[[6],[6,6,6,6,6]]," +
                "[[7],[7,7,7,7,7]]" +
                "]";

            // Create a Reader
            var reader = new ChannelDataReader(data, "MD,CH1,CH2,CH3,CH4,CH5".Split(','), "ft,ft1,ft2,ft3,ft4,ft5".Split(','), "eml://witsml1411/well(Energistics-well-0001)/wellbore(Energistics-w1-wellbore-0001)/log(Energistics-w1-wb1-log-0002)", "06e4dff8-3de4-4057-a21b-92026e89a6d4");

            // Slice the Reader
            //var slices = new string[] { "MD", "CH2", "CH5" };

            Dictionary<int, string> requestedMnemonics = new Dictionary<int, string>() { { 0, "MD" }, { 2, "CH2" }, { 5, "CH5" } };
            Dictionary<int, string> requestedUnits = new Dictionary<int, string>() { { 0, "ft" }, { 2, "ft2" }, { 5, "ft5" } };

            reader.Slice(requestedMnemonics, requestedUnits);

            // Test Mnemonic Slices
            var mnemonics = reader.Mnemonics;
            var requestedMnemonicValues = requestedMnemonics.Values.ToArray();
            for (var i = 0; i < mnemonics.Length; i++)
            {
                Assert.AreEqual(mnemonics[i], requestedMnemonicValues[i]);
            }

            // Test Unit Slices
            var units = reader.Units;
            Assert.AreEqual(requestedMnemonics.Keys.Count, units.Length);
            Assert.AreEqual(units[0], "ft");
            Assert.AreEqual(units[1], "ft2");
            Assert.AreEqual(units[2], "ft5");

            var values = new object[6];
            var valueCount = reader.GetValues(values);

            Assert.AreEqual(requestedMnemonics.Keys.Count, valueCount);
        }



        //[TestMethod]
        //public void ChannelDataReader_can_read_Log_131()
        //{
        //    var devKit = new DevKit131Aspect();
        //    var log = new Witsml131.Log();
        //    var rows = 10;
        //    var cols = 3;

        //    devKit.InitHeader(log, Witsml131.ReferenceData.LogIndexType.measureddepth);
        //    devKit.InitDataMany(log, devKit.Mnemonics(log), devKit.Units(log), rows);

        //    var reader = log.GetReader();
        //    int count = 0;

        //    Assert.AreEqual(1, reader.Depth);
        //    Assert.AreEqual(cols, reader.FieldCount);
        //    Assert.AreEqual(rows, reader.RecordsAffected);

        //    while (reader.Read())
        //    {
        //        Console.WriteLine("Row {0}: {1}, {2}, {3}", count++,
        //            reader.GetDouble(0),
        //            reader.GetDouble(1),
        //            reader.GetDouble(2));
        //    }
        //}

        //[TestMethod]
        //public void ChannelDataReader_can_read_Log_141()
        //{
        //    var devKit = new DevKit141Aspect();
        //    var log = new Witsml141.Log();
        //    var rows = 10;
        //    var cols = 3;

        //    devKit.InitHeader(log, Witsml141.ReferenceData.LogIndexType.measureddepth);
        //    devKit.InitDataMany(log, devKit.Mnemonics(log), devKit.Units(log), rows, 0.5);

        //    var reader = log.GetReaders().Single();
        //    int count = 0;

        //    Assert.AreEqual(1, reader.Depth);
        //    Assert.AreEqual(cols, reader.FieldCount);
        //    Assert.AreEqual(rows, reader.RecordsAffected);

        //    while (reader.Read())
        //    {
        //        Console.WriteLine("Row {0}: {1}, {2}, {3}", count++,
        //            reader.GetDouble(0),
        //            reader.GetDouble(1),
        //            reader.GetDouble(2));
        //    }
        //}

        //[TestMethod]
        //public void ChannelDataReader_can_read_Log_200()
        //{
        //    var devKit = new DevKit200Aspect();
        //    var log = new Witsml200.Log();
        //    var rows = 4;
        //    var cols = 4;

        //    var channelIndex = new Witsml200.ComponentSchemas.ChannelIndex
        //    {
        //        IndexType = Witsml200.ReferenceData.ChannelIndexType.datetime,
        //        Direction = Witsml200.ReferenceData.IndexDirection.increasing,
        //        Mnemonic = "MD",
        //        Uom = "m"
        //    };

        //    devKit.InitHeader(log, Witsml200.ReferenceData.LoggingMethod.Mixed, channelIndex);

        //    var reader = log.GetReaders().Single();
        //    int count = 0;

        //    Assert.AreEqual(1, reader.Depth);
        //    Assert.AreEqual(cols, reader.FieldCount);
        //    Assert.AreEqual(rows, reader.RecordsAffected);
        //    Console.WriteLine(log.ChannelSet[0].Data.Data);

        //    while (reader.Read())
        //    {
        //        Console.WriteLine("Row {0}: {1}, {2}, {3}, {4}", count++,
        //            reader.GetDateTimeOffset(0),
        //            reader.GetString(1),
        //            reader.GetDouble(2),
        //            reader.GetDouble(3));
        //    }
        //}
    }
}
