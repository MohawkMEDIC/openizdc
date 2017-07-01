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

layoutApp.controller('SyncCentreController', ['$scope', '$state', '$rootScope', function ($scope, $state, $rootScope) {


    $scope.queue = {};
    $scope.closeQueue = closeQueue;
    $scope.requeueAllDead = requeueAllDead;
    $scope.renderType = renderType;
    $scope.renderB64 = renderB64;
    $scope.requeueItem = requeueItem;
    $scope.deleteQueueItem = deleteQueueItem;
    $scope.selectItem = selectItem;
    $scope.forceSync = forceSync;

    function forceSync() {

        OpenIZ.App.showWait("#forceSync");
        OpenIZ.Queue.forceResyncAsync({
            onException: function(ex) {
                if (ex.message)
                    alert(OpenIZ.Localization.getString(ex.message));
                else
                    console.error(ex);
            },
            finally: function () {
                OpenIZ.App.hideWait("#forceSync");
            }
        });
    }

    function getQueue(queueName) {
        OpenIZ.Queue.getQueueAsync({
            queueName: queueName,
            continueWith: function (data) {
                if ($scope.queue[queueName]) {
                    $scope.queue[queueName].Size = data.Size;
                    $scope.queue[queueName].CollectionItem = data.CollectionItem;
                }
                else
                    $scope.queue[queueName] = data;
                $scope.queue[queueName].name = queueName;
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

    function selectItem(id) {
        $scope.isLoading = true;
        OpenIZ.Queue.getQueueAsync({
            queueName: $scope.queue.current.name,
            id: id,
            continueWith: function (data) {
                $scope.queue.currentItem = data.CollectionItem[0];
                $scope.$apply();
            },
            onException: function (ex) {
                if (ex.message)
                    alert(OpenIZ.Localization.getString(ex.message));
                else
                    console.error(ex);
            },
            finally: function () {
                $scope.isLoading = false;
            }
        })
    }

    function requeueAllDead(queueId, acknowledgedUnsafe) {

        if (!acknowledgedUnsafe) {
            if (!confirm(OpenIZ.Localization.getString("locale.sync.batchForceConfirm")))
                return;
            OpenIZ.App.showWait("#reQueueDead")
            OpenIZ.App.showWait("#reQueueDeadModal")
        }

        var key =[];
        for (var k in $scope.queue["dead"].CollectionItem)
            key.push($scope.queue["dead"].CollectionItem[k].id);

        for (var k in key) {
            OpenIZ.Queue.requeueDeadAsync({
                queueId: key[k],
                continueWith: function (data, state) {
                    // Last queue item?
                    if(state == key.length - 1)
                    refreshQueueState(true);
                    OpenIZ.App.hideWait("#reQueueDead");
                    OpenIZ.App.hideWait("#reQueueDeadModal");
                },
                state: k,
                onException: function (ex) {
                    if (ex.message)
                        alert(OpenIZ.Localization.getString(ex.message));
                    else
                        console.error(ex);
                    OpenIZ.App.hideWait("#reQueueDead")
                    OpenIZ.App.hideWait("#reQueueDeadModal");

                }
            });
        }
    };

    function renderType(typeName) {
        var pat = /^((\w+)[\.,])*/;
        var matches = pat.exec(typeName);
        return matches[2];
    }

    // Render the specified tag from base64
    function renderB64(tag) {
        if (tag) {
            var tagContent = atob(tag);
            return tagContent;
        }
    }
    
    // Re-queue the specified item so the mobile will attempt to re-send
    // @param {int} itemId The item in the dead-letter queue to be re-queued
    function requeueItem(itemId) {
        if (!confirm(OpenIZ.Localization.getString("locale.sync.forceConfirm")))
            return;

        OpenIZ.App.showWait("#requeQueueItem");
        OpenIZ.Queue.requeueDeadAsync({
            queueId: itemId,
            continueWith: function (e) {
                refreshQueueState(true);
            },
            onException: function (ex) {
                if (ex.message)
                    alert(OpenIZ.Localization.getString(ex.message));
                else
                    console.error(ex);
            },
            finally: function () {
                OpenIZ.App.hideWait("#requeQueueItem");
                $("#resolveDialog").modal('hide');
            }
        });
    }

    // Delete the specified item so the mobile will attempt to re-send
    // @param {int} itemId The item in the dead-letter queue to be re-queued
    function deleteQueueItem(itemId) {
        if ($scope.queue.currentItem.operation == "Insert" && !confirm(OpenIZ.Localization.getString("locale.sync.deleteConfirm.insert")))
            return ;
        else if($scope.queue.currentItem.operation != "Insert" && !confirm(OpenIZ.Localization.getString("locale.sync.deleteConfirm.insert")))
            return;

        OpenIZ.App.showWait("#deleteQueueItem");

        var doDelete = function () {
            OpenIZ.Queue.deleteQueueAsync({
                queueId: itemId,
                queueName: OpenIZ.Queue.QueueNames.DeadLetterQueue,
                continueWith: function (e) {
                    refreshQueueState(true);
                },
                onException: function (ex) {
                    if (ex.type == "PolicyViolationException") return; // allow override
                    if (ex.message)
                        alert(OpenIZ.Localization.getString(ex.message));
                    else
                        console.error(ex);
                },
                finally: function () {
                    OpenIZ.App.hideWait("#deleteQueueItem");
                    $("#resolveDialog").modal('hide');
                }
            });
        };

        OpenIZ.Authentication.$elevationCredentials.continueWith = doDelete;
        doDelete();
    }

    // Refresh queue state
    // @param {bool} noTimer When true, instructs the function not to re-run on a timer
    function refreshQueueState(noTimer) {

        getQueue(OpenIZ.Queue.QueueNames.InboundQueue);
        getQueue(OpenIZ.Queue.QueueNames.OutboundQueue);
        getQueue(OpenIZ.Queue.QueueNames.DeadLetterQueue);
        getQueue(OpenIZ.Queue.QueueNames.AdminQueue);

        if (!noTimer && $state.is('org-openiz-core.sync') && $rootScope.session != null)
            setTimeout(refreshQueueState, 5000);
    }

    refreshQueueState();

    OpenIZ.App.getInfoAsync({
        continueWith: function (d) {
            $scope.about = d;
            $scope.$apply();
        }
    })

}]);