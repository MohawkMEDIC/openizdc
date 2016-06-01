/**
 * OpenIZ wrapper binding
 *
 * The purpose of this file is to provide a convenient JavaScript wrapper around the OpenIZ host container for applets.
 * This file should be changed whenever you are writing an Applet container which will host applets.
 */

/**
 * Namespace for OpenIZ model wrapper JAvaScript classes
 */
var OpenIZModel = new function () {
    /**
     * Represents OpenIZ session data, rather, the session that is currently in play.
     * @ckass OpenIZ session information
     * @constructor
     */
    this.Session = function (sessionData) {

        /**
         * The information related to the principal
         */
        this.identity = {
            name: sessionData.username,
            roles: sessionData.roles
        };
        /**
         * The method of authentication, either local, oauth, or basic
         */
        this.method = sessionData.method;
        /**
         * The date on which the session will expire
         */
        this.expires = sessionData.exp;
        /**
         * The date on which the session was issued
         */
        this.issued = sessionData.nbf;
        /**
         * The token which can be used as a refresh
         */
        this.refresh_token = sessionData.refresh_token;
        /**
         * Contains the JWT token assets (name, etc.) from the session
         */
        this.jwt = sessionData.jwt;

        /**
         * Returns whether the current session is expired
         */
        this.isExpired = function () {
            return new Date() > this.expires;
        };
        /**
         * The abandon function is used to abandon the current session
         */
        this.abandon = function () {
            // TODO: Implement
        };
        /**
         * The refresh function is used to refresh the current session 
         */
        this.refresh = function () {
            // TODO: Implement
        };
    };

    /**
     * Represents a security user (user information)
     * @class Security User model
     * @constructor
     */
    this.SecurityUser = function (securityUserData) {

        /**
         * Saves the current security user object in the back-end
         */
        this.save = function () {

        };

        /**
         * Obsoletes the specified security user
         */
        this.obsolete = function () {

        };

    };

    /**
     * This class represents data related to a patient
     * @class The model patient class
     * @constructor
     */
    this.Patient = function (patientData) {

    };
};
