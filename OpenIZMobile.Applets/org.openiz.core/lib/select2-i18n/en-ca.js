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
(function () { if (jQuery && jQuery.fn && jQuery.fn.select2 && jQuery.fn.select2.amd) var e = jQuery.fn.select2.amd; return e.define("select2/i18n/en-ca", [], function () { return { errorLoading: function () { return "The results could not be loaded." }, inputTooLong: function (e) { var t = e.input.length - e.maximum, n = "Please delete " + t + " character"; return t != 1 && (n += "s"), n }, inputTooShort: function (e) { var t = e.minimum - e.input.length, n = "Please enter " + t + " or more characters"; return n }, loadingMore: function () { return "Loading more results..." }, maximumSelected: function (e) { var t = "You can only select " + e.maximum + " item"; return e.maximum != 1 && (t += "s"), t }, noResults: function () { return "No results found" }, searching: function () { return "Searching..." } } }), { define: e.define, require: e.require } })();