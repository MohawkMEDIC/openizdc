
// Bind datepickers
$(document).ready(function() {
	$('input[data-type="date"]').each(function(k,v) {
		$(v).datepicker({
			maxDate: $(v).attr('data-max-date') ? $(v).attr('data-max-date') : null,
			minDate: $(v).attr('data-min-date') ? $(v).attr('data-max-date') : null,
			dateFormat: $(v).attr('data-date-format') ? $(v).attr('data-date-format') : "yy-mm-dd"
			});
	});
})

// OpenIZ Bridge functions
var OpenIZ = {
	// Instruct the mobile app to perform a barcode scan, placing the result in @target
	barcodeScan : function(target) {
		try
		{
			var value = OpenIZApplication.BarcodeScan();
			console.log('Barcode scan complete. Data: ' + value + ' into ' + target);
			target.val(value);
		}
		catch(e)
		{
			console.error(e);
		}
	},
	// Instruct the mobile app to look for patients
	searchPatients : function(formData) {
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
};