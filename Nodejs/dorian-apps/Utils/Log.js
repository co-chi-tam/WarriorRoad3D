module.exports = function() {
	
	this.print = function(val) {
		console.log ('[MSG]: ' + val);
	};
	
	this.debug = function (check, val) {
		if (check) {
			console.log (val);
		}
	};
	/**
	 * Returns a random number between min (inclusive) and max (exclusive)
	 */
	this.getRandomArbitrary = function (min, max) {
		return Math.random() * (max - min) + min;
	}

	/**
	 * Returns a random integer between min (inclusive) and max (inclusive)
	 * Using Math.round() will give you a non-uniform distribution!
	 */
	this.getRandomInt = function (min, max) {
		return Math.floor(Math.random() * (max - min + 1)) + min;
	}
}