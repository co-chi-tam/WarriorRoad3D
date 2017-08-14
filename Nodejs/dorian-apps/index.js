
const numCPUs = require('os').cpus().length || 4;
const cluster = require('cluster');
const appServer = require('./app');
const wsServer = require('./WSServer');

const mongodb = require('mongodb');
const mongoClient = mongodb.MongoClient;
const GameModel = require('./models/gameModel');

require('./Utils/Log')();

var dbUser = 'cochitam1990';
var dbPassword = 'Pro12%40833';
var dbName = 'heroku_clpx7q07';
var connectionString = "mongodb://" + dbUser + ":" + dbPassword + "@ds011933.mlab.com:11933/" + dbName;
var workers = [];

if (cluster.isMaster) {
	print('Cluster working ...');
	// FORK
	for (var i = 0; i < numCPUs; i++) {
		// NEW WORKER
		var currentWorker = cluster.fork();
		currentWorker.on ('message', function(msg) {
			if (msg.wMsg) {
				workers.forEach (function (item) {
					item.send (msg.wMsg);
				});
			} 
		});
		if (!workers.includes(currentWorker)) {
			workers.push (currentWorker);
		}
	}
	// ONLINE WORKER
	cluster.on('online', function(worker) {
		print ('Worker ' + worker.process.pid + ' is online.');
	});
	// EXIT WORKER
	cluster.on ('exit', function (worker, code, signal) {
		print ('Worker ' + worker.process.pid + ' died with code ' + code + ' and signal ' + signal);
		print ('Starting a new worker');
		// REMOVE WORKER
		if (workers.indexOf(worker) !== -1) {
			workers.splice (workers.indexOf(worker), 1);
		}
		// NEW WORKER
		var currentWorker = cluster.fork();
		currentWorker.on ('message', function(msg) {
			if (msg.wMsg) {
				workers.forEach (function (item) {
					item.send (msg.wMsg);
				});
			} 
		});
		if (!workers.includes(currentWorker)) {
			workers.push (currentWorker);
		}
	});
} else {
	mongoClient.connect(connectionString, function(err, database) {
		if (err) {
			print ('MongoDB Connection Error ' + err);
		} else {
			print ('MongoDB Connection Completed. ');
			var databaseGame = database.db(dbName);
			var app = new appServer(databaseGame);
			var ws = new wsServer(app.server, databaseGame, function (workerMsg) {
				process.send ({wMsg : workerMsg});
			});
			process.on('message', function(msg) {
				if (msg.eventName && msg.data) {
					ws.receiveBroadCast (msg.eventName, msg.data);
				}
			});
		}
	});
}



































