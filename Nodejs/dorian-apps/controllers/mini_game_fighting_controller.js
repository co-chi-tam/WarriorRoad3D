"use strict";

const Promise = require('promise');
const UserModel = require('../models/userModel');
const HeroModel = require('../models/heroModel');
const SocketModel = require('../models/socketModel');
const RoomCtrl = require ('../controllers/room_controller');
const _ = require ('underscore');

require('../Utils/Log')();
require('../models/resultResponse')();
require ('../models/syncWorkerModel')();
require ('../models/privateSocketModel')();

var self = this;
var user;
var hero;
var socket;
var syncData;

// ROOM MAP
var roomMap = [];
// MAX ROOM
var maxRoom = 20;
// MAX PLAYER EACH ROOM
var maxPlayer = 2;

exports.init = function(data) {
	user 		= new UserModel(data);
	hero 		= new HeroModel(data);
	socket 		= new SocketModel();
};

// ON SERVER ROOM MAP
exports.clientGetFightingRoomList = function(sender) {
	// SEND CLIENT DATA
	var responseRoomMap = [];
	for (var i = 0; i < maxRoom; i++) {
		var room = roomMap [i];
		if (typeof room === 'undefined') {
			var emptyRoom = new RoomCtrl ('Room' + i, maxPlayer);
			responseRoomMap[i] = emptyRoom.getRoomInfo();
		} else {
			responseRoomMap[i] = room.getRoomInfo();
		}	
	}
	var clientEvent = 'clientReceiveFightingRoomList';
	var clientData = { 'roomListData': responseRoomMap };
	socket.sendMessage (sender, clientEvent, clientData);
}

// ON SERVER JOINED ROOM
exports.serverPlayerJoinedFightingRoom = function(roomSyncData, server) {
	var roomIndex = parseInt (roomSyncData.roomIndex);
	var playerRequest = roomSyncData.playerRequest;
	// CHECK PLAYER JOINED
	var indexRoomJoined = _.findIndex (roomMap, function (item) {
		if (typeof item === 'undefined') {
			return false;
		}
		var room = item.getRoomInfo();
		return _.findIndex (room.playerInRoom, function (playerItem) {
			return playerItem.playerId == playerRequest.playerId;
		}) != -1;
	});
	if (indexRoomJoined == -1) {
		// FIND ROOM
		var selectRoom = roomMap[roomIndex];
		// CHECK ROOM EXISTED
		if (typeof selectRoom === 'undefined') {
			// CREATE EMPTY ROOM
			var roomName = 'Room' + roomIndex;
			selectRoom = new RoomCtrl (roomName, maxPlayer);
			// ADD ROOM LIST
			roomMap[roomIndex] = selectRoom;
		}
		// ADD PLAYER
		selectRoom.addPlayer (playerRequest)
		//JOINED
		.then ((roomJoined) => {
			// START UPDATE ROOM
			selectRoom.startRoom(server);
			// SEND CLIENT ROOM INIT
			var clientEvent = 'clientInitFightingRoom';
			var clientData = { 'roomData': roomJoined };
			server.receivePrivate ({
				privateId: playerRequest.userId,
				eventName: clientEvent,
				eventData: clientData
			});
			// SEND CLIENT ROOM UPDATE
			var responseRoomMap = [];
			for (var i = 0; i < maxRoom; i++) {
				var room = roomMap [i];
				if (typeof room === 'undefined') {
					var emptyRoom = new RoomCtrl ('Room' + i, maxPlayer);
					responseRoomMap[i] = emptyRoom.getRoomInfo();
				} else {
					responseRoomMap[i] = room.getRoomInfo();
				}	
			}
			var clientUpdateEvent = 'clientReceiveFightingRoomList';
			var clientUpdateData = { 'roomListData': responseRoomMap };
			server.receiveBroadCast ({
				eventName: clientUpdateEvent,
				eventData: clientUpdateData
			});
		})
		// ERROR
		.catch ((error) => {
			// SEND CLIENT WARNING
			var clientEvent = 'warning';
			var clientData = { 'warning': error };
			server.receivePrivate ({
				privateId: playerRequest.userId,
				eventName: clientEvent,
				eventData: clientData
			});
		});
	} else {
		// SEND CLIENT WARNING
		var clientEvent = 'warning';
		var clientData = { 'warning': "PLAYER JOINED ROOM " + indexRoomJoined };
		server.receivePrivate ({
			privateId: playerRequest.userId,
			eventName: clientEvent,
			eventData: clientData
		});
	}
}

