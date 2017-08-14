
const HOST = process.env.HOST || 'localhost';
const PORT = process.env.PORT || 5000;

const bodyParser = require('body-parser');
const http = require("http");
const cluster = require('cluster');
const express = require('express');

const urlHelper = require("./routes.js");

require('./Utils/Log')();

var appServer = function(database) {

	this.app = express();
	
	this.app.set('port', PORT);

	this.app.use(bodyParser.urlencoded({ extended: false }));
	this.app.use(bodyParser.json());
	
	this.app.use(express.static(__dirname + '/public'));
	this.app.use('/socket.io', express.static(__dirname + '/public/socket.io'));

	// views is directory for all template files
	this.app.set('views', __dirname + '/views');
	this.app.set('view engine', 'ejs'); 

	// var server = http.createServer(this.app);
	// this.server.listen(PORT);
	
	this.server = this.app.listen(PORT);
	
	print("Server starting ...");
	
	var middleWare = function (req, res, next) {
		// var headerVerify = req.headers.verify;
		// if (headerVerify) {
			next();
		// } else {
			// res.end("HEADER NOT VERIFY");
		// }
	};
	
	this.app.use(middleWare);
	
	urlHelper.setRequestUrl (this.app, database);
}

module.exports = appServer;