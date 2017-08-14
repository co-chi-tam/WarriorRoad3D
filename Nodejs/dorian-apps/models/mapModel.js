require('../Utils/Log')();

const uuid = require('node-uuid');
const crypto = require('crypto');
const simpleData = require ('./simpleData');
const Promise = require('promise');

var mapModel = function (database) {
	
	// SCOPE
	var self = this;
	// COLLECTION NAME
	var collectionName = 'clMaps';
	// USER MODEL
	var simpleModel = new simpleData(collectionName, database);
	// MAP
	var newMap = function (ownerId, objs) {
		return {
			uID: uuid.v4().toString(),
			uOwner: ownerId,
			createdTime: new Date(),
			completed: false,
			mapObjects: objs
		};
	}
	
	// FIND MAP
	this.findMap = function(ownerId) {
		return new Promise(function (resolve, reject) {
			simpleModel.findData ({'uOwner': ownerId})
			// FOUND
			.then ((completeFind) => {
				resolve(completeFind[0]);
			})
			// ERROR
			.catch ((errorFind) => {
				reject(errorFind);
			});
		});
	};
	
	// INSERT MAP
	this.createMap = function(ownerId, mapObjects) {
		return new Promise (function (resolve, reject) {
			var createdMap = new newMap(ownerId, mapObjects);
			simpleModel.createData (createdMap)
			// CREATED 
			.then ((mapDocs) => {
				resolve (createdMap.mapObjects);
			})
			// ERROR CREATE
			.catch ((errDoc) => {
				reject (errDoc);
			});
		});
	}
	
	// UPDATE MAP
	this.updateMap = function(ownerId, mapObjects) {
		return new Promise (function (resolve, reject) {
			var createdMap = new newMap(ownerId, mapObjects);
			simpleModel.updateData ({'uOwner': ownerId}, createdMap)
			// UPDATE
			.then ((completeUpdate) => {
				resolve (createdMap);
			})
			// CAN NOT UPDATE USER
			.catch ((errorUpdate) => {
				reject('ERROR ' + errorUpdate);
			});
		});
	}
	
}

module.exports = mapModel;

























