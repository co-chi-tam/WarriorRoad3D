"use strict";

const Promise 			= require('promise');
const UserModel 		= require('../models/userModel');
const HeroModel 		= require('../models/heroModel');
const BattleLogModel 	= require('../models/battleLogModel');
const SocketModel 		= require('../models/socketModel');
const RoomCtrl 			= require('../controllers/room_controller');
const _ 				= require('underscore');

require('../Utils/Log')();
require('../models/resultResponse')();
require ('../models/syncWorkerModel')();
require ('../models/privateSocketModel')();

var self = this;
var user;
var hero;
var battleLog;
var socket;
var syncData;

exports.init = function(data) {
	user 		= new UserModel(data);
	hero 		= new HeroModel(data);
	battleLog   = new BattleLogModel (data);
	socket 		= new SocketModel();
};

// PLAYER WINNING MATCHES
exports.clientEndMiniFightingGame = function (sender, data, server) {
	var userTmpDatabase = sender.userTmpDatabase;
	var isoTime 	= data.isoTime;
	var winnerId 	= data.winnerId;
	var closerId 	= data.closerId;
	var randomSeed 	= data.randomSeed;
	var rewardGlory = 10;
	var rewardGold 	= 200;
	battleLog.createBattleLog (isoTime, winnerId, closerId, randomSeed, rewardGlory, rewardGold)
	// CREATED BATTLE LOG
	.then((logResult) => {
		battleLog.claimRewardBattleLog (logResult.uID)
		// CLAIMED REWARD
		.then ((claimed) => {
			// FOUND WINNER OWNER.
			hero.findAndModifyHero ({ uID: winnerId }, { $inc:{ currentGlory: claimed.rewardGlory, currentGold: claimed.rewardGold } })
			// FOUND
			.then ((heroFound) => {
				// WINNING DATA REWARD
				var clientEvent = 'clientGetRewardBattle';
				var clientData = {
					glory: claimed.rewardGlory,
					gold: claimed.rewardGold
				};
				// SEND PLAYER CLAIM REWARD
				server.sendTo (sendPrivateData (heroFound.uOwner, clientEvent, clientData));
			})
			// ERROR
			.catch ((heroError) => {
				// SEND CLIENT ERROR
				var clientEvent = 'error';
				var clientData = { 'error': heroError };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		})
		// ERROR
		.catch ((logError) => {
			// SEND CLIENT ERROR
			var clientEvent = 'error';
			var clientData = { 'error': logError };
			socket.sendMessage (sender, clientEvent, clientData);
		});
	})
	// ERROR
	.catch ((logError) => {
		// SEND CLIENT ERROR
		var clientEvent = 'debug';
		var clientData = { 'debug': logError };
		socket.sendMessage (sender, clientEvent, clientData);
	});
}

































