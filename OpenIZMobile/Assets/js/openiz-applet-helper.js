
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

	Util : {
		toDateInputString : function(date) {
			return date.toISOString().substring(0,10)
		}
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
		}
	},
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
			alert("I will join " + address);
		},
		leaveRealm : function() {
			if(!confirm('You are about to leave the realm ' + configuration.realm.address + '. Doing so will force the OpenIZ back into an initial configuration mode. Are you sure you want to do this?'))
				return;
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