// ON SERVER PLAYER LEAVE ROOM
exports.serverPlayerLeaveFightingRoom = function (roomSyncData, server) {
	var playerRequest = roomSyncData.playerRequest;
	// CHECK PLAYER JOINED
	var indexRoomJoined = _.findIndex (roomMap, function (item) {
		if (typeof item === 'undefined') {
			return false;
		}
		var room = item.getRoomInfo();
		return _.findIndex (room.playerInRoom, function (playerItem) {
			return playerItem.playerId == playerRequest.playerId;
		}) != -1;
	});
	if (indexRoomJoined != -1) {
		// FILTER ROOM
		var roomSelected = roomMap[indexRoomJoined];
		// REMOVE PLAYER
		roomSelected.removePlayer (playerRequest)
		.then((roomUpdate) => {
			// SEND CLIENT WARNING
			var clientEvent = 'warning';
			var clientData = { 'warning': "PLAYER ALREADY LEAVE ROOM." };
			server.receivePrivate ({
				privateId: playerRequest.userId,
				eventName: clientEvent,
				eventData: clientData
			});
			// SEND CLIENT ROOM UPDATE
			var responseRoomMap = [];
			for (var i = 0; i < maxRoom; i++) {
				var room = roomMap [i];
				if (typeof room === 'undefined') {
					var emptyRoom = new RoomCtrl ('Room' + i, maxPlayer);
					responseRoomMap[i] = emptyRoom.getRoomInfo();
				} else {
					responseRoomMap[i] = room.getRoomInfo();
				}	
			}
			var clientUpdateEvent = 'clientReceiveFightingRoomList';
			var clientUpdateData = { 'roomListData': responseRoomMap };
			server.receiveBroadCast ({
				eventName: clientUpdateEvent,
				eventData: clientUpdateData
			});
		})
		.catch ((error) => {
			// SEND CLIENT WARNING
			var clientEvent = 'warning';
			var clientData = { 'warning': error };
			server.receivePrivate ({
				privateId: playerRequest.userId,
				eventName: clientEvent,
				eventData: clientData
			});
		});
	} else {
		// SEND CLIENT WARNING
		var clientEvent = 'warning';
		var clientData = { 'warning': "PLAYER HAVE NOT A ROOM." };
		server.receivePrivate ({
			privateId: playerRequest.userId,
			eventName: clientEvent,
			eventData: clientData
		});
	}
}

// ON SERVER RECEIVE DATA
exports.serverReceiveDataFightingRoom = function (roomSyncData, server) {
	var playerData = roomSyncData.playerData;
	var playerRequest = roomSyncData.playerRequest;
	// CHECK PLAYER JOINED
	var indexRoomJoined = _.findIndex (roomMap, function (item) {
		if (typeof item === 'undefined') {
			return false;
		}
		var room = item.getRoomInfo();
		return _.findIndex (room.playerInRoom, function (playerItem) {
			return playerItem.playerId == playerRequest.playerId;
		}) != -1;
	});
	if (indexRoomJoined != -1) {
		// FILTER ROOM
		var roomSelected = roomMap[indexRoomJoined];
		// ROOM RECEIVE DATA
		roomSelected.receivePlayerData(playerData, playerRequest, server);
	} else {
		// SEND CLIENT WARNING
		var clientEvent = 'warning';
		var clientData = { 'warning': "PLAYER HAVE NOT A ROOM." };
		server.receivePrivate ({
			privateId: playerRequest.userId,
			eventName: clientEvent,
			eventData: clientData
		});
	}
}

// PLAYER JOIN ROOM
exports.clientRequestJoinFightingRoom = function (sender, data, server) {
	var userTmpDatabase = sender.userTmpDatabase;
	var roomIndex = data.roomIndex;
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((player) => {
		// SYNC WORKER ROOM INFO
		server.sendTo (sendSyncData ('serverPlayerJoinedFightingRoom', {
			roomIndex: roomIndex,
			playerRequest: {
				userId: userTmpDatabase.userId,
				playerId: player.uID,
				objectName: player.objectName,
				objectModel: player.objectModel,
				objectAvatar: player.objectAvatar,
				currentGold: player.currentGold,
				isReady: false
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

// LEAVE ROOM
exports.clientRequestLeaveFightingRoom = function (sender, data, server) {
	var userTmpDatabase = sender.userTmpDatabase;
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((player) => {
		// SYNC WORKER ROOM INFO
		server.sendTo (sendSyncData ('serverPlayerLeaveFightingRoom', {
			playerRequest: {
				userId: userTmpDatabase.userId,
				playerId: player.uID,
				objectName: player.objectName,
				objectModel: player.objectModel,
				objectAvatar: player.objectAvatar,
				currentGold: player.currentGold,
				isReady: false
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

// SEND TO ROOM SINGLE WORKER
exports.clientSendDataFightingRoom = function (sender, data, server) {
	var userTmpDatabase = sender.userTmpDatabase;
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((player) => {
		// SYNC WORKER ROOM INFO
		var sendData = { 
			syncEvent: 'serverReceiveDataFightingRoom',
			syncData: {
				playerData: data,
				playerRequest: {
					userId: userTmpDatabase.userId,
					playerId: player.uID,
					objectName: player.objectName,
					objectModel: player.objectModel,
					objectAvatar: player.objectAvatar,
					currentGold: player.currentGold
				}
			}
		};
		server.syncWorkerData (sendData);
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		var clientEvent = 'error';
		var clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});	
}
































