
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


db.createCollection('clHeroes');
db.clHeroes.insertMany([{
    'uID': '4baf73401e6b68e50768edfbdb22dfad',
    'heroName': 'Warrior',
    'heroAvatar': 'Warrior-avatar',
    'heroModel': 'Warrior-model',
    'heroAttackPoint': 20,
    'heroAttackSpeed': 1.2,
    'heroDefendPoint': 20,
    'heroHealthPoint': 100,
    'heroSkillSlots': []
    },{
    'uID': 'b7a23f1b6b9c27fc5b8c6d840dc83f93',
    'heroName': 'Wizard',
    'heroAvatar': 'Wizard-avatar',
    'heroModel': 'Wizard-model',
    'heroAttackPoint': 35,
    'heroAttackSpeed': 2.2,
    'heroDefendPoint': 10,
    'heroHealthPoint': 80,
    'heroSkillSlots': []
    },{
    'uID': 'bb383f1abab8cdf7619b1723a21c6e1f',
    'heroName': 'Archer',
    'heroAvatar': 'Archer-avatar',
    'heroModel': 'Archer-model',
    'heroAttackPoint': 15,
    'heroAttackSpeed': 0.75,
    'heroDefendPoint': 15,
    'heroHealthPoint': 90,
    'heroSkillSlots': []
    }])

db.createCollection('clSkills');
db.clSkills.insertMany([{
    'uID': '502ec8465441f1d108b8c963ec402b08',
    'skillName': 'Normal Attack',
    'skillAvatar': 'NormalAttack-avatar',
    'skillModel': 'NormalAttack-model',
    'skillValue': [10],
    'SkillMethod': 'ApplyDamage'
    },{
    'uID': 'b4d0a149ec60fb7124d3d4d72ea8174b',
    'skillName': 'Bash',
    'skillAvatar': 'Bash-avatar',
    'skillModel': 'Bash-model',
    'skillValue': [20],
    'SkillMethod': 'ApplyDamage'
    },{
    'uID': '38c3a4da101090c04ae3428422e80c3f',
    'skillName': 'Fire ball',
    'skillAvatar': 'FireBall-avatar',
    'skillModel': 'FireBall-model',
    'skillValue': [15],
    'SkillMethod': 'ApplyDamage'
    }]);

db.clUsers.find({});
db.clUsers.remove({});

