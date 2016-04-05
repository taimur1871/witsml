﻿//----------------------------------------------------------------------- 
// PDS.Witsml.Studio, 2016.1
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

using Caliburn.Micro;
using Energistics.Datatypes;
using Energistics.Protocol.Core;
using PDS.Witsml.Studio.Connections;
using PDS.Witsml.Studio.Plugins.EtpBrowser.Models;
using PDS.Witsml.Studio.Runtime;
using PDS.Witsml.Studio.ViewModels;

namespace PDS.Witsml.Studio.Plugins.EtpBrowser.ViewModels
{
    /// <summary>
    /// Manages the behavior of the settings view.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.Screen" />
    public class SettingsViewModel : Screen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        /// <param name="runtime">The runtime service.</param>
        public SettingsViewModel(IRuntimeService runtime)
        {
            Runtime = runtime;
            DisplayName =  string.Format("{0:D} - {0}", Protocols.Core);
        }

        /// <summary>
        /// Gets or Sets the Parent <see cref="T:Caliburn.Micro.IConductor" />
        /// </summary>
        public new MainViewModel Parent
        {
            get { return (MainViewModel)base.Parent; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public EtpSettings Model
        {
            get { return Parent.Model; }
        }

        /// <summary>
        /// Gets the runtime service.
        /// </summary>
        /// <value>The runtime.</value>
        public IRuntimeService Runtime { get; private set; }

        /// <summary>
        /// Shows the connection dialog.
        /// </summary>
        public void ShowConnectionDialog()
        {
            var viewModel = new ConnectionViewModel(Runtime, ConnectionTypes.Etp)
            {
                DataItem = Model.Connection
            };

            if (Runtime.ShowDialog(viewModel))
            {
                Model.Connection = viewModel.DataItem;
                RequestSession();
            }
        }

        /// <summary>
        /// Requests a new ETP session.
        /// </summary>
        public void RequestSession()
        {
            Parent.OnConnectionChanged();
        }

        /// <summary>
        /// Closes the current ETP session.
        /// </summary>
        public void CloseSession()
        {
            Parent.Client.Handler<ICoreClient>()
                .CloseSession();
        }
    }
}
