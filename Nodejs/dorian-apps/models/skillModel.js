require('../Utils/Log')();

const uuid = require('node-uuid');
const crypto = require('crypto');
const simpleData = require ('./simpleData');
const Promise = require('promise');

var skillModel = function (database) {
	// SCOPE
	var self = this;
	// COLLECTION NAME
	var collectionName = 'clSkills';
	// USER MODEL
	var simpleModel = new simpleData(collectionName, database);
	// RESPONSE SKILL
	var responseSkill = function(skill) {	
		return {
			uID: skill.uID,
			objectName: skill.objectName,
			objectModel: skill.objectModel,
			objectAvatar: skill.objectAvatar,
			characterClasses: skill.characterClasses,
			levelRequire : skill.levelRequire,
			skillDelay: skill.skillDelay,
			skillTime: skill.skillTime,
			skillEffects: skill.skillEffects,
		};
	};
	// FIND SKILL
	this.findSkills = function(charClass, charLevel) {
		return new Promise(function (resolve, reject) {
			if (charClass && charLevel) {
				var charLevelInt = parseInt (charLevel)
				simpleModel.findData ({$and: [{characterClasses: charClass},{levelRequire: {$lte: charLevelInt}}]})
				// FOUND HERO OWNER
				.then((compFindSkill) => {
					var resSkill = [];
					compFindSkill.forEach(function(item) {
						resSkill.push (new responseSkill (item));
					});
					resolve (resSkill);
				})
				// NOT FOUND HERO DATA
				.catch((errFindSkill) => {
					reject (errFindSkill);
				});
			} else {
				reject ('Fields empty.');
			}
		});
	};
	// FIND SKILL WITH NAME
	this.findSkillWithNames = function(skillNames) {
		return new Promise(function (resolve, reject) {
			if (skillNames) {
				simpleModel.findData ({ objectName: { $in: skillNames } })
				// FOUND HERO OWNER
				.then((compFindSkill) => {
					var resSkill = [];
					compFindSkill.forEach(function(item) {
						resSkill.push (new responseSkill (item));
					});
					resolve (resSkill);
				})
				// NOT FOUND HERO DATA
				.catch((errFindSkill) => {
					reject (errFindSkill);
				});
			} else {
				reject ('Fields empty.');
			}
		});
	};
}

module.exports = skillModel;