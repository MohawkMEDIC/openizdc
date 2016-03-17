
// Bind datepickers
$(document).ready(function() {


	$('input[type="date"]').each(function(k,v){
		if($(v).attr('data-max-date'))
		{
			var date = new Date();
			date.setDate(date.getDate() + parseInt($(v).attr('data-max-date')));
			var maxDate = OpenIZ.Util.toDateInputString(date);
			$(v).attr('max', maxDate);
		}
		if($(v).attr('data-min-date'))
		{
			var date = new Date();
			date.setDate(date.getDate() + parseInt($(v).attr('data-min-date')));
			var minDate = OpenIZ.Util.toDateInputString(date)
			$(v).attr('min', minDate);
		}
	});
	$('select[data-openiz-tag="select2"]').each(function(k,v) {

		// TODO: update this
		$(v).select2({
			dropdownAutoWidth:false


		});
	});
})

// OpenIZ Bridge functions
var OpenIZ = {
	urlParams : {},
	Util : {
		toDateInputString : function(date) {
			return date.toISOString().substring(0,10)
		},

	},
	// Instruct the mobile app to perform a barcode scan, placing the result in @target
	App : {
		scanBarcode : function() {
			try
			{
				var value = OpenIZApplicationService.BarcodeScan();
				console.log('Barcode scan complete. Data: ' + value);
				return value;
			}
			catch(e)
			{
				console.error(e);
			}
		},
		close : function() {
			try
			{
				var value = OpenIZApplicationService.Back();
			}
			catch(e)
			{
				console.error(e);
			}
		},
		toast : function(text) {
			try
			{
				var value = OpenIZApplicationService.ShowToast(text);
			}
			catch(e)
			{
				console.error(e);
			}
		},
		// Navigates to an applet
		navigateApplet : function(appletId, context)
		{
			try
			{
				OpenIZApplicationService.Navigate(appletId, JSON.stringify(context));
			}
			catch(e)
			{
				console.error(e);
			}
		},
		getString : function(stringId) {
			try
			{
				return OpenIZApplicationService.GetString(stringId);
			}
			catch(e)
			{
				console.error(e);
			}
		}
	},
	Concept : {
		getConceptSet : function(setName) {
			try
			{
				var results = JSON.parse(OpenIZConceptService.GetConceptSet(setName));
				return results;
			}
			catch(e)
			{
				console.error(e);
			}
		}
	},
	// Patient Functions
	Patient : {

		// Instruct the mobile app to look for patients
		search : function(formData) {
			try
			{
				var results = JSON.parse(OpenIZPatientService.Find(formData));
				return results;
			}
			catch(e)
			{
				console.error(e);
			}
		}
	},
	Configuration : {
		joinRealm : function(address) {
			try
			{
				OpenIZConfigurationService.JoinRealm(address);
			}
			catch(e)
			{
				console.error(e);
			}
		},
		leaveRealm : function() {
			try
			{
				if(!confirm('You are about to leave the realm ' + configuration.realm.address + '. Doing so will force the OpenIZ back into an initial configuration mode. Are you sure you want to do this?'))
					return;
				OpenIZConfigurationService.LeaveRealm();
			}
			catch(e)
			{
				console.error(e);
			}
		},
		save : function(configuration) {
			try
			{
				if(OpenIZConfigurationService.Save(JSON.stringify(configuration)))
				{
					OpenIZ.App.toast("Changes will take effect when OpenIZ is restarted");
					return true;
				}
				else
					OpenIZ.App.toast("Saving configuration failed");
			}
			catch(e)
			{
				console.error(e);
			}
		}
	}
};

// Parameters
(window.onpopstate = function () {
    var match,
        pl     = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query  = window.location.search.substring(1);

    OpenIZ.urlParams = {};
    while (match = search.exec(query))
       OpenIZ.urlParams[decode(match[1])] = decode(match[2]);
})();