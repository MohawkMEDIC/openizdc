/// <reference path="~/js/openiz.js"/>
/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2017-3-31
 */

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>

layoutApp.controller('SyncCentreController', ['$scope', function ($scope) {


    $scope.queue = {};
    $scope.closeQueue = closeQueue;
    $scope.requeueAllDead = requeueAllDead;
    $scope.renderType = renderType;
    $scope.renderB64 = renderB64;

    function getQueue(queueName) {
        OpenIZ.Queue.getQueueAsync({
            queueName: queueName,
            continueWith: function (data) {
                $scope.queue[queueName] = data;
                $scope.$apply();
            },
            onException: function (data) {
                $scope.queue[queueName] = { Size: 0 };
                $scope.$apply();
            }
        });
    }

    function closeQueue() {
        console.log("close");
        $scope.queue.current = null;
        delete $scope.queue.current;
    }

    function requeueAllDead(queueId, acknowledgedUnsafe) {

        if (!acknowledgedUnsafe && !confirm(OpenIZ.Localization.getString("locale.sync.batchForceConfirm")))
            return;

        OpenIZ.App.showWait("#reQueueDead")

        OpenIZ.Queue.requeueDeadAsync({
            queueId: $scope.queue["dead"].CollectionItem[queueId || 0].id,
            continueWith: function (data, state) {
                
                // Last queue item?
                var idx = ((state || queueId || 0) + 1);
                if (idx > $scope.queue["dead"].CollectionItem.length) {
                    getQueue(OpenIZ.Queue.QueueNames.DeadLetterQueue);
                    OpenIZ.App.hideWait("#reQueueDead")
                }
                else
                    requeueAllDead(idx, true);
            },
            state: queueId || 0,
            onException: function (ex) {
                if (ex.message)
                    alert(OpenIZ.Localization.getString(ex.message));
                else
                    console.error(ex);
                OpenIZ.App.hideWait("#reQueueDead")

            }
        });
    };

    function renderType(typeName) {
        var pat = /^((\w+)[\.,])*/;
        var matches = pat.exec(typeName);
        return matches[2];
    }

    function renderB64(tag) {
        if (tag) {
            var tagContent = atob(tag);
            return tagContent;
        }
    }

    getQueue(OpenIZ.Queue.QueueNames.InboundQueue);
    getQueue(OpenIZ.Queue.QueueNames.OutboundQueue);
    getQueue(OpenIZ.Queue.QueueNames.DeadLetterQueue);
    OpenIZ.App.getInfoAsync({
        continueWith: function (d) {
            $scope.about = d;
            $scope.$apply();
        }
    })

}]);