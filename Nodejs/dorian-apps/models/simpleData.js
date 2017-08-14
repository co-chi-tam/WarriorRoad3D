require('../Utils/Log')();

const Mongo = require('mongodb');
const MongoClient = Mongo.MongoClient;
const Promise = require('promise');

var simpleData = function(colName, database) {
	// SCOPE
	var self = this; 
	// COLLECTION CONNECTED
	this.collectionConnect = database.collection(colName);
	// Find And Sort Data
	this.findDataAndSort = function (query, sort) {
		return new Promise(function (resolve, reject) {
			if (self.collectionConnect) {
				self.collectionConnect.find(query, function(errQuery, cursor) {
					if (errQuery) {
						reject(errQuery);
					} else {
						cursor.sort(sort).toArray (function (errToArray, items){
							if (errToArray) {
								reject(errToArray);
							} else {
								if (items.length > 0) {
									resolve(items);
								} else {
									reject("Not found item !! ");
								}
							}
						});
					}
				});
			} else {
				reject("Not have collection to query item !! ");
			}	
		});
	};
	
	// Find Data
	this.findData = function (query, complete, error) {
		return new Promise(function (resolve, reject) {
			if (self.collectionConnect) {
				self.collectionConnect.find(query, function(errQuery, cursor) {
					if (errQuery) {
						reject(errQuery);
					} else {
						cursor.toArray (function (errToArray, items){
							if (errToArray) {
								reject(errToArray);
							} else {
								if (items.length > 0) {
									resolve(items);
								} else {
									reject("Not found item !! ");
								}
							}
						});
					}
				});
			} else {
				reject("Not have collection to query item !! ");
			}
		});
	};
	
	// Limit item
	this.findSkipAndLimitData = function (query, skip, limit, complete, error) {
		return new Promise(function (resolve, reject) {
			if (self.collectionConnect) {
				self.collectionConnect.find(query).skip(skip).limit(limit).toArray (function (errToArray, items) {
					if (errToArray) {
						reject(errToArray);
					} else {
						if (items) {
							if (items.length > 0) {
								resolve(items);
							} else {
								reject("Not found item !! ");
							}
						} else {
							reject("Not found item !! ");
						}
					}
				});
			} else {
				reject("Not have collection to query item !! ");
			}
		});
	};
	
	// Create Data
	this.createData = function (query, complete, error) {
		return new Promise(function (resolve, reject) {
			if (self.collectionConnect) {
				self.collectionConnect.insert(query, function(errInsert, docs){
					if (errInsert) {
						reject(errInsert);
					} else {
						resolve(docs);
					}
				});
			} else {
				reject("Not have collection to query item !! ");
			}
		});
	};
	
	// Update Data
	this.updateData = function (query, item, complete, error) {
		return new Promise(function (resolve, reject) {
			if (self.collectionConnect) {
				self.collectionConnect.update(query, { $set:item }, { upsert: false, multi: true }, function (errUpdate, result) {
					if (errUpdate) {
						reject(errUpdate);
					} else {
						if (result) {
							resolve(result);
						} else {
							reject("Query return null !! ");
						}
					}
				});
			} else {
				reject("Not have collection to query item !! ");
			}
		});
	};
	
	// Find And Update Data
	this.findAndSetData = function (query, item, complete, error) {
		return new Promise(function (resolve, reject) {
			if (self.collectionConnect) {
				self.collectionConnect.findAndModify(query, [], { $set:item }, { upsert: false }, function (errFindAndModify, result) {
					if (errFindAndModify) {
						reject(errFindAndModify);
					} else {
						if (result.value) {
							resolve(result.value);
						} else {
							reject("Query return null !! " + JSON.stringify(query) + " // " + JSON.stringify(item));
						}
					}
				});
			} else {
				reject("Not have collection to query item !! ");
			}
		});
	};
	
	// Modify Data
	this.findAndModifyData = function (query, item) {
		return new Promise(function (resolve, reject) {
			if (self.collectionConnect) {
				self.collectionConnect.findAndModify(query, [],  item , { upsert: false }, function (errFindAndModify, result) {
					if (errFindAndModify) {
						reject(errFindAndModify);
					} else {
						if (result.value) {
							resolve(result.value);
						} else {
							reject("Query return null !! ");
						}
					}
				});
			} else {
				reject("Not have collection to query item !! ");
			}
		});
	};
	
	// Aggregate
	this.aggregate = function (query, size) {
		return new Promise(function (resolve, reject) {
			if (self.collectionConnect) {
				self.collectionConnect.aggregate([{$match: query}, {$sample: {size: size}}],
				function (errAggregate, result) {
					if (errAggregate) {
						reject(errAggregate);
					} else {
						resolve(result);
					}
				});
			} else {
				reject("Not have collection to query item !! ");
			}
		});
	};
	
	// Delete Data
	this.deleteData = function (query) {
		return new Promise(function (resolve, reject) { 
			self.findData (query, function(items) {
				col.remove(items[0], function(errDelete, docs){
					if (errDelete) {
						reject(errDelete);
					} else {
						resolve(docs);
					}
				});
			}, function(err) {
				reject(err);
			});
		});
	};
}

module.exports = simpleData;
