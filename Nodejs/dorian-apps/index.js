
const numCPUs = require('os').cpus().length || 4;
const cluster = require('cluster');
const appServer = require('./app');
const wsServer = require('./WSServer');

const mongodb = require('mongodb');
const mongoClient = mongodb.MongoClient;

require('./Utils/Log')();

// DATABASE USER NAME
var dbUser = 'cochitam1990';
// DATABASE PASSWORD
var dbPassword = 'Pro12%40833';
// DATABASE NAME
var dbName = 'heroku_clpx7q07';
// CONNECTION STRING
var connectionString = "mongodb://" + dbUser + ":" + dbPassword + "@ds011933.mlab.com:11933/" + dbName;
// LIST WORKER
var workers = [];
// MASTER
if (cluster.isMaster) {
	print('Cluster working ...');
	// FORK
	for (var i = 0; i < numCPUs; i++) {
		// NEW WORKER
		var currentWorker = cluster.fork();
		currentWorker.on ('message', function(msg) {
			workers.forEach (function (item) {
				item.send (msg);
			});
		});
		if (workers.indexOf(currentWorker) === -1) {
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
			workers.forEach (function (item) {
				item.send (msg);
			});
		});
		if (workers.indexOf(currentWorker) === -1) {
			workers.push (currentWorker);
		}
	});
// WORKER
} else {
	mongoClient.connect(connectionString, function(err, database) {
		if (err) {
			print ('MongoDB Connection Error ' + err);
		} else {
			print ('MongoDB Connection Completed. ');
			var databaseGame = database.db(dbName);
			var app = new appServer(databaseGame);
			var ws = new wsServer(app.server, databaseGame, function (workerMsg) {
				process.send (workerMsg);
			});
			process.on('message', function(msg) {
				// BROADCAST
				if (msg.broadcastData) {
					ws.receiveBroadCast (msg.broadcastData);
				// WORKER SYNC
				} else if (msg.workerSync) {
					ws.syncWorkerData (msg.workerSync);
				// PRIVATE SOCKET
				} else if (msg.privateData) {
					ws.receivePrivate (msg.privateData);
				}
			});
		}
	});
}



































