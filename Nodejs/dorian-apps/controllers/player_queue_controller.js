"use strict";

const Promise 		= require('promise');
const UserModel 	= require('../models/userModel');
const HeroModel 	= require('../models/heroModel');
const SocketModel 	= require('../models/socketModel');
const EUserAction 	= require('../models/userAction');
const _ 			= require ('underscore');
const crypto 		= require('crypto');

require('../Utils/Log')();
require('../models/resultResponse')();
require ('../models/syncWorkerModel')();
require ('../models/privateSocketModel')();

var self = this;
var user;
var hero;
var socket;
var defaultRandomSeed = 188888;

// ROOM MAP
var currentQueue = [];

exports.init = function(data) {
	user 		= new UserModel(data);
	hero 		= new HeroModel(data);
	socket 		= new SocketModel();
};
// PLAYER JOIN ROOM
exports.clientRequestJoinPlayerQueue = function (sender, data, server) {
	var userTmpDatabase = sender.userTmpDatabase;
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((player) => {
		var currentEnergy = player.currentEnergy;
		if (currentEnergy > 0) {
			hero.modifyHero (userTmpDatabase.userId, 
			{$set: {currentEnergy: currentEnergy - 1, 
					lastUpdateEnergy: new Date()}})
			// UPDATE COMPLETED
			.then ((updated) => {
				// SYNC WORKER ROOM INFO
				server.sendTo (sendSyncData ('serverPlayerJoinedPlayerQueue', {
					playerRequest: {
						userId: userTmpDatabase.userId,
						playerId: player.uID,
						objectName: player.objectName,
						objectModel: player.objectModel,
						objectAvatar: player.objectAvatar,
						characterLevel: player.characterLevel,
						currentGold: player.currentGold,
						userAction: sender.userAction
					}
				}));
			})
			// UPDATE ERROR
			.catch ((errorUpdate) => {
				// SEND CLIENT ERROR
				clientEvent = 'error';
				clientData = { 'error': errorUpdate };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		} else {
			// SEND CLIENT ERROR
			var clientEvent = 'warning';
			var clientData = { 'warning': 'You not enought energy.' };
			socket.sendMessage (sender, clientEvent, clientData);
		}
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		var clientEvent = 'error';
		var clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});	
}
// LEAVE ROOM
exports.clientRequestLeavePlayerQueue = function (sender, data, server) {
	var userTmpDatabase = sender.userTmpDatabase;
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((player) => {
		// SYNC WORKER LEAVE QUEUE
		server.sendTo (sendSyncData ('serverPlayerLeavePlayerQueue', {
			playerRequest: {
				userId: userTmpDatabase.userId,
				playerId: player.uID,
				objectName: player.objectName,
				objectModel: player.objectModel,
				objectAvatar: player.objectAvatar,
				characterLevel: player.characterLevel,
				currentGold: player.currentGold,
				userAction: sender.userAction
			}
		}));
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		var clientEvent = 'error';
		var clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});	
}
// ON SERVER JOINED ROOM
exports.serverPlayerJoinedPlayerQueue = function(syncData, server) {
	var playerRequest = syncData.playerRequest;
	// ADD QUEUE LIST
	var playerIndex = _.findIndex (currentQueue, function(item) {
		return item.userId == playerRequest.userId;
	});
	if (playerIndex == -1) {
		currentQueue.push (playerRequest);
	}
	// FOUND QUEUE
	var playerOther = currentQueue[0];
	for	(var i = currentQueue.length - 1; i > 0; i--) {
		if (currentQueue[i].userId != playerRequest.userId) {
			playerOther = currentQueue[i];
			break;
		}
	}
	// FOUND IN QUEUE PLAYER
	if (currentQueue.length > 1 && playerOther.userId != playerRequest.userId) {
		hero.findHero(playerRequest.userId)
		// FOUND HERO
		.then ((playerHero) => {
			// REMOVE LAST PLAYER
			currentQueue.splice (currentQueue.indexOf(playerOther), 1);
			// REMOVE PLAYER
			currentQueue.splice (currentQueue.indexOf(playerRequest), 1);
			// SET SOCKET USER ACTION
			server.setPrivateUserAction (playerRequest.userId, EUserAction.PROCESSING_PLAYER_QUEUE);
			// SET SOCKET USER ACTION
			server.setPrivateUserAction (playerOther.userId, EUserAction.PROCESSING_PLAYER_QUEUE);
			hero.findHero(playerOther.userId)
			// FOUND HERO
			.then ((otherPlayerHero) => {
				// SEND CLIENT BATTLE INFO
				var currentISOTime = new Date().toISOString();
				// var battleLogId = crypto.createHash('md5').update('log+' 
									// + playerHero.uID + '-vs-' + otherPlayerHero.uID 
									// + '+' + currentISOTime).digest('hex');
				var clientEvent = 'clientReceiveResultPlayerQueue';
				playerHero.characterHealthPoint = playerHero.characterMaxHealthPoint;
				otherPlayerHero.characterHealthPoint = otherPlayerHero.characterMaxHealthPoint;
				var clientData = { 	isoTime: currentISOTime,
									playerData: playerHero,
									enemyData: otherPlayerHero,
									randomSeed: defaultRandomSeed };
				// SEND PLAYER REQUEST
				server.receivePrivate ({
						privateId: playerRequest.userId,
						eventName: clientEvent,
						eventData: clientData
					});
				// SEND OTHER PLAYER
				server.receivePrivate ({
						privateId: playerOther.userId,
						eventName: clientEvent,
						eventData: clientData
					});
			})
			// ERROR FOUND
			.catch ((errorFound) => {
				// SEND CLIENT ERROR
				var clientEvent = 'error';
				var clientData = { 'error': errorFound };
				server.receivePrivate ({
							privateId: playerRequest.userId,
							eventName: clientEvent,
							eventData: clientData
						});
			});
		})
		// ERROR FOUND
		.catch ((errorFound) => {
			// SEND CLIENT ERROR
			var clientEvent = 'error';
			var clientData = { 'error': errorFound };
			server.receivePrivate ({
						privateId: playerRequest.userId,
						eventName: clientEvent,
						eventData: clientData
					});
		});	
	} else {
		// SEND CLIENT WAITING 3 SECOND
		var clientEvent = 'clientWaitPlayerQueue';
		var clientData = { 'waitingTime': 3 };
		server.receivePrivate ({
				privateId: playerRequest.userId,
				eventName: clientEvent,
				eventData: clientData
			});
	}
}
// ON SERVER PLAYER LEAVE ROOM
exports.serverPlayerLeavePlayerQueue = function (syncData, server) {
	var playerRequest = syncData.playerRequest;
	// SET SOCKET USER ACTION
	server.setPrivateUserAction (playerRequest.userId, EUserAction.CANCEL_PLAYER_QUEUE);
	// SEND CLIENT CANCEL PLAYER QUEUE
	var clientEvent = 'clientCancelPlayerQueue';
	var clientData = { 'canceled': true };
	server.receivePrivate ({
			privateId: playerRequest.userId,
			eventName: clientEvent,
			eventData: clientData
		});
	// REMOVE PLAYER
	var playerIndex = _.findIndex (currentQueue, function(item) {
		return item.userId == playerRequest.userId;
	});
	if (playerIndex != -1) {
		currentQueue.splice (playerIndex, 1);
	}
}





























