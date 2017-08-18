using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CUtil {

	public static string GetMd5Hash(this string input)
	{
		// Create Hash
		MD5 md5Hash = MD5.Create ();

		// Convert the input string to a byte array and compute the hash.
		byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();

		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}

		// Return the hexadecimal string.
		return sBuilder.ToString();
	}

	// Verify a hash against a string.
	static bool VerifyMd5Hash(this string input, string hash)
	{
		// Hash the input.
		string hashOfInput = input.GetMd5Hash();

		// Create a StringComparer an compare the hashes.
		StringComparer comparer = StringComparer.OrdinalIgnoreCase;

		if (0 == comparer.Compare(hashOfInput, hash))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

}
