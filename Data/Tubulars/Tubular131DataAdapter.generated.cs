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
using System.ComponentModel.Composition;
using System.Linq;
using Energistics.DataAccess.WITSML131;
using Energistics.DataAccess.WITSML131.ComponentSchemas;
using Energistics.Datatypes;
using LinqToQuerystring;
using PDS.WITSMLstudio.Framework;
using PDS.WITSMLstudio.Store.Configuration;

namespace PDS.WITSMLstudio.Store.Data.Tubulars
{
    /// <summary>
    /// Data adapter that encapsulates CRUD functionality for <see cref="Tubular" />
    /// </summary>
    /// <seealso cref="PDS.WITSMLstudio.Store.Data.MongoDbDataAdapter{Tubular}" />
    [Export(typeof(IWitsml131Configuration))]
    [Export(typeof(IWitsmlDataAdapter<Tubular>))]
    [Export131(ObjectTypes.Tubular, typeof(IWitsmlDataAdapter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class Tubular131DataAdapter : MongoDbDataAdapter<Tubular>, IWitsml131Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tubular131DataAdapter" /> class.
        /// </summary>
        /// <param name="container">The composition container.</param>
        /// <param name="databaseProvider">The database provider.</param>
        [ImportingConstructor]
        public Tubular131DataAdapter(IContainer container, IDatabaseProvider databaseProvider)
            : base(container, databaseProvider, ObjectNames.Tubular131)
        {
            Logger.Debug("Instance created.");
        }

        /// <summary>
        /// Gets the supported capabilities for the <see cref="Tubular"/> object.
        /// </summary>
        /// <param name="capServer">The capServer instance.</param>
        public void GetCapabilities(CapServer capServer)
        {
            Logger.DebugFormat("Getting the supported capabilities for Tubular data version {0}.", capServer.Version);

            capServer.Add(Functions.GetFromStore, ObjectTypes.Tubular);
            capServer.Add(Functions.AddToStore, ObjectTypes.Tubular);
            capServer.Add(Functions.UpdateInStore, ObjectTypes.Tubular);
            capServer.Add(Functions.DeleteFromStore, ObjectTypes.Tubular);
      }

        /// <summary>
        /// Gets a collection of data objects related to the specified URI.
        /// </summary>
        /// <param name="parentUri">The parent URI.</param>
        /// <returns>A collection of data objects.</returns>
        public override List<Tubular> GetAll(EtpUri? parentUri)
        {
            Logger.DebugFormat("Fetching all Tubulars; Parent URI: {0}", parentUri);

            return GetAllQuery(parentUri)
                .OrderBy(x => x.Name)
                .ToList();
        }

        /// <summary>
        /// Gets an <see cref="IQueryable{Tubular}" /> instance to by used by the GetAll method.
        /// </summary>
        /// <param name="parentUri">The parent URI.</param>
        /// <returns>An executable query.</returns>
        protected override IQueryable<Tubular> GetAllQuery(EtpUri? parentUri)
        {
            var query = GetQuery().AsQueryable();

            if (parentUri != null)
            {
                var ids = parentUri.Value.GetObjectIds().ToDictionary(x => x.ObjectType, y => y.ObjectId, StringComparer.CurrentCultureIgnoreCase);
                var uidWellbore = ids[ObjectTypes.Wellbore];
                var uidWell = ids[ObjectTypes.Well];

                if (!string.IsNullOrWhiteSpace(uidWell))
                    query = query.Where(x => x.UidWell == uidWell);

                if (!string.IsNullOrWhiteSpace(uidWellbore))
                    query = query.Where(x => x.UidWellbore == uidWellbore);

                if (!string.IsNullOrWhiteSpace(parentUri.Value.Query))
                    query = query.LinqToQuerystring(parentUri.Value.Query);
            }

            return query;
        }
    }
}