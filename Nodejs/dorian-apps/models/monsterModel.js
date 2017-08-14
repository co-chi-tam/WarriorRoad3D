require('../Utils/Log')();

const uuid = require('node-uuid');
const crypto = require('crypto');
const simpleData = require ('./simpleData');
const Promise = require('promise');
const RegExp = require("regex");

// CACHE MONSTER
var monsterCache = [];

var monsterModel = function(database) {
	// SCOPE
	var self = this;
	// COLLECTION NAME
	var collectionName = 'clMonsters';
	// MODEL
	var simpleModel = new simpleData(collectionName, database);	
	// RESPONSE MONSTER
	var responseMonster = function(monster) {	
		return {
			// uID: monster.uID,
			objectName: monster.objectName,
			objectAvatar: monster.objectAvatar,
			objectModel: monster.objectModel,
			characterAttackPoint: monster.characterAttackPoint,
			maxAttackPoint: monster.maxAttackPoint,
			characterAttackSpeed: monster.characterAttackSpeed,
			maxAttackSpeed: monster.maxAttackSpeed,
			characterDefendPoint: monster.characterDefendPoint,
			maxDefendPoint: monster.maxDefendPoint,
			characterHealthPoint: monster.characterHealthPoint,
			characterMaxHealthPoint: monster.characterMaxHealthPoint,
			maxHealthPoint: monster.maxHealthPoint,
			characterSkillSlots: monster.characterSkillSlots,
			characterLevel: monster.characterLevel,
			dataPerLevel: {
				characterAttackPoint: monster.dataPerLevel.characterAttackPoint,
				characterAttackSpeed: monster.dataPerLevel.characterAttackSpeed,
				characterDefendPoint: monster.dataPerLevel.characterDefendPoint,
				characterMaxHealthPoint: monster.dataPerLevel.characterMaxHealthPoint
			}
		};
	};
	// FIND MONSTER
	this.findMonster = function(monsterName) {
		return new Promise(function (resolve, reject) {
			if (monsterName) {
				var regexValue = monsterName;
				simpleModel.findData ({'objectName': { $regex: regexValue, $options: 'i' }})
				// FOUND HERO OWNER
				.then((compFind) => {
					var resMonster = [];
					compFind.forEach(function(item) {
						 resMonster.push (new responseMonster (item));
					});
					resolve (compFind);
				})
				// NOT FOUND HERO DATA
				.catch((errFind) => {
					reject (errFind);
				});
			} else {
				reject ('Fields empty.');
			}
		});
	};
	// GET MONSTER
	this.getMonsters = function () {
		return new Promise(function (resolve, reject) {
			// FIND CACHE
			if (monsterCache.length == 0) {
				simpleModel.findData ({})
				// FOUND HERO OWNER
				.then((compFind) => {
					var resMonster = [];
					compFind.forEach(function(item) {
						var reExportData = new responseMonster (item);
						resMonster.push (reExportData);
						monsterCache.push (reExportData);
					});
					resolve (resMonster);
				})
				// NOT FOUND HERO DATA
				.catch((errFind) => {
					reject (errFind);
				});
			} else {
				// RESPONSE CACHE
				resolve (monsterCache);
			}
		});
	};
};

module.exports = monsterModel;