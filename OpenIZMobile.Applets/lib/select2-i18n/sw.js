/*! Select2 4.0.3 | https://github.com/select2/select2/blob/master/LICENSE.md */

(function () {
    if (jQuery && jQuery.fn && jQuery.fn.select2 && jQuery.fn.select2.amd) var e = jQuery.fn.select2.amd;
    return e.define("select2/i18n/sw", [], function () {
        return {
            errorLoading: function () {
                return "Matokeo haikuweza kupakiwa."
            },
            inputTooLong: function (e) {
                var t = e.input.length - e.maximum;

                return "Tafadhali futa " + (t > 1 ? "wahusika" : "herufi") + t;
            },
            inputTooShort: function (e) {
                var t = e.minimum - e.input.length,
                    n = "Tafadhali ingiza herufi " + t + " au zaidi";
                return n
            },
            loadingMore: function () {
                return "Kupakia matokeo zaidi..."
            },
            maximumSelected: function (e) {
                var t = "Unaweza tu kuchagua " + (e.maximum > 1 ? "vitu " + e.maximum : e.maximum + " vitu");
                return t
            },
            noResults: function () {
                return "Hakuna matokeo ya kupatikana"
            },
            searching: function () {
                return "Kutafuta..."
            }
        }
    }), {
        define: e.define,
        require: e.require
    }
})();