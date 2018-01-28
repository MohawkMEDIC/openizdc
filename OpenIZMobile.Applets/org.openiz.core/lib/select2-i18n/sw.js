/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-10-30
 */
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