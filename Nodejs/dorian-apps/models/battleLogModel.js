require('../Utils/Log')();

const uuid = require('node-uuid');
const crypto = require('crypto');
const simpleData = require ('./simpleData');
const Promise = require('promise');
const RegExp = require("regex");

var battleLogModel = function(database) {
	// SCOPE
	var self = this;
	// COLLECTION NAME
	var collectionName = 'clBattleLogs';
	// MODEL
	var simpleModel = new simpleData(collectionName, database);	
	// NEW BATTLE LOG
	var newBattleLog = function (isoTime, winnerId, closerId, randomSeed, rewardGlory, rewardGold) {
		return {
			uID: crypto.createHash('md5').update('=' + isoTime + '+' + winnerId + '+' + closerId + '+' + randomSeed).digest('hex'),
			isoTime: isoTime,
			winnerId: winnerId,
			closerId: closerId,
			randomSeed: randomSeed,
			reward: {
				glory: rewardGlory,
				gold: rewardGold
			},
			createdDate: new Date(),
			claimRewardDate: new Date(),
			activeLog: false
		};
	}
	// RESPONSE BATTLE LOG
	var responseBattleLog = function (log) {
		return {
			logId: log.uID,
			rewardGlory: log.reward.glory,
			rewardGold: log.reward.gold,
			claimRewardDate: log.claimRewardDate
		};
	}
	// FIND
	this.findBattleLog = function (logId) {
		return new Promise(function (resolve, reject) {
			simpleModel.findData ({uID: logId})
			// FOUND 
			.then ((completeFind) => {
				resolve(completeFind[0]);
			})
			// ERROR 
			.catch ((errorFind) => {
				reject(errorFind);
			});
		});
	}
	// FIND
	this.findBattleLogWith = function (isoTime, winnerId, closerId, randomSeed) {
		return new Promise(function (resolve, reject) {
			// FIELDS
			if (isoTime && winnerId && closerId && randomSeed) {
				var logId = crypto.createHash('md5').update('=' + isoTime + '+' + winnerId + '+' + closerId + '+' + randomSeed).digest('hex');
				simpleModel.findData ({uID: logId})
				// FOUND 
				.then ((completeFind) => {
					resolve(completeFind[0]);
				})
				// ERROR 
				.catch ((errorFind) => {
					reject(errorFind);
				});
			} else {
				reject('FIELDS DO NOT EMPTY');
			}
		});
	}
	// CREATE BATTLE LOG
	this.createBattleLog = function (isoTime, winnerId, closerId, randomSeed, rewardGlory, rewardGold) {
		return new Promise(function (resolve, reject) {
			// FIELDS
			if (isoTime && winnerId && closerId && randomSeed && rewardGlory && rewardGold) {
				// NEW LOG
				var newLog = new newBattleLog (isoTime, winnerId, closerId, randomSeed, rewardGlory, rewardGold);
				simpleModel.findData ({uID: newLog.uID})
				// FOUND 
				.then((result) => {
					resolve (result);
				})
				// THIS IS NEW LOG
				.catch((errFind) => {
					simpleModel.createData (newLog)
					// CREATE LOG
					.then ((userDocs) => {
						resolve (newLog);
					})
					// ERROR CREATE
					.catch ((errCreate) => {
						reject (errCreate);
					});
				});
			} else {
				reject('FIELDS DO NOT EMPTY');
			}
		});
	}
	// CLAIM REWARD BATTLE LOG
	this.claimRewardBattleLog = function (logId) {
		return new Promise(function (resolve, reject) {
			simpleModel.findData ({uID: logId})
			// FOUND 
			.then ((completeFind) => {
				simpleModel.updateData ({uID: logId}, {'claimRewardDate': new Date(), 'activeLog': true})
				// UPDATE LOG
				.then ((completeUpdate) => {
					// RESPONSE LOG
					resolve (new responseBattleLog(completeFind[0]));
				})
				// CAN NOT UPDATE USER
				.catch ((errorUpdate) => {
					reject('ERROR ' + errorUpdate);
				});
			})
			// ERROR 
			.catch ((errorFind) => {
				reject(errorFind);
			});
		});
	}
}

module.exports = battleLogModel;
