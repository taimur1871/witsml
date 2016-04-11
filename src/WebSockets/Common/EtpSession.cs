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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Avro.IO;
using Avro.Specific;
using Energistics.Datatypes;

namespace Energistics.Common
{
    public abstract class EtpSession : EtpBase, IEtpSession
    {
        private long MessageId = 0;

        public EtpSession(string application, string version, IDictionary<string, string> headers)
        {
            Headers = headers ?? new Dictionary<string, string>();
            Handlers = new Dictionary<object, IProtocolHandler>();
            ApplicationName = application;
            ApplicationVersion = version;
            ValidateHeaders();
        }

        public string ApplicationName { get; private set; }

        public string ApplicationVersion { get; private set; }

        public string SessionId { get; set; }

        protected IDictionary<string, string> Headers { get; private set; } 

        protected IDictionary<object, IProtocolHandler> Handlers { get; private set; }

        public T Handler<T>() where T : IProtocolHandler
        {
            IProtocolHandler handler;

            if (Handlers.TryGetValue(typeof(T), out handler) && handler is T)
            {
                return (T)handler;
            }

            Logger.Error(Format("[{0}] Protocol handler not registered for {1}.", SessionId, typeof(T).FullName));
            throw new NotSupportedException(string.Format("Protocol handler not registered for {0}.", typeof(T).FullName));
        }

        public bool CanHandle<T>() where T : IProtocolHandler
        {
            return Handlers.ContainsKey(typeof(T));
        }

        public override void OnSessionOpened(IList<SupportedProtocol> supportedProtocols)
        {
            HandleUnsupportedProtocols(supportedProtocols);

            // notify protocol handlers about new session
            foreach (var item in Handlers)
            {
                if (item.Key is Type)
                    item.Value.OnSessionOpened(supportedProtocols);
            }
        }

        public virtual void OnDataReceived(byte[] data)
        {
            Decode(data);
        }

        public void SendMessage<T>(MessageHeader header, T body) where T : ISpecificRecord
        {
            byte[] data = new byte[0];

            try
            {
                data = body.Encode(header);
            }
            catch (Exception ex)
            {
                Handler(header.Protocol)
                    .ProtocolException(1000, ex.Message, header.MessageId);

                return;
            }

            Send(data, 0, data.Length);
            Sent(header, body);
        }

        public IList<SupportedProtocol> GetSupportedProtocols(bool isSender = false)
        {
            var supportedProtocols = new List<SupportedProtocol>();
            var version = new Datatypes.Version()
            {
                Major = 1
            };

            foreach (var handler in Handlers.Values)
            {
                var role = isSender ? handler.RequestedRole : handler.Role;

                if (supportedProtocols.Contains(handler.Protocol, role))
                    continue;

                supportedProtocols.Add(new SupportedProtocol()
                {
                    Protocol = handler.Protocol,
                    ProtocolVersion = version,
                    ProtocolCapabilities = handler.GetCapabilities(),
                    Role = role
                });
            }

            return supportedProtocols;
        }

        public long NewMessageId()
        {
            return Interlocked.Increment(ref MessageId);
        }

        public abstract void Close(string reason = null);

        protected abstract void Send(byte[] data, int offset, int length);

        protected void Decode(byte[] data)
        {
            using (var inputStream = new MemoryStream(data))
            {
                // create avro binary decoder to read from memory stream
                var decoder = new BinaryDecoder(inputStream);
                // deserialize the header
                var header = decoder.Decode<MessageHeader>();

                // log message metadata
                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("[{0}] Message received: {1}", SessionId, this.Serialize(header));
                }

                // call processing action
                HandleMessage(header, decoder);
            }
        }

        protected void HandleMessage(MessageHeader header, Decoder decoder)
        {
            if (Handlers.ContainsKey(header.Protocol))
            {
                Handler(header.Protocol)
                    .HandleMessage(header, decoder);
            }
            else
            {
                var message = string.Format("Protocol handler not registered for protocol {0}.", header.Protocol);

                Handler((int)Protocols.Core)
                    .ProtocolException((int)ErrorCodes.EUNSUPPORTED_PROTOCOL, message, header.MessageId);
            }
        }

        protected override void Register(Type contractType, Type handlerType)
        {
            base.Register(contractType, handlerType);

            var handler = CreateInstance(contractType);

            if (handler != null)
            {
                handler.Session = this;
                Handlers[contractType] = handler;
                Handlers[handler.Protocol] = handler;
            }
        }

        protected IProtocolHandler Handler(int protocol)
        {
            if (Handlers.ContainsKey(protocol))
            {
                return Handlers[protocol];
            }

            Logger.Error(Format("[{0}] Protocol handler not registered for protocol {1}.", SessionId, protocol));
            throw new NotSupportedException(string.Format("Protocol handler not registered for protocol {0}.", protocol));
        }

        protected virtual void HandleUnsupportedProtocols(IList<SupportedProtocol> supportedProtocols)
        {
            // remove unsupported handler mappings (excluding Core protocol)
            Handlers
                .Where(x => x.Value.Protocol > 0 && !supportedProtocols.Contains(x.Value.Protocol, x.Value.Role))
                .ToList()
                .ForEach(x =>
                {
                    x.Value.Session = null;
                    Handlers.Remove(x.Key);
                    Handlers.Remove(x.Value.Protocol);
                });

            // update remaining handler mappings by protocol
            foreach (var handler in Handlers.Values.ToArray())
            {
                if (!Handlers.ContainsKey(handler.Protocol))
                    Handlers[handler.Protocol] = handler;
            }
        }

        protected void Sent<T>(MessageHeader header, T body)
        {
            if (Output != null)
            {
                Format("[{0}] Message sent at {1}", SessionId, DateTime.Now);
                Format(this.Serialize(header));
                Format(this.Serialize(body, true));
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("[{0}] Message sent: {1}", SessionId, this.Serialize(header));
            }
        }

        protected virtual void ValidateHeaders()
        {
        }
    }
}
