"use strict";

require('../Utils/Log')();

const EventEmitter = require('events');
const Promise = require('promise');
const SocketModel = require('../models/socketModel');
const RoomModel = require ('../models/roomModel');
const _ = require ('underscore');
const async = require("async");

class rEvent extends EventEmitter {}

var roomController = function(roomName, maxPlayer) {
	// SOCKET
	var socket	= new SocketModel();
	// ROOM MODEL
	var room 	= new RoomModel(roomName, maxPlayer);
	// SCOPE
	var self 	= this;
	// ROOM ALREADY UPDATE
	var roomUpdated = false;
	// Delay ROOM UPDATE
	var roomDelayUpdate = 3000;
	// ROOM UPDATE TIMER
	var roomTimer;
	// ROOM EVENT
	var roomEvent = new rEvent();
	// ADD PLAYER IN ROOM
	this.addPlayer = function (player) {
		return room.addPlayer (player);
	}
	// REMOVE PLAYER IN ROOM 
	this.removePlayer = function (player) {
		return room.removePlayer (player);
	}
	// REMOVE PLAYER IN ROOM 
	this.removeUser = function (user) {
		return room.removeUser (user);
	}
	// ROOM INFO
	this.getRoomInfo = function () {
		return room.getRoomInfo();
	};
	// START ROOM UPDATE
	this.startRoom = function(server) {
		// UPDATE PER SECOND
		if (roomUpdated == true)
			return;
		roomUpdated = true;
		async.forever(function(next) {
			// ROOM RESPONSE PER SECOND.
			var eventResponseCode = room.getResponseCode ();
			var playerInRoom = room.getPlayerInRoom ();
			var clientEvent = eventResponseCode;
			var clientData = { 'roomResponse': 'This is room response.' };
			playerInRoom.forEach (function (player) {
				server.receivePrivate ({
					privateId: player.userId,
					eventName: clientEvent,
					eventData: clientData
				});
			});
			// UPDATE REPEAT
			if (roomUpdated) {
				//Repeat after the delay
				roomTimer = setTimeout(function() {
					next();
				}, roomDelayUpdate);
			} else if (roomUpdated == false || roomInfo.length == 0) {
				// CLEAR TIMEOUT
				clearTimeout (roomTimer);
				roomUpdated = false;
			}
		});
		//  ========================= EVENTS =========================
		roomEvent.on ('onFightingRoomPlayerReady',		self.onFightingRoomPlayerReady);
	};
	// STOP UPDATE
	this.stopRoom = function() {
		roomUpdated = false;
	}
	// ROOM RECEIVE DATA
	this.receivePlayerData = function (data, player, server) {
		var eventName = data.eventName;
		var eventData = data.eventData;
		if (eventData != 'null') {
			eventData = eventData.replace (/'/g, '"');
		} else {
			eventData = '{}';
		}
		roomEvent.emit(eventName, JSON.parse (eventData), player, server);	
	}
	// ROOM PLAYER READY
	this.onFightingRoomPlayerReady = function (eventData, player, server) {
		// CLIENT RECEIVE DATA
		var clientData 	= {
			resultPerSecond: 15,
			awardPerBoard: 150
		};
		// SEND TO CLIENT
		server.receivePrivate ({
			privateId: player.userId,
			eventName: 'clientFightingRoomReceiveMessage',
			eventData: clientData
		});
	}
	
};

module.exports = roomController;














