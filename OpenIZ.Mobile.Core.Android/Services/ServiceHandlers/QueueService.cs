/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: khannan
 * Date: 2016-8-22
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Android.Services.Attributes;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Mobile.Core.Android.Services.Model;
using OpenIZ.Core.Model.Acts;

namespace OpenIZ.Mobile.Core.Android.Services.ServiceHandlers
{
	/// <summary>
	/// Represents a queue service for managing patients.
	/// </summary>
	[RestService("/__queue")]
	public class QueueService
	{
		/// <summary>
		/// Dequeues a patient from the queue.
		/// </summary>
		/// <param name="patient">The patient to be removed from the queue.</param>
		/// <returns>Returns the newly removed patient.</returns>
		[RestOperation(Method = "POST", UriPath = "/Patient/Dequeue", FaultProvider = nameof(QueueFault))]
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		public Patient Dequeue(Patient patient)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adds a patient to the queue.
		/// </summary>
		/// <param name="patient">The patient to be added to the queue.</param>
		/// <returns>Returns the newly added patient.</returns>
		[RestOperation(Method = "POST", UriPath = "/Patient/Enqueue", FaultProvider = nameof(QueueFault))]
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		public Patient Enqueue([RestMessage(RestMessageFormat.SimpleJson)]Patient patient)
		{
			//PatientEncounter encounter = new PatientEncounter();

			//encounter.Participations.Add(new ActParticipation(null, patient));
			throw new NotImplementedException();
		}

		/// <summary>
		/// Handles a fault.
		/// </summary>
		/// <param name="e">The exception which occurred as a part of the fault.</param>
		/// <returns>Returns an error result.</returns>
		public ErrorResult QueueFault(Exception e)
		{
			return new ErrorResult()
			{
				Error = e.Message,
				ErrorDescription = e.InnerException?.Message
			};
		}
	}
}