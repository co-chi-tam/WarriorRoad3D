require('../Utils/Log')();

const uuid = require('node-uuid');
const crypto = require('crypto');
const simpleData = require ('./simpleData');
const Promise = require('promise');

var heroModel = function(database) {
	// SCOPE
	var self = this;
	// COLLECTION NAME
	var collectionName = 'clHeroes';
	// MODEL
	var simpleModel = new simpleData(collectionName, database);
	// Heroes template
	this.heroesTemplate = {
		warrior: {
			uID: uuid.v4().toString(),
			objectName: 'Warrior',
			objectAvatar: 'Warrior-avatar',
			objectModel: 'Warrior-model',
			characterClass: 'Warrior',
			characterAttackPoint: 20,
			maxAttackPoint: 500,
			characterAttackSpeed: 1,
			maxAttackSpeed: 2,
			characterDefendPoint: 20,
			maxDefendPoint: 500,
			characterHealthPoint: 100,
			characterMaxHealthPoint: 100,
			maxHealthPoint: 9999,
			currentGold: 500,
			maxGold: 999999999,
			goldPerStep: 25,
			currentEnergy: 30,
			maxEnergy: 30,
			lastUpdateEnergy: new Date(),
			characterSkillSlots: [
				{
					uID: '502ec8465441f1d108b8c963ec402b08',
					objectName: 'Normal Attack',
					objectAvatar: 'NormalAttack-avatar',
					objectModel: 'NormalAttack-model',
					characterClasses: ['Warrior','Archer','Wizard'],
					levelRequire: 0,
					skillDelay: 0.1,
					skillTime: 0.1,
					skillEffects: [
						{
							skillValue: 1,
							skillMethod: 'ApplyDamage'
						}
					]
				}
			],
			uOwner: '',
			characterLevel: 1,
			characterStep: 0,
			dataPerLevel: {
				characterAttackPoint: 2,
				characterAttackSpeed: 0.1,
				characterDefendPoint: 2,
				characterMaxHealthPoint: 5
			}
		},
		wizard: {
			uID: uuid.v4().toString(),
			objectName: 'Wizard',
			objectAvatar: 'Wizard-avatar',
			objectModel: 'Wizard-model',
			characterClass: 'Wizard',
			characterAttackPoint: 35,
			maxAttackPoint: 500,
			characterAttackSpeed: 1.5,
			maxAttackSpeed: 2,
			characterDefendPoint: 10,
			maxDefendPoint: 500,
			characterHealthPoint: 80,
			characterMaxHealthPoint: 80,
			maxHealthPoint: 9999,
			currentGold: 500,
			maxGold: 999999999,
			goldPerStep: 25,
			currentEnergy: 30,
			maxEnergy: 30,
			lastUpdateEnergy: new Date(),
			characterSkillSlots: [
				{
					uID: '502ec8465441f1d108b8c963ec402b08',
					objectName: 'Normal Attack',
					objectAvatar: 'NormalAttack-avatar',
					objectModel: 'NormalAttack-model',
					characterClasses: ['Warrior','Archer','Wizard'],
					levelRequire: 0,
					skillDelay: 0.1,
					skillTime: 0.1,
					skillEffects: [
						{
							skillValue: 1,
							skillMethod: 'ApplyDamage'
						}
					]
				}
			],
			uOwner: '',
			characterLevel: 1,
			characterStep: 0,
			dataPerLevel: {
				characterAttackPoint: 2,
				characterAttackSpeed: 0.1,
				characterDefendPoint: 2,
				characterMaxHealthPoint: 5
			}
		},
		archer: {
			uID: uuid.v4().toString(),
			objectName: 'Archer',
			objectAvatar: 'Archer-avatar',
			objectModel: 'Archer-model',
			characterClass: 'Archer',
			characterAttackPoint: 15,
			maxAttackPoint: 500,
			characterAttackSpeed: 0.75,
			maxAttackSpeed: 2,
			characterDefendPoint: 15,
			maxDefendPoint: 500,
			characterHealthPoint: 90,
			characterMaxHealthPoint: 90,
			maxHealthPoint: 9999,
			currentGold: 500,
			maxGold: 999999999,
			goldPerStep: 25,
			currentEnergy: 30,
			maxEnergy: 30,
			lastUpdateEnergy: new Date(),
			characterSkillSlots: [
				{
					uID: '502ec8465441f1d108b8c963ec402b08',
					objectName: 'Normal Attack',
					objectAvatar: 'NormalAttack-avatar',
					objectModel: 'NormalAttack-model',
					characterClasses: ['Warrior','Archer','Wizard'],
					levelRequire: 0,
					skillDelay: 0.1,
					skillTime: 0.1,
					skillEffects: [
						{
							skillValue: 1,
							skillMethod: 'ApplyDamage'
						}
					]
				}
			],
			uOwner: '',
			characterLevel: 1,
			characterStep: 0,
			dataPerLevel: {
				characterAttackPoint: 2,
				characterAttackSpeed: 0.1,
				characterDefendPoint: 2,
				characterMaxHealthPoint: 5
			}
		}
	}
	// New Heroes
	var newHero = function(heroType, heroName, ownerId) {	
		var tmpHero = self.heroesTemplate[heroType];
		if (typeof tmpHero === 'undefined') {
			tmpHero = self.heroesTemplate['warrior'];
		} 
		return {
			uID: uuid.v4().toString(),
			objectName: heroName,
			objectAvatar: tmpHero.objectAvatar,
			objectModel: tmpHero.objectModel,
			characterClass: tmpHero.characterClass,
			characterAttackPoint: tmpHero.characterAttackPoint,
			maxAttackPoint: tmpHero.maxAttackPoint,
			characterAttackSpeed: tmpHero.characterAttackSpeed,
			maxAttackSpeed: tmpHero.maxAttackSpeed,
			characterDefendPoint: tmpHero.characterDefendPoint,
			maxDefendPoint: tmpHero.maxDefendPoint,
			characterHealthPoint: tmpHero.characterHealthPoint,
			characterMaxHealthPoint: tmpHero.characterMaxHealthPoint,
			maxHealthPoint: tmpHero.maxHealthPoint,
			currentGold: tmpHero.currentGold,
			maxGold: tmpHero.maxGold,
			goldPerStep: tmpHero.goldPerStep,
			currentEnergy: tmpHero.currentEnergy,
			maxEnergy: tmpHero.maxEnergy,
			lastUpdateEnergy: new Date(),
			characterSkillSlots: tmpHero.characterSkillSlots,
			uOwner: ownerId,
			characterLevel: 1,
			characterStep: 0,
			dataPerLevel: {
				characterAttackPoint: tmpHero.dataPerLevel.characterAttackPoint,
				characterAttackSpeed: tmpHero.dataPerLevel.characterAttackSpeed,
				characterDefendPoint: tmpHero.dataPerLevel.characterDefendPoint,
				characterMaxHealthPoint: tmpHero.dataPerLevel.characterMaxHealthPoint
			}
		};
	};
	// RESPONSE HERO
	var responseHero = function(hero) {	
		return {
			uID: hero.uID,
			objectName: hero.objectName,
			objectAvatar: hero.objectAvatar,
			objectModel: hero.objectModel,
			characterClass: hero.characterClass,
			characterAttackPoint: hero.characterAttackPoint,
			maxAttackPoint: hero.maxAttackPoint,
			characterAttackSpeed: hero.characterAttackSpeed,
			maxAttackSpeed: hero.maxAttackSpeed,
			characterDefendPoint: hero.characterDefendPoint,
			maxDefendPoint: hero.maxDefendPoint,
			characterHealthPoint: hero.characterHealthPoint,
			characterMaxHealthPoint: hero.characterMaxHealthPoint,
			maxHealthPoint: hero.maxHealthPoint,
			currentGold: hero.currentGold,
			maxGold: hero.maxGold,
			goldPerStep: hero.goldPerStep,
			currentEnergy: hero.currentEnergy,
			maxEnergy: hero.maxEnergy,
			lastUpdateEnergy: hero.lastUpdateEnergy,
			characterSkillSlots: hero.characterSkillSlots,
			characterLevel: hero.characterLevel,
			characterStep: hero.characterStep,
			dataPerLevel: {
				characterAttackPoint: hero.dataPerLevel.characterAttackPoint,
				characterAttackSpeed: hero.dataPerLevel.characterAttackSpeed,
				characterDefendPoint: hero.dataPerLevel.characterDefendPoint,
				characterMaxHealthPoint: hero.dataPerLevel.characterMaxHealthPoint
			}
		};
	};
	// FIND HERO
	this.findHero = function(ownerId) {
		return new Promise(function (resolve, reject) {
			if (ownerId) {
				simpleModel.findData ({'uOwner': ownerId})
				// FOUND HERO OWNER
				.then((compFindHero) => {
					var resHero = new responseHero (compFindHero[0]);
					resolve (resHero);
				})
				// NOT FOUND HERO DATA
				.catch((errFindHero) => {
					reject (errFindHero);
				});
			} else {
				reject ('Fields empty.');
			}
		});
	};
	// CREATE HERO
	this.createHero = function(heroType, heroName, ownerId) {
		return new Promise(function (resolve, reject) {
			if (heroType && heroName && ownerId) {
				simpleModel.findData ({'uOwner': ownerId})
				// FOUND HERO OWNER
				.then((compFindHero) => {
					resolve (compFindHero);
				})
				// NOT FOUND HERO DATA
				.catch((errFindHero) => {
					var createdHero = new newHero(heroType, heroName, ownerId);
					simpleModel.createData (createdHero)
					// CREATE HERO
					.then ((userDocs) => {
						var resHero = new responseHero (createdHero);
						resolve (resHero);
					})
					// ERROR CREATE
					.catch ((errCreate) => {
						reject (errCreate);
					});
				});
			} else {
				reject ('Fields empty.');
			}
		});
	};
	
	// GET HERO BY LEVEL
	this.findHeroBaseLevel = function(exceptID, minLevel, maxLevel, size) {
		return new Promise(function (resolve, reject) {
			if (exceptID && minLevel && maxLevel && size) {
				simpleModel.aggregate({characterLevel: { $gte: minLevel, $lt: maxLevel }, uID: { $ne: exceptID }}, size)
				// FOUND DATA
				.then ((foundData) => {
					var resHero = [];
					for(var i = 0; i < foundData.length; i++) {
						resHero.push (new responseHero (foundData[i]));
					}
					resolve (resHero);
				})
				// ERROR FOUND DATA
				.catch ((foundError) => {
					reject (foundError);
				});
			} else {
				reject ('Fields is empty');
			}
		});
	};
	
	// UPDATE HERO
	this.updateHero = function (ownerId, query) {
		return new Promise(function (resolve, reject) {
			if (ownerId) {
				simpleModel.findData ({'uOwner': ownerId})
				// FOUND HERO OWNER
				.then((compFindHero) => {
					simpleModel.updateData ({'uOwner': ownerId}, query)
					// UPDATED
					.then ((completeUpdate) => {
						resolve (completeUpdate);
					})
					// ERROR UPDATE
					.catch ((errorUpdate) => {
						reject('ERROR ' + errorUpdate);
					});
				})
				// NOT FOUND HERO DATA
				.catch((errFindHero) => {
					reject (errFindHero);
				});
			} else {
				reject ('Fields empty.');
			}
		});
	}
	
	// UPDATE HERO STEP
	this.modifyHero = function (ownerId, query) {
		return new Promise(function (resolve, reject) {
			if (ownerId) {
				simpleModel.findData ({'uOwner': ownerId})
				// FOUND HERO OWNER
				.then((compFindHero) => {
					simpleModel.findAndModifyData ({'uOwner': ownerId}, query)
					// UPDATED
					.then ((completeUpdate) => {
						resolve (completeUpdate);
					})
					// ERROR UPDATE
					.catch ((errorUpdate) => {
						reject('ERROR ' + errorUpdate);
					});
				})
				// NOT FOUND HERO DATA
				.catch((errFindHero) => {
					reject (errFindHero);
				});
			} else {
				reject ('Fields empty.');
			}
		});
	}
};

module.exports = heroModel;





















