require('../Utils/Log')();

const uuid 		= require('node-uuid');
const crypto 	= require('crypto');
const Promise 	= require('promise');
const _ 		= require ('underscore');

const HeroModel = require('../models/heroModel');

var roomModel = function (roomName, maxPlayer) {
	// ROOM ID
	var roomId 			= uuid.v4().toString();
	// ROOM NAME
	var roomName 		= roomName;
	// MAX PLAYER
	var maxPlayer 		= maxPlayer;
	// PLAYER IN ROOM
	var playerInRoom	= [];
	// ROOM RESPONSE CODE
	var eventResponseCode = crypto.createHash('md5')
								.update(roomName + '+' + '95c5')
								.digest('hex');
	// SCOPE
	var self = this;
	// RESPONSE
	var responseRoom = function() {
		return {
			roomId: roomId,
			roomName: roomName,
			maxPlayer: maxPlayer,
			playerInRoom: playerInRoom,
			eventResponseCode: eventResponseCode
		}
	}
	// ADD PLAYER 
	this.addPlayer = function (player) {
		return new Promise(function (resolve, reject) {
			if (playerInRoom.length >= maxPlayer) {
				reject ('ROOM MAXIMUM PLAYER');
			} else {
				// PLAYER JOINED
				var isPlayerJoined = _.findIndex (playerInRoom, function(playerItem) {
					return playerItem.playerId == player.playerId;
				}) !== -1;
				if (isPlayerJoined) {
					reject ('PLAYER JOINED');
				} else {
					playerInRoom.push (player);
					resolve (new responseRoom());
				}
			}
		});
	}
	// REMOVE PLAYER
	this.removePlayer = function (player) {
		return new Promise(function (resolve, reject) {
			try {
				playerInRoom = _.filter (playerInRoom, function (playerItem) {
					return playerItem.playerId != player.playerId;
				});
				resolve (new responseRoom());
			} catch (error) {
				reject (error);
			}
		});
	}
	// REMOVE USER
	this.removeUser = function (user) {
		return new Promise(function (resolve, reject) {
			try {
				playerInRoom = _.filter (playerInRoom, function (playerItem) {
					return playerItem.userId != user.userId;
				});
				resolve (new responseRoom());
			} catch (error) {
				reject (error);
			}
		});
	}
	// GET ROOM
	this.getRoomInfo = function () {
		return responseRoom();
	}
	// GET ROOM ID
	this.getRoomId = function () {
		return roomId;
	};
	// GET ROOM NAME
	this.getRoomName = function () {
		return roomName;
	};
	// GET ROOM PLAYERS
	this.getPlayerInRoom = function () {
		return playerInRoom;
	};
	// GET RESPONSE CODE
	this.getResponseCode = function () {
		return eventResponseCode;
	}
}

module.exports = roomModel;