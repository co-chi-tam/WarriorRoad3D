require('./models/resultResponse')();

require('./Utils/Log')();

var user 			= require('./controllers/user_controller');
var hero 			= require('./controllers/hero_controller');
var monster 		= require('./controllers/monster_controller');
var game 			= require('./controllers/game_controller');
var skill 			= require('./controllers/skill_controller');
var miniFighting 	= require('./controllers/mini_game_fighting_controller');
var playerQueue 	= require('./controllers/player_queue_controller');

exports.setRequestUrl = function(app, database){
	// INIT
	user.init 			(database);
	hero.init 			(database);
	monster.init		(database);
	game.init 			(database);
	skill.init			(database);
	miniFighting.init 	(database);
	playerQueue.init 	(database);
	
	// URL POST REGISTER
    app.post('/register',		user.postUserRegister);
	// URL POST LOGIN
    app.post('/login',			user.postUserLogin);
	// URL GET AUTHORISE
	app.get('/auth', 			user.getUserAuthorise);
	
	// URL POST CREATE HERO
	app.post('/create/hero',	hero.postCreateHero);
	// URL GET HERO
	app.get('/hero',			hero.getHero);
	// URL GET HERO BASE LEVEL
	app.get('/hero/level',		hero.getHeroBaseLevel);
	
	// URL GET MONSTER
	app.get('/monster',			monster.getMonster);
	// URL GET MONSTER
	app.get('/all_monsters',	monster.getAllMonsters);
	
	// URL GET MAP
	app.get('/map',				game.getGenerateMap);
	
	//URL GET SKILL
	app.get('/skills',			skill.getSkills);
	
	// TEST
	app.get('/', function(request, response) {
		// response.render('pages/index');
		response.end (createResult(1, {
			Server: 'OK'
		}));
	});
	
};