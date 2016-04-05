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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using Witsml131 = Energistics.DataAccess.WITSML131;
using Energistics.DataAccess.WITSML131.ComponentSchemas;
using log4net;
using PDS.Witsml.Server.Properties;

namespace PDS.Witsml.Server.Configuration
{
    /// <summary>
    /// Provides common WITSML server capabilities for data schema version 1.3.1.1.
    /// </summary>
    /// <seealso cref="PDS.Witsml.Server.Configuration.CapServerProvider{Energistics.DataAccess.WITSML131.CapServers}" />
    [Export(typeof(ICapServerProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CapServer131Provider : CapServerProvider<Witsml131.CapServers>
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CapServer131Provider));

        private const string Namespace131 = "http://www.witsml.org/schemas/131";

        /// <summary>
        /// Gets the data schema version.
        /// </summary>
        /// <value>The data schema version.</value>
        public override string DataSchemaVersion
        {
            get { return OptionsIn.DataVersion.Version131.Value; }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="IWitsml131Configuration"/> providers.
        /// </summary>
        /// <value>The collection of providers.</value>
        [ImportMany]
        public IEnumerable<IWitsml131Configuration> Providers { get; set; }

        /// <summary>
        /// Validates the namespace for a specific WITSML data schema version.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <exception cref="WitsmlException"></exception>
        protected override void ValidateNamespace(XDocument document)
        {
            if (!Namespace131.Equals(GetNamespace(document)))
            {
                throw new WitsmlException(ErrorCodes.MissingDefaultWitsmlNamespace);
            }
        }

        /// <summary>
        /// Creates the capServers instance for a specific data schema version.
        /// </summary>
        /// <returns>The capServers instance.</returns>
        protected override Witsml131.CapServers CreateCapServer()
        {
            if (!Providers.Any())
            {
                _log.WarnFormat("No WITSML configuration providers loaded for data schema version {0}", DataSchemaVersion);
                return null;
            }

            var capServer = new Witsml131.CapServer();

            foreach (var config in Providers)
            {
                config.GetCapabilities(capServer);
            }

            capServer.ApiVers = "1.3.1";
            capServer.SchemaVersion = DataSchemaVersion;

            capServer.Name = Settings.Default.DefaultServerName;
            capServer.Version = Settings.Default.DefaultServerVersion;
            capServer.Description = Settings.Default.DefaultServerDescription;
            capServer.Vendor = Settings.Default.DefaultVendorName;
            capServer.Contact = new Contact()
            {
                Name = Settings.Default.DefaultContactName,
                Email = Settings.Default.DefaultContactEmail,
                Phone = Settings.Default.DefaultContactPhone
            };

            return new Witsml131.CapServers()
            {
                CapServer = capServer,
                Version = capServer.ApiVers
            };
        }
    }
}
