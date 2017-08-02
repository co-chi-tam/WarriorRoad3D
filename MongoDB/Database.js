
db.createCollection('clUsers');
db.clUsers.createIndex({'uName': 1, 'uEmail': 1, 'uDisplayName': 1}, {unique: true});
db.clUsers.insert({
    "uID": "0cef72b6-a5c8-490d-ba6c-efb28a80bcc1",
    "uName": "user0001",
    "uPassword": "123456",
    "uDisplayName": "Hero-01",
    "uEmail": "user0001@gmail.com",
    "uLoginMethod": "FB",
    "uToken": "8d795a36fdfa772b51fcf571f88bdefd",
    "uExpireTime": new Date(),
    "uCreateTime": new Date(),
    "uActive": true
});
db.clUsers.find({});

db.createCollection('clHeroes');
db.clHeroes.createIndex({'uID': 1, 'objectName': 1, 'uOwner': 1}, {unique: true});
db.clHeroes.insertMany([{
    'uID': '4baf73401e6b68e50768edfbdb22dfad',
    'objectName': 'Warrior',
    'objectAvatar': 'Warrior-avatar',
    'objectModel': 'Warrior-model',
    'heroAttackPoint': 20,
    'heroAttackSpeed': 1.2,
    'heroDefendPoint': 20,
    'heroHealthPoint': 100,
    'heroSkillSlots': [],
    'uOwner': ''
    },{
    'uID': 'b7a23f1b6b9c27fc5b8c6d840dc83f93',
    'objectName': 'Wizard',
    'objectAvatar': 'Wizard-avatar',
    'objectModel': 'Wizard-model',
    'heroAttackPoint': 35,
    'heroAttackSpeed': 2.2,
    'heroDefendPoint': 10,
    'heroHealthPoint': 80,
    'heroSkillSlots': [],
    'uOwner': ''
    },{
    'uID': 'bb383f1abab8cdf7619b1723a21c6e1f',
    'objectName': 'Archer',
    'objectAvatar': 'Archer-avatar',
    'objectModel': 'Archer-model',
    'heroAttackPoint': 15,
    'heroAttackSpeed': 0.75,
    'heroDefendPoint': 15,
    'heroHealthPoint': 90,
    'heroSkillSlots': [],
    'uOwner': ''
    }]);
db.clHeroes.find({});

db.createCollection('clSkills');
db.clSkills.createIndex({'uID': 1, 'skillName': 1}, {unique: true});   
db.clSkills.insertMany([{
    'uID': '502ec8465441f1d108b8c963ec402b08',
    'objectName': 'Normal Attack',
    'objectAvatar': 'NormalAttack-avatar',
    'objectModel': 'NormalAttack-model',
    'skillTime': 0,
    'skillEffectPerTime': 0,
    'skillEffects': [
        {
            'skillValue': 15,
            'SkillMethod': 'ApplyDamage'
        }
    ]
    },{
    'uID': 'b4d0a149ec60fb7124d3d4d72ea8174b',
    'objectName': 'Bash',
    'objectAvatar': 'Bash-avatar',
    'objectModel': 'Bash-model',
    'skillTime': 0,
    'skillEffectPerTime': 0,
    'skillTriggers': [
        {
            'skillValue': 20,
            'SkillMethod': 'ApplyDamage'
        }
    ]
    },{
    'uID': '38c3a4da101090c04ae3428422e80c3f',
    'objectName': 'Fire ball',
    'objectAvatar': 'FireBall-avatar',
    'objectModel': 'FireBall-model',
    'skillTime': 0,
    'skillEffectPerTime': 0,
    'skillTriggers': [
        {
            'skillValue': 25,
            'SkillMethod': 'ApplyDamage'
        }
    ]
    }]);

db.clSkills.find({});

