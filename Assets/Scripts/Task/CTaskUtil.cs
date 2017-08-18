using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CTaskUtil {

		public static string HOST			= "https://dorian-apps.herokuapp.com/";
		public static string SOCKET_HOST	= "wss://dorian-apps.herokuapp.com/socket.io/";
		public static string LOGIN_URL		= HOST + "login";
		public static string REGISTER_URL	= HOST + "register";
		public static string USER_NAME		= "USER_NAME";
		public static string USER_PASSWORD	= "USER_PASSWORD";

		public static string USER_DATA 		= "USER_DATA";
		public static string HERO_DATA 		= "HERO_DATA";
		public static string SKILL_DATA_LIST = "SKILL_DATA_LIST";
		public static string HERO_TEMPLATES = "HERO_TEMPLATES";

		public static string FIGHTING_ROOM_RESPONSE_CODE = "FIGHTING_ROOM_RESPONSE_CODE";
		public static string FIGHTING_ROOM = "FIGHTING_ROOM";

		public static Dictionary<string, string> VERIFY_HEADERS = new Dictionary<string, string> () { 
			{"verify", "THIS IS VERIFY HEADERS."}
		};

		public static Dictionary<string, object> REFERENCES = new Dictionary<string, object> () { 
			{ USER_DATA,		new CUserData()	},
			{ HERO_DATA, 		new CCharacterData() },
			{ SKILL_DATA_LIST, 	new List<CSkillData>() },
			{ HERO_TEMPLATES, 	new Dictionary<string, CCharacterData>() },
			{ FIGHTING_ROOM_RESPONSE_CODE, 	string.Empty },
			{ FIGHTING_ROOM, 				new CRoomData () },
		};

		public static object Get(string name) {
			return REFERENCES [name];
		}

		public static T Get<T>(string name) {
			var value = REFERENCES [name];
			var convert = Convert.ChangeType (value, typeof(T));
			return (T)convert;
		}

		public static void Set(string name, object value) {
			REFERENCES [name] = value;
		}

	}

}
