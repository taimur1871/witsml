﻿//----------------------------------------------------------------------- 
// ETP DevKit, 1.0
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

using System.Collections.Generic;
using Avro.IO;
using Energistics.Datatypes;
using Energistics.Protocol;
using Energistics.Protocol.Core;

namespace Energistics.Common
{
    public interface IProtocolHandler
    {
        IEtpSession Session { get; set; }

        int Protocol { get; }

        string Role { get; }

        string RequestedRole { get; }

        IDictionary<string, DataValue> GetCapabilities();

        void OnSessionOpened(IList<SupportedProtocol> supportedProtocols);

        void Acknowledge(long correlationId, MessageFlags messageFlag = MessageFlags.None);

        void ProtocolException(int errorCode, string errorMessage, long correlationId = 0);

        void HandleMessage(MessageHeader header, Decoder decoder);

        event ProtocolEventHandler<Acknowledge> OnAcknowledge;

        event ProtocolEventHandler<ProtocolException> OnProtocolException;
    }
}
