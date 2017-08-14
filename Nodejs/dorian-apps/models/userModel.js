require('../Utils/Log')();

const uuid = require('node-uuid');
const crypto = require('crypto');
const simpleData = require ('./simpleData');
const Promise = require('promise');

var userModel = function(database) {
	// SCOPE
	var self = this;
	// COLLECTION NAME
	var collectionName = 'clUsers';
	// USER MODEL
	var simpleModel = new simpleData(collectionName, database);
	// EXPIRE TIME
	var userExpire = 1; // 1 DAY
	// TEMP NEW USER
	var newUser = function(userName, userPassword, userEmail, userDisplayName, userLoginMethod) {
		var currentDate = new Date();
		var expireDate = new Date();
		expireDate.setDate (currentDate.getDate() + userExpire);
		return {
			uID: uuid.v4().toString(),
			uName: userName,
			uPassword: userPassword,
			uDisplayName: userDisplayName,
			uEmail: userEmail,
			uLoginMethod: userLoginMethod,
			uLastLogin: currentDate,
			uToken: crypto.createHash('md5').update('user-' + userName + '+' + userEmail+ '-' + userDisplayName).digest('hex'),
			uExpireTime: expireDate,
			uCreateTime: currentDate,
			uActive: true
		};
	};
	// FIND USER
	this.findUser = function(userName, userToken) {
		return new Promise(function (resolve, reject) {
			simpleModel.findData ({$and: [{'uName': userName}, {'uToken': userToken}]})
			// FOUND USER
			.then ((completeFind) => {
				resolve(completeFind[0]);
			})
			// ERROR USER
			.catch ((errorFind) => {
				reject(errorFind);
			});
		});
	};
	// USER REGISTER
	this.UserRegister = function(userName, userPassword, userEmail, userDisplayName, userLoginMethod) {
		return new Promise(function (resolve, reject) {
			simpleModel.findData ({$or: [{'uName': userName}, {'uEmail': userEmail}, {'uDisplayName': userDisplayName}]})
			// FOUND USER
			.then((result) => {
				reject('User existed, please choise orther user name, email or display name');
			})
			// THIS IS NEW USER
			.catch((errFindUser) => {
				var createdUser = new newUser(userName, userPassword, userEmail, userDisplayName, userLoginMethod);
				simpleModel.createData (createdUser)
				// CREATE USER
				.then ((userDocs) => {
					resolve ({
						userName: createdUser.uName,
						userEmail: createdUser.uEmail,
						userDisplayName: createdUser.uDisplayName,
						token: createdUser.uToken
					});
				})
				// ERROR CREATE
				.catch ((errCreate) => {
					reject (errCreate);
				});
			});
		});
	};
	// USER LOGIN
	this.UserLogin = function(userName, userPassword) {
		return new Promise(function (resolve, reject) {
			simpleModel.findData ({$and: [{'uName': userName}, {'uPassword': userPassword}]})
			// FOUND USER
			.then ((completeFind) => {
				var loggedUser = completeFind[0];
				var currentDate = new Date();
				var expireDate = new Date();
				expireDate.setDate (currentDate.getDate() + userExpire);
				var token = crypto.createHash('md5').update('user-' + loggedUser.uName + '+' + loggedUser.uEmail + '-' + loggedUser.uDisplayName + "+" + expireDate).digest('hex').toString();
				
				simpleModel.updateData ({$and: [{'uName': userName}, {'uPassword': userPassword}]}, {'uLastLogin': new Date(), 'uToken': token, 'uExpireTime': expireDate})
				// UPDATE LOGGED IN
				.then ((completeUpdate) => {
					resolve ({
						userName: loggedUser.uName,
						userEmail: loggedUser.uEmail,
						userDisplayName: loggedUser.uDisplayName,
						token: token
					});
				})
				// CAN NOT UPDATE USER
				.catch ((errorUpdate) => {
					reject('ERROR ' + errorUpdate + ' == User not existed !!');
				});
			})
			// NOT FOUND USER
			.catch ((errorFindUser) => {
				reject('ERROR ' + errorFindUser + ' == User not existed !!');
			});
		});
	};
	// AUTHORIZE USER 
	this.UserAuthorise = function (userName, token) {
		return new Promise(function (resolve, reject) {
			simpleModel.findData ({$and: [{'uName': userName}, {'uToken': token}]})
			// FOUND USER
			.then ((completeFind) => {
				var currentTime = new Date ();
				var expireTime = new Date (Date.parse (completeFind[0]['uExpireTime'].toISOString()));
				if (currentTime.getTime() < expireTime.getTime()) {
					resolve ({
						userId: completeFind[0]['uID'],
						userName: completeFind[0]['uName'],
						loginToken: completeFind[0]['uToken']
					});
				} else {
					reject('Not authorize !!');
				}
			})
			// NOT FOUND USER
			.catch ((errorFind) => {
				reject('Not authorize !!');
			});
		});
	};
	// AUTHORIZE TOKEN 
	this.verifyToken = function (token) {
		return new Promise(function (resolve, reject) {
			simpleModel.findData ({'uToken': token})
			// FOUND TOKEN
			.then ((completeFind) => {
				var currentTime = new Date ();
				var expireTime = new Date (Date.parse (completeFind[0]['uExpireTime'].toISOString()));
				if (currentTime.getTime() < expireTime.getTime()) {
					resolve ({
						userId: completeFind[0]['uID'],
						userName: completeFind[0]['uName'],
						loginToken: completeFind[0]['uToken']
					});
				} else {
					reject('Not authorize !!');
				}
			})
			// NOT FOUND TOKEN
			.catch ((errorFind) => {
				reject('Not authorize !!');
			});
		});
	};
	// VERIFY USER LOGGIN INPUT
	this.verifyLoginInput = function(uName, uPass) {
		return new Promise(function (resolve, reject) {
			if (uName && uPass) {
				if (checkLength (uName.toString(), 5, 18) 
					&& checkSpecialChar(uName.toString()) == false) {
					if (checkLength (uPass.toString(), 5, 18)) {
						resolve (self);
					} else {
						reject("Password must length greater 5 and lower 18 character.");
					}
				} else {
					reject("User name must length greater 5 and lower 18 character.");
				}
			} else {
				reject("Field empty.");
			}
		});
	};
	// VERIFY USER REGISTER INPUT
	this.verifyRegisterInput = function(uName, uPass, uEmail, uDisplayName) {
		return new Promise(function (resolve, reject) {
			if (uName && uPass && uEmail && uDisplayName) {
				if (checkLength (uName.toString(), 5, 18) 
					&& checkSpecialChar(uName.toString()) == false) {
					if (checkLength (uPass.toString(), 5, 18)) {
						if (checkLength (uEmail.toString(), 5, 50)) {
							if (checkLength (uDisplayName.toString(), 5, 18) 
							&& checkSpecialChar(uDisplayName.toString()) == false) {
								resolve (self);
							} else {
								reject ("Display name must length greater 5 and lower 18 character.");
							}
						} else {
							reject ("Email must length greater 5 and lower 50 character.");
						}
					} else {
						reject ("Password must length greater 5 and lower 18 character.");
					}
				} else {
					reject("User name must length greater 5 and lower 18 character.");
				}
			} else {
				reject ("Field empty.");
			}
		});
	};
};

function checkLength(valueStr, min, max) {
	if (valueStr.length < min || valueStr.length > max)
		return false;
	return true;
}

function checkSpecialChar(valueStr) {
	 var specialChars = "<>@!#$%^&*()_+[]{}?:;|'\"\\,./~`-=@!";
	 for(var i = 0; i < specialChars.length; i++){
	   if(valueStr.indexOf(specialChars[i]) > -1){
		   return true;
		}
	 }
	 return false;
}

module.exports = userModel;