﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Energistics.DataAccess;
using Energistics.DataAccess.WITSML131;
using PDS.Witsml.Server.Configuration;

namespace PDS.Witsml.Server.Data.Wellbores
{
    /// <summary>
    /// Data adapter that encapsulates CRUD functionality for <see cref="Wellbore" />
    /// </summary>
    /// <seealso cref="PDS.Witsml.Server.Data.MongoDbDataAdapter{Energistics.DataAccess.WITSML131.Wellbore}" />
    /// <seealso cref="PDS.Witsml.Server.Configuration.IWitsml131Configuration" />
    [Export(typeof(IWitsml131Configuration))]
    [Export(typeof(IWitsmlDataAdapter<Wellbore>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Wellbore131DataAdapter : MongoDbDataAdapter<Wellbore>, IWitsml131Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Wellbore131DataAdapter"/> class.
        /// </summary>
        /// <param name="databaseProvider">The database provider.</param>
        [ImportingConstructor]
        public Wellbore131DataAdapter(IDatabaseProvider databaseProvider) : base(databaseProvider, ObjectNames.Wellbore131)
        {
        }

        /// <summary>
        /// Gets the supported capabilities for the <see cref="Wellbore"/> object.
        /// </summary>
        /// <param name="capServer">The capServer instance.</param>
        public void GetCapabilities(CapServer capServer)
        {
            capServer.Add(Functions.GetFromStore, ObjectTypes.Wellbore);
            capServer.Add(Functions.AddToStore, ObjectTypes.Wellbore);
            //capServer.Add(Functions.UpdateInStore, ObjectTypes.Wellbore);
            //capServer.Add(Functions.DeleteFromStore, ObjectTypes.Wellbore);
        }

        /// <summary>
        /// Queries the object(s) specified by the parser.
        /// </summary>
        /// <param name="parser">The parser that specifies the query parameters.</param>
        /// <returns>Queried objects.</returns>
        public override WitsmlResult<IEnergisticsCollection> Query(WitsmlQueryParser parser)
        {
            var returnElements = parser.ReturnElements();
            Logger.DebugFormat("Querying with return elements '{0}'", returnElements);

            var fields = (OptionsIn.ReturnElements.IdOnly.Equals(returnElements))
                ? new List<string> { IdPropertyName, NamePropertyName, "UidWell", "NameWell" }
                : null;

            return new WitsmlResult<IEnergisticsCollection>(
                ErrorCodes.Success,
                new WellboreList()
                {
                    Wellbore = QueryEntities(parser, fields)
                });
        }

        /// <summary>
        /// Adds a <see cref="Wellbore"/> to the data store.
        /// </summary>
        /// <param name="entity">The object.</param>
        /// <returns>
        /// A WITSML result that includes a positive value indicates a success or a negative value indicates an error.
        /// </returns>
        public override WitsmlResult Add(Wellbore entity)
        {
            entity.Uid = NewUid(entity.Uid);
            entity.CommonData = entity.CommonData.Update();

            var validator = Container.Resolve<IDataObjectValidator<Wellbore>>();
            validator.Validate(Functions.AddToStore, entity);

            Logger.DebugFormat("Add new wellbore with uidWell: {0}; uid: {1}", entity.UidWell, entity.Uid);
            InsertEntity(entity);

            return new WitsmlResult(ErrorCodes.Success, entity.Uid);
        }

        /// <summary>
        /// Parses the specified XML string.
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <returns>An instance of <see cref="Wellbore" />.</returns>
        protected override Wellbore Parse(string xml)
        {
            var list = WitsmlParser.Parse<WellboreList>(xml);
            return list.Wellbore.FirstOrDefault();
        }
    }
}
