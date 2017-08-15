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
	this.mapPathTemplate = ['ForestBlock', 'LavaBlock'];
	// MAP
	var newMap = function (ownerId, path, objs) {
		return {
			uID: uuid.v4().toString(),
			uOwner: ownerId,
			createdTime: new Date(),
			completed: false,
			mapPath: path,
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
	this.createMap = function(ownerId, mapPath, mapObjects) {
		return new Promise (function (resolve, reject) {
			var createdMap = new newMap(ownerId, mapPath, mapObjects);
			simpleModel.createData (createdMap)
			// CREATED 
			.then ((mapDocs) => {
				resolve (createdMap);
			})
			// ERROR CREATE
			.catch ((errDoc) => {
				reject (errDoc);
			});
		});
	}
	// UPDATE MAP
	this.updateMap = function(ownerId, mapPath, mapObjects) {
		return new Promise (function (resolve, reject) {
			var createdMap = new newMap(ownerId, mapPath, mapObjects);
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

